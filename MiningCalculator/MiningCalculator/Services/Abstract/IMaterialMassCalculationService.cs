namespace MiningCalculator.Services.Abstract
{
    public interface IMaterialMassCalculationService
    {
        double CalculateRectangle(double length, double width, double height, double density);

        double CalculateCyclinder(double diameter, double height, double density);
    }
}
