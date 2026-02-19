using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using MiningCalculator.Configuration;
using MiningCalculator.Services.Abstract;
using System.Globalization;
using System.Reflection;

namespace MiningCalculator.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly IEnumerable<Substance> _substances;

    private readonly IMaterialMassCalculationService _calculationService;

    [ObservableProperty]
    private List<string> _substanceNames = [];

    [ObservableProperty]
    private int _selectedSubstanceIndex;

    [ObservableProperty]
    private bool _isManualDensity;

    [ObservableProperty]
    private bool _isManualDensityEntryEnabled;

    [ObservableProperty]
    private bool _isPickerEnabled = true;

    [ObservableProperty]
    private string _manualDensityText = string.Empty;

    [ObservableProperty]
    private bool _isRectangleSelected = true;

    [ObservableProperty]
    private bool _isCylinderSelected;

    [ObservableProperty]
    private bool _isRectangleVisible = true;

    [ObservableProperty]
    private bool _isCylinderVisible;

    [ObservableProperty]
    private string _lengthText = string.Empty;

    [ObservableProperty]
    private string _widthText = string.Empty;

    [ObservableProperty]
    private string _heightText = string.Empty;

    [ObservableProperty]
    private string _rectangleAnswer = string.Empty;

    [ObservableProperty]
    private string _diameterText = string.Empty;

    [ObservableProperty]
    private string _cylinderHeightText = string.Empty;

    [ObservableProperty]
    private string _cylinderAnswer = string.Empty;

    [ObservableProperty]
    private string _versionText = string.Empty;

    public MainPageViewModel(IOptions<SubstancesSettings> substancesSettings, IMaterialMassCalculationService calculationService)
    {
        _substances = substancesSettings.Value.Substances;
        _calculationService = calculationService;

        SubstanceNames = [.. _substances.Select(s => s.Name)];
        SelectedSubstanceIndex = 0;

        VersionText = $"Version {Assembly.GetExecutingAssembly().GetName().Version}";
    }

    partial void OnIsManualDensityChanged(bool value)
    {
        IsManualDensityEntryEnabled = value;
        IsPickerEnabled = !value;

        if (!value)
            ManualDensityText = string.Empty;
    }

    partial void OnIsRectangleSelectedChanged(bool value)
    {
        IsRectangleVisible = value;
    }

    partial void OnIsCylinderSelectedChanged(bool value)
    {
        IsCylinderVisible = value;
    }

    private bool TryGetRelativeDensity(out double density)
    {
        density = 0;

        if (IsManualDensity)
        {
            if (string.IsNullOrWhiteSpace(ManualDensityText))
            {
                Shell.Current.DisplayAlertAsync("Missing Relative Density", "Please enter a manual relative density or uncheck the Manual Relative Density checkbox.", "OK");
                return false;
            }

            density = double.Parse(ManualDensityText, CultureInfo.InvariantCulture);
        }
        else
        {
            if (SelectedSubstanceIndex < 0)
            {
                Shell.Current.DisplayAlertAsync("Missing Relative Density", "Please select a substance.", "OK");
                return false;
            }

            density = _substances.ElementAt(SelectedSubstanceIndex).Density;
        }

        return true;
    }

    [RelayCommand]
    private async Task CalculateRectangleAsync()
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(LengthText))
            missing.Add("length");

        if (string.IsNullOrWhiteSpace(WidthText))
            missing.Add("width");

        if (string.IsNullOrWhiteSpace(HeightText))
            missing.Add("height");

        if (missing.Count > 0)
        {
            await Shell.Current.DisplayAlertAsync("Missing Values", $"Please enter the {string.Join(" and ", missing)}.", "OK");
            return;
        }

        if (!double.TryParse(LengthText, out double length) || !double.TryParse(WidthText, out double width) || !double.TryParse(HeightText, out double height))
        {
            await Shell.Current.DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
            return;
        }

        if (!TryGetRelativeDensity(out double density))
            return;

        var answer = _calculationService.CalculateRectangle(length, width, height, density);

        RectangleAnswer = $"{answer:0.000} KG";
    }

    [RelayCommand]
    private async Task CalculateCylinderAsync()
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(DiameterText))
            missing.Add("diameter");

        if (string.IsNullOrWhiteSpace(CylinderHeightText))
            missing.Add("height");

        if (missing.Count > 0)
        {
            await Shell.Current.DisplayAlertAsync("Missing Values", $"Please enter the {string.Join(" and ", missing)}.", "OK");
            return;
        }

        if (!double.TryParse(DiameterText, out double diameter) || !double.TryParse(CylinderHeightText, out double cylHeight))
        {
            await Shell.Current.DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
            return;
        }

        if (!TryGetRelativeDensity(out double density))
            return;

        var answer = _calculationService.CalculateCyclinder(diameter, cylHeight, density);

        CylinderAnswer = $"{answer:0.000} KG";
    }
}
