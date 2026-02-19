using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiningCalculator.Services.Abstract;

namespace MiningCalculator.ViewModels;

public partial class GoldPageViewModel : ObservableObject
{
    private readonly IGoldCalculationService _goldCalculationService;

    private double _gPerTonValue;
    private double _tonsValue;
    private double _stopingWidthValue;

    [ObservableProperty]
    private string _cmgtText = string.Empty;

    [ObservableProperty]
    private string _stopingWidthText = string.Empty;

    [ObservableProperty]
    private bool _isStopingWidthCm;

    [ObservableProperty]
    private string _stopingWidthUnit = "meter(s)";

    [ObservableProperty]
    private string _answerGPerTon = string.Empty;

    [ObservableProperty]
    private bool _calcSquareMeters;

    [ObservableProperty]
    private bool _isFaceLengthEnabled;

    [ObservableProperty]
    private bool _isFaceAdvanceEnabled;

    [ObservableProperty]
    private bool _isSquareMetresEnabled = true;

    [ObservableProperty]
    private string _faceLengthText = string.Empty;

    [ObservableProperty]
    private string _faceAdvanceText = string.Empty;

    [ObservableProperty]
    private string _squareMetresText = string.Empty;

    [ObservableProperty]
    private string _answerTons = string.Empty;

    [ObservableProperty]
    private string _answerGold = string.Empty;

    public GoldPageViewModel(IGoldCalculationService goldCalculationService)
    {
        _goldCalculationService = goldCalculationService;
    }

    partial void OnIsStopingWidthCmChanged(bool value)
    {
        StopingWidthUnit = value ? "centimeter(s)" : "meter(s)";
    }

    partial void OnCalcSquareMetersChanged(bool value)
    {
        IsFaceLengthEnabled = value;
        IsFaceAdvanceEnabled = value;
        IsSquareMetresEnabled = !value;
    }

    [RelayCommand]
    private async Task CalculateGPerTonAsync()
    {
        if (string.IsNullOrWhiteSpace(CmgtText) && string.IsNullOrWhiteSpace(StopingWidthText))
        {
            await Shell.Current.DisplayAlertAsync("Missing Values", "Please enter the CMGT and Stoping Width values.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(CmgtText))
        {
            await Shell.Current.DisplayAlertAsync("Missing CMGT", "Please enter the CMGT value.", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(StopingWidthText))
        {
            await Shell.Current.DisplayAlertAsync("Missing Stoping Width", "Please enter the Stoping Width.", "OK");
            return;
        }

        if (!double.TryParse(CmgtText, out double cmgt) || !double.TryParse(StopingWidthText, out double sw))
        {
            await Shell.Current.DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
            return;
        }

        _stopingWidthValue = sw;
        _gPerTonValue = _goldCalculationService.CalculateGPerTon(cmgt, _stopingWidthValue, IsStopingWidthCm);

        AnswerGPerTon = $"{_gPerTonValue:0.000} G/Ton";
    }

    [RelayCommand]
    private async Task CalculateTonsAsync()
    {
        if (string.IsNullOrWhiteSpace(AnswerGPerTon))
        {
            await Shell.Current.DisplayAlertAsync("Missing Values", "Please finish the G/Ton calculation above before moving forward.", "OK");
            return;
        }

        double squaredMetres;

        if (CalcSquareMeters)
        {
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(FaceLengthText))
                missing.Add("length");

            if (string.IsNullOrWhiteSpace(FaceAdvanceText))
                missing.Add("width");

            if (missing.Count > 0)
            {
                await Shell.Current.DisplayAlertAsync("Missing Values", $"Please enter the face {string.Join(" and ", missing)} or uncheck Calculate Square Meters.", "OK");
                return;
            }

            if (!double.TryParse(FaceLengthText, out double len) || !double.TryParse(FaceAdvanceText, out double wid))
            {
                await Shell.Current.DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
                return;
            }

            squaredMetres = _goldCalculationService.CalculateSquareMeters(len, wid);

            SquareMetresText = squaredMetres.ToString("0.000");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(SquareMetresText))
            {
                await Shell.Current.DisplayAlertAsync("Missing Values", "Please enter the Square Metres.", "OK");
                return;
            }

            if (!double.TryParse(SquareMetresText, out squaredMetres))
            {
                await Shell.Current.DisplayAlertAsync("Characters found", "Please use numeric values only.", "OK");
                return;
            }
        }

        _tonsValue = Math.Round(squaredMetres * (_stopingWidthValue / 100) * 2.78, 3);

        AnswerTons = $"{_tonsValue:0.000} tons";
    }

    [RelayCommand]
    private async Task CalculateGoldAsync()
    {
        if (string.IsNullOrWhiteSpace(AnswerGPerTon) || string.IsNullOrWhiteSpace(AnswerTons))
        {
            await Shell.Current.DisplayAlertAsync("Missing Values", "Please finish the above calculations before moving forward.", "OK");
            return;
        }

        var answer = _goldCalculationService.CalculateGold(_tonsValue, _gPerTonValue);

        AnswerGold = $"{answer:0.000} KG";
    }

    [RelayCommand]
    private void Clear()
    {
        CmgtText = string.Empty;
        StopingWidthText = string.Empty;
        AnswerGPerTon = string.Empty;
        FaceLengthText = string.Empty;
        FaceAdvanceText = string.Empty;
        SquareMetresText = string.Empty;
        AnswerTons = string.Empty;
        AnswerGold = string.Empty;
        _gPerTonValue = 0;
        _tonsValue = 0;
        _stopingWidthValue = 0;
    }
}
