using System.Reflection;

namespace MiningCalculator;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();

        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetName();

        lblProductName.Text = GetAttribute<AssemblyProductAttribute>(assembly)?.Product ?? name.Name ?? "Mining Calculator";
        lblVersion.Text = $"Version {name.Version}";
        lblDescription.Text = GetAttribute<AssemblyDescriptionAttribute>(assembly)?.Description ?? string.Empty;
        lblCopyright.Text = GetAttribute<AssemblyCopyrightAttribute>(assembly)?.Copyright ?? string.Empty;
    }

    private static T? GetAttribute<T>(Assembly assembly) where T : Attribute
    {
        return assembly.GetCustomAttribute<T>();
    }
}