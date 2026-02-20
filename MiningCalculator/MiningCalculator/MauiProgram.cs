using Flurl.Http;
using Microsoft.Extensions.Configuration;
using MiningCalculator.Configuration;
using MiningCalculator.Services.Abstract;
using MiningCalculator.Services.Concrete;
using MiningCalculator.ViewModels;
using System.Reflection;
using System.Text.Json;

namespace MiningCalculator;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        //var assembly = Assembly.GetExecutingAssembly();

        //using var stream = assembly.GetManifestResourceStream("MiningCalculator.appsettings.json");

        var config = new ConfigurationBuilder()
            //.AddJsonStream(stream!)
            .Build();

        builder.Configuration.AddConfiguration(config);

        RegisterSettings(builder);

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        AddDependencies(builder);

        return builder.Build();
    }

    private static void RegisterSettings(MauiAppBuilder builder)
    {
        var appSettingsUrl = "https://raw.githubusercontent.com/GabrielCoetzee/MiningCalculator/refs/heads/main/MiningCalculator/MiningCalculator/appsettings.json";

        var json = appSettingsUrl.GetStringAsync().Result;

        var doc = JsonDocument.Parse(json);

        var substancesSettings = doc.RootElement.GetProperty("SubstancesSettings").Deserialize<SubstancesSettings>();

        builder.Services.Configure<SubstancesSettings>(options =>
        {
            options.Substances = substancesSettings.Substances;
        });
    }

    private static void AddDependencies(MauiAppBuilder builder)
    {
        builder.Services.AddTransient<IMaterialMassCalculationService, MaterialMassCalculationService>();
        builder.Services.AddTransient<IGoldCalculationService, GoldCalculationService>();

        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<GoldPageViewModel>();
        builder.Services.AddTransient<AboutPageViewModel>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<GoldPage>();
        builder.Services.AddTransient<AboutPage>();
    }
}