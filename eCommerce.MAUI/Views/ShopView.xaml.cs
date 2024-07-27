using eCommerce.MAUI.ViewModels;

namespace eCommerce.MAUI.Views;

public partial class ShopView : ContentPage
{
    public ShopView()
    {
        InitializeComponent();
        var viewModel = new ShopViewModel();
        BindingContext = viewModel;

        MessagingCenter.Subscribe<ConfigurationViewModel, decimal>(this, "TaxRateChanged", (sender, newTaxRate) =>
        {
            viewModel.TaxRate = newTaxRate;
        });
    }

    private void CancelClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (BindingContext as ShopViewModel)?.Refresh();
    }

    private void InventorySearchClicked(object sender, EventArgs e)
    {
        (BindingContext as ShopViewModel)?.Search();
    }

    private void PlaceInCartClicked(object sender, EventArgs e)
    {
        (BindingContext as ShopViewModel)?.PlaceInCart();
    }

    private void AddCartClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Cart");
    }

    private void ConfigureTaxRateClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Configuration");
    }
}
