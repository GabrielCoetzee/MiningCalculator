using MiningCalculator.Services.Abstract;

namespace MiningCalculator.Services.Concrete
{
    public class GoldCalculationService : IGoldCalculationService
    {
        public double CalculateGPerTon(double cmgt, double stopingWidth, bool useCentimeters)
        {
            if (useCentimeters)
                stopingWidth /= 1000;

            return Math.Round(cmgt / stopingWidth * 100, 3);
        }

        public double CalculateSquareMeters(double length, double width)
        {
            return Math.Round(length * width, 3);
        }

        public double CalculateTons(double squareMeters, double stopingWidth)
        {
            return Math.Round(squareMeters * (stopingWidth / 100) * 2.78, 3);
        }

        public double CalculateGold(double tons, double gPerTon)
        {
            return Math.Round(tons * gPerTon / 1000, 3);
        }
    }
}
