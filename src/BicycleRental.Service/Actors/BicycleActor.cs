using BicycleRental.Interfaces;
using BicycleRental.Interfaces.Cards;
using Dapr.Actors.Client;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BicycleRental.Service.Actors
{
    public class BicycleActor : Actor, IBicycleActor, IRemindable
    {
        private readonly string StatusKey = "status";
        private readonly string MaximumDurationReminderName = "maximumDurationReminder";
        private readonly string PingTimerName = "pingTimer";

        private readonly TimeSpan MaximumDuration = TimeSpan.FromSeconds(5);
        private readonly TimeSpan PingInterval = TimeSpan.FromHours(1);
        private readonly TimeSpan None = TimeSpan.FromMilliseconds(-1);
        //private readonly decimal PenaltyBond = 100;

        private readonly IRater _rater;

        public BicycleActor(ActorHost host, IRater rater) : base(host)
        {
            _rater = rater;
        }

        public async Task StartRenting(string cardId)
        {
            var state = await this.StateManager.GetOrAddStateAsync<State>(StatusKey, new State());

            if (state.CurrentRental is not null)
            {
                throw new InvalidOperationException("Bicycle is already rented");
            }
            state.CurrentRental = new Rental(cardId);

            await this.RegisterReminderAsync(MaximumDurationReminderName, state: null, dueTime: MaximumDuration, period: None);

            await this.StateManager.SetStateAsync(StatusKey, state);
        }

        public async Task FinishRenting()
        {
            var state = await this.StateManager.GetOrAddStateAsync<State>(StatusKey, new State());

            var rental = state.CurrentRental;
            if (rental is null)
            {
                throw new InvalidOperationException("Bicycle has not a rental yet");
            }
            rental!.EndDateTime = DateTime.UtcNow;

            var price = _rater.CalculatePrice(rental.Duration!.Value);

            var rentalWalletActorId = new ActorId(rental.CardId);
            var rentalWalletActor = ActorProxy.Create<IWalletActor>(rentalWalletActorId, "WalletActor");
            await rentalWalletActor.Withdraw(price);

            state.CurrentRental = null;

            await this.StateManager.SetStateAsync(StatusKey, state);
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName != MaximumDurationReminderName)
                throw new ArgumentException("Unexpected reminder name", nameof(reminderName));
            
            await ReportInfraction();
        }

        private async Task ReportInfraction()
        {
            //set penalty bond
            var state = await this.StateManager.GetOrAddStateAsync<State>(StatusKey, new State());

            var rental = state.CurrentRental;
            if (rental is null)
            {
                throw new InvalidOperationException("Bicycle has not a rental yet");
            }

            Console.WriteLine($"Bicycle {Host.Id}: Infraction reported!!");
        }

        protected override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            await this.RegisterTimerAsync(PingTimerName, nameof(OnLivenessTimerCallBack), null, dueTime: PingInterval, period: PingInterval);
        }

        private Task OnLivenessTimerCallBack(byte[] data)
        {
            Console.WriteLine($"Bicycle {Host.Id}: Live");
            return Task.CompletedTask;
        }

        public async Task Reset()
        {
            await this.StateManager.TryRemoveStateAsync(StatusKey);
            await this.UnregisterReminderAsync(MaximumDurationReminderName);
            await this.UnregisterTimerAsync(PingTimerName);
        }

        public record State
        {
            public Rental? CurrentRental { get; set; }
        }

        public record Rental
        {
            public string CardId { get; init; }

            public DateTime StartDateTime { get; init; }
            public DateTime? EndDateTime { get; set; }
            public TimeSpan? Duration => EndDateTime.HasValue ? EndDateTime.Value - StartDateTime : default;

            public Rental(string cardId)
            {
                CardId = cardId;
                StartDateTime = DateTime.UtcNow;
            }
        }
    }
}
