namespace MiningCalculator.Configuration;

public record SubstancesSettings
{
    public Substance[] Substances { get; set; }
}

public record Substance
{
    public string Name { get; init; }
    public double Density { get; init; }
}
