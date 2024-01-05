using BicycleRental.Interfaces;

namespace BicycleRental.Service
{
    public class RandomRater : IRater
    {
        public decimal CalculatePrice(TimeSpan rentalDuration)
        {
            return new decimal(new Random().NextDouble() * 10);
        }
    }
}
