using Dapr.Actors;

namespace BicycleRental.Interfaces.Cards
{
    public interface IBicycleActor : IActor
    {
        Task StartRenting(string cardId);
        Task FinishRenting();

        Task Reset();
    }    
}
