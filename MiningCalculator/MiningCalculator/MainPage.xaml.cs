using Microsoft.Extensions.Options;
using MiningCalculator.Configuration;
using System.Globalization;
using System.Reflection;

namespace MiningCalculator;

public partial class MainPage : ContentPage
{
    public IEnumerable<Substance> Substances { get; set; }

    public MainPage(IOptions<SubstancesSettings> substancesSettings)
    {
        InitializeComponent();

        Substances = substancesSettings.Value.Substances;

        foreach (var item in Substances)
            pickerRelativeDensity.Items.Add(item.Name);

        pickerRelativeDensity.SelectedIndex = 0;

        lblVersion.Text = $"Version {Assembly.GetExecutingAssembly().GetName().Version}";
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

            density = Substances.Single(x => x.Name.Equals(pickerRelativeDensity.SelectedItem)).Density;
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