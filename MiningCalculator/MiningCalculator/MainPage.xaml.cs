using System.Globalization;
using System.Reflection;

namespace MiningCalculator;

public partial class MainPage : ContentPage
{
    private readonly Dictionary<string, double> _densityLookup = new()
    {
        { "0.730 - Ammonium Nitrate", 0.730 },
        { "0.890 - Fuel Oil (Medium Weight)", 0.890 },
        { "0.998 - Water at 4 C", 0.998 },
        { "1.000 - Water at 20 C", 1.000 },
        { "1.310 - Sludge", 1.310 },
        { "1.506 - Cement : Portland", 1.506 },
        { "1.520 - Gravel : Loose, Dry", 1.520 },
        { "1.600 - Crushed Stone/Rock", 1.600 },
        { "1.682 - Gravel : Dry", 1.682 },
        { "1.730 - Mud : Fluid", 1.730 },
        { "1.906 - Mud : Packed", 1.906 },
        { "1.920 - Gravel : With sand, natural", 1.920 },
        { "1.922 - Brick : Common Red", 1.922 },
        { "2.002 - Gravel : Wet", 2.002 },
        { "2.180 - Rock Salt", 2.180 },
        { "2.400 - Concrete : Gravel", 2.400 },
        { "2.780 - Rock/Stone : South Africa", 2.780 },
        { "7.840 - Carbon Steel", 7.840 }
    };

    public MainPage()
    {
        InitializeComponent();

        foreach (var item in _densityLookup)
            pickerRelativeDensity.Items.Add(item.Key);

        pickerRelativeDensity.SelectedIndex = 0;

        var version = Assembly.GetExecutingAssembly().GetName().Version;

        lblVersion.Text = $"Version {version}";
    }

    private bool TryGetRelativeDensity(out double density)
    {
        density = 0;

        if (chkManualDensity.IsChecked)
        {
            if (string.IsNullOrWhiteSpace(entryManualDensity.Text))
            {
                DisplayAlertAsync("Missing Relative Density", "Please enter a manual relative density or uncheck the Manual Relative Density checkbox.", "OK");
                return false;
            }

            density = double.Parse(entryManualDensity.Text, CultureInfo.InvariantCulture);
        }
        else
        {
            if (pickerRelativeDensity.SelectedIndex < 0)
            {
                DisplayAlertAsync("Missing Relative Density", "Please select a substance.", "OK");
                return false;
            }

            density = _densityLookup.ElementAt(pickerRelativeDensity.SelectedIndex).Value;
        }

        return true;
    }

    private void RadioShape_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (radioRectangle == null || radioCylinder == null) 
            return;

        frameRectangle.IsVisible = radioRectangle.IsChecked;
        frameCylinder.IsVisible = radioCylinder.IsChecked;
    }

    private void ChkManualDensity_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        entryManualDensity.IsEnabled = chkManualDensity.IsChecked;
        pickerRelativeDensity.IsEnabled = !chkManualDensity.IsChecked;

        if (!chkManualDensity.IsChecked)
            entryManualDensity.Text = string.Empty;
    }

    private async void BtnCalculateRectangle_Clicked(object? sender, EventArgs e)
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(entryLength.Text)) 
            missing.Add("length");

        if (string.IsNullOrWhiteSpace(entryWidth.Text)) 
            missing.Add("width");

        if (string.IsNullOrWhiteSpace(entryHeight.Text)) 
            missing.Add("height");

        if (missing.Count > 0)
        {
            await DisplayAlertAsync("Missing Values", $"Please enter the {string.Join(" and ", missing)}.", "OK");

            return;
        }

        if (!double.TryParse(entryLength.Text, out double length) || !double.TryParse(entryWidth.Text, out double width) || !double.TryParse(entryHeight.Text, out double height))
        {
            await DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");

            return;
        }

        if (!TryGetRelativeDensity(out double density))
            return;

        var answer = length * width * height * density * 1000;
        answer = Math.Round(answer, 3);

        //var answer = CalculationService.CalculateRectangle(length, width, height, density);

        lblRectangleAnswer.Text = $"{answer:0.000} KG";
    }

    private async void BtnCalculateCylinder_Clicked(object? sender, EventArgs e)
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(entryDiameter.Text)) 
            missing.Add("diameter");

        if (string.IsNullOrWhiteSpace(entryCylinderHeight.Text)) 
            missing.Add("height");

        if (missing.Count > 0)
        {
            await DisplayAlertAsync("Missing Values", $"Please enter the {string.Join(" and ", missing)}.", "OK");

            return;
        }

        if (!double.TryParse(entryDiameter.Text, out double diameter) || !double.TryParse(entryCylinderHeight.Text, out double cylHeight))
        {
            await DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");

            return;
        }

        if (!TryGetRelativeDensity(out double density))
            return;

        var radius = diameter / 2;

        var answer = Math.PI * Math.Pow(radius, 2) * cylHeight * density * 1000;

        answer = Math.Round(answer, 3);

        lblCylinderAnswer.Text = $"{answer:0.000} KG";
    }
}