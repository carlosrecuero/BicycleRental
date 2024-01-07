using BicycleRental.Interfaces;

namespace BicycleRental.Service
{
    public class RandomRater : IRater
    {
        public decimal CalculatePrice(TimeSpan rentalDuration)
        {
            return 1.0m;
            //return new decimal(new Random().NextDouble() * 10);
        }
    }
}
