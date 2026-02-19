namespace MiningCalculator;

public partial class GoldPage : ContentPage
{
    private double _answerGPerTon;
    private double _answerTons;
    private double _stopingWidth;

    public GoldPage()
    {
        InitializeComponent();
    }

    private void ChkStopingWidthCm_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        lblStopingWidthUnit.Text = chkStopingWidthCm.IsChecked ? "centimeter(s)" : "meter(s)";
    }

    private void ChkCalcSquareMeters_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        bool calc = chkCalcSquareMeters.IsChecked;
        entryFaceLength.IsEnabled = calc;
        entryFaceAdvance.IsEnabled = calc;
        entrySquareMetres.IsEnabled = !calc;
    }

    private async void BtnCalculateGPerTon_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryCMGT.Text) && string.IsNullOrWhiteSpace(entryStopingWidth.Text))
        {
            await DisplayAlertAsync("Missing Values", "Please enter the CMGT and Stoping Width values.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(entryCMGT.Text))
        {
            await DisplayAlertAsync("Missing CMGT", "Please enter the CMGT value.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(entryStopingWidth.Text))
        {
            await DisplayAlertAsync("Missing Stoping Width", "Please enter the Stoping Width.", "OK");
            return;
        }

        if (!double.TryParse(entryCMGT.Text, out double cmgt) || !double.TryParse(entryStopingWidth.Text, out double sw))
        {
            await DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
            return;
        }

        _stopingWidth = sw;

        if (chkStopingWidthCm.IsChecked)
            _stopingWidth /= 1000;

        double swForCalc = _stopingWidth * 100;
        _answerGPerTon = Math.Round(cmgt / swForCalc, 3);

        lblAnswerGPerTon.Text = $"{_answerGPerTon:0.000} G/Ton";
    }

    private async void BtnCalculateTons_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(lblAnswerGPerTon.Text))
        {
            await DisplayAlertAsync("Missing Values", "Please finish the G/Ton calculation above before moving forward.", "OK");
            return;
        }

        double squaredMetres;

        if (chkCalcSquareMeters.IsChecked)
        {
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(entryFaceLength.Text)) missing.Add("length");
            if (string.IsNullOrWhiteSpace(entryFaceAdvance.Text)) missing.Add("width");

            if (missing.Count > 0)
            {
                await DisplayAlertAsync("Missing Values", $"Please enter the face {string.Join(" and ", missing)} or uncheck Calculate Square Meters.", "OK");
                return;
            }

            if (!double.TryParse(entryFaceLength.Text, out double len) ||
                !double.TryParse(entryFaceAdvance.Text, out double wid))
            {
                await DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
                return;
            }

            squaredMetres = Math.Round(len * wid, 3);
            entrySquareMetres.Text = squaredMetres.ToString("0.000");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(entrySquareMetres.Text))
            {
                await DisplayAlertAsync("Missing Values", "Please enter the Square Metres.", "OK");
                return;
            }

            if (!double.TryParse(entrySquareMetres.Text, out squaredMetres))
            {
                await DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
                return;
            }
        }

        _answerTons = Math.Round(squaredMetres * (_stopingWidth / 100) * 2.78, 3);

        lblAnswerTons.Text = $"{_answerTons:0.000} tons";
    }

    private async void BtnCalculateGold_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(lblAnswerGPerTon.Text) || string.IsNullOrWhiteSpace(lblAnswerTons.Text))
        {
            await DisplayAlertAsync("Missing Values", "Please finish the above calculations before moving forward.", "OK");
            return;
        }

        var answerGold = Math.Round(_answerTons * _answerGPerTon / 1000, 3);

        lblAnswerGold.Text = $"{answerGold:0.000} KG";
    }

    private void BtnClear_Clicked(object? sender, EventArgs e)
    {
        entryCMGT.Text = string.Empty;
        entryStopingWidth.Text = string.Empty;
        lblAnswerGPerTon.Text = string.Empty;
        entryFaceLength.Text = string.Empty;
        entryFaceAdvance.Text = string.Empty;
        entrySquareMetres.Text = string.Empty;
        lblAnswerTons.Text = string.Empty;
        lblAnswerGold.Text = string.Empty;
        _answerGPerTon = 0;
        _answerTons = 0;
        _stopingWidth = 0;
    }
}