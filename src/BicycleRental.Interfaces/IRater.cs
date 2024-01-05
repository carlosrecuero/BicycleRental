using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BicycleRental.Interfaces
{
    public interface IRater
    {
        public decimal CalculatePrice(TimeSpan rentalDuration);
    }
}
