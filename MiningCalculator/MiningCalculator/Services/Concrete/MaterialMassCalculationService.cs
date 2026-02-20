using MiningCalculator.Services.Abstract;

namespace MiningCalculator.Services.Concrete
{
    public class MaterialMassCalculationService : IMaterialMassCalculationService
    {
        public double CalculateRectangle(double length, double width, double height, double density)
        {
            return Math.Round(length * width * height * density * 1000, 3);
        }

        public double CalculateCyclinder(double diameter, double height, double density)
        {
            var radius = diameter / 2;

            return Math.Round(Math.PI * Math.Pow(radius, 2) * height * density * 1000, 3);
        }
    }
}
