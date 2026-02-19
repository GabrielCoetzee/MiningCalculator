using MiningCalculator.ViewModels;

namespace MiningCalculator;

public partial class AboutPage : ContentPage
{
    public AboutPage(AboutPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}