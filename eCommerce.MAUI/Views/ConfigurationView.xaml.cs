using eCommerce.MAUI.ViewModels;

namespace eCommerce.MAUI.Views;

public partial class ConfigurationView : ContentPage
{
    public ConfigurationView()
    {
        InitializeComponent();
        BindingContext = new ConfigurationViewModel();
    }
}
