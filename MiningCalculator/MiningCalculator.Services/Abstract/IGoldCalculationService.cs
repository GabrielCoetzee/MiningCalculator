using System;
using System.Collections.Generic;
using System.Text;

namespace MiningCalculator.Services.Abstract
{
    public interface IGoldCalculationService
    {
        double CalculateGPerTon(double cmgt, double stopingWidth, bool useCentimeters);
        double CalculateSquareMeters(double length, double width);
        double CalculateTons(double squareMeters, double stopingWidth);
        double CalculateGold(double tons, double gPerTon);
    }
}
