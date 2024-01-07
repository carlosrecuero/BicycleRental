using BicycleRental.Interfaces.Cards;
using Dapr.Actors.Runtime;

namespace BicycleRental.Service.Actors
{
    public class WalletActor : Actor, IWalletActor
    {
        private const string BalanceKey = "balance";
        private const decimal defaultBalance = 0;
        public WalletActor(ActorHost host) : base(host) { }

        public Task<decimal> GetBalance()
        {
            return this.StateManager.GetOrAddStateAsync<decimal>(BalanceKey, defaultBalance);
        }

        public async Task<decimal> Withdraw(decimal withdrawAmount)
        {
            var balance = await this.StateManager.GetOrAddStateAsync<decimal>(BalanceKey, defaultBalance);

            var updatedBalance = balance - withdrawAmount;
            if (updatedBalance < 0)
            {
                throw new OverdraftException(balance, withdrawAmount);
            }

            await this.StateManager.SetStateAsync(BalanceKey, balance);

            return updatedBalance;
        }

        public async Task<decimal> Deposit(decimal depositAmount)
        {
            var balance = await this.StateManager.GetOrAddStateAsync<decimal>(BalanceKey, defaultBalance);

            var updatedBalance = balance + depositAmount;
            await this.StateManager.SetStateAsync(BalanceKey, updatedBalance);

            return updatedBalance;
        }

        public async Task Reset()
        {
            await this.StateManager.TryRemoveStateAsync(BalanceKey);
        }
    }

    public class OverdraftException : Exception
    {
        public OverdraftException(decimal balance, decimal amount)
            : base($"Your current balance is {balance:c} - that's not enough to withdraw {amount:c}.")
        {
        }
    }
}
