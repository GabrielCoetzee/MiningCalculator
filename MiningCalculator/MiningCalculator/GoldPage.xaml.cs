using MiningCalculator.ViewModels;

namespace MiningCalculator;

public partial class GoldPage : ContentPage
{
    public GoldPage(GoldPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}