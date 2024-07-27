namespace eCommerce.MAUI
{
    public partial class MainPage : ContentPage
    {
        // Removed unused field 'count'

        public MainPage()
        {
            InitializeComponent();
        }

        private void ManageInventoryClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//Inventory");
        }

        private void ShopClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//Shop");
        }
    }
}
