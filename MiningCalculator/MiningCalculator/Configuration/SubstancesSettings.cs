namespace MiningCalculator.Configuration;

public record SubstancesSettings
{
    public required Substance[] Substances { get; set; }
}

public record Substance
{
    public required string Name { get; init; }
    public required double Density { get; init; }
}
