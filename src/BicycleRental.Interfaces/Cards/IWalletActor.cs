using Dapr.Actors;

namespace BicycleRental.Interfaces.Cards
{
    public interface IWalletActor : IActor
    {
        Task<decimal> GetBalance();
        Task<decimal> Deposit(decimal amount);
        Task<decimal> Withdraw(decimal amount);

        Task Reset();
        //Task Lock(decimal amount);
        //Task Unlock(decimal amount);
    }    
}
