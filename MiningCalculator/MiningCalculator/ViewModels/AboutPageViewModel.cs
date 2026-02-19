using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace MiningCalculator.ViewModels;

public partial class AboutPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _productName;

    [ObservableProperty]
    private string _version;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _copyright;

    public AboutPageViewModel()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetName();

        ProductName = "Mining Calculator";
        Version = $"Version {name.Version}";
        Description = "Vibe-Coded by André Coetzee and Claude Opus 4.6";
        Copyright = "Icons used from freepik.com";
    }
}
