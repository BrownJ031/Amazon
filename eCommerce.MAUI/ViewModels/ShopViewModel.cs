using Amazon.Library.Models;
using Amazon.Library.Services;
using eCommerce.Library.DTO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace eCommerce.MAUI.ViewModels
{
    public class ShopViewModel : INotifyPropertyChanged
    {
        private decimal taxRate;
        public decimal TaxRate
        {
            get => taxRate;
            set
            {
                taxRate = value;
                NotifyPropertyChanged();
                CalculateTotalPrice();
            }
        }

        public ShopViewModel()
        {
            InventoryQuery = string.Empty;
            TaxRate = LoadTaxRate();
            if (Carts.Any())
            {
                SelectedCart = Carts.First();
            }
            AddToCartCommand = new Command<ProductViewModel>(AddToCart, CanAddToCart);
            RemoveFromCartCommand = new Command<ProductViewModel>(RemoveFromCart, CanRemoveFromCart);
            CheckoutCommand = new Command(Checkout);
        }

        private string inventoryQuery;
        public string InventoryQuery
        {
            set
            {
                inventoryQuery = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Products));
            }
            get { return inventoryQuery; }
        }

        public List<ProductViewModel> Products
        {
            get
            {
                return InventoryServiceProxy.Current.Products
                    .Where(p => p != null)
                    .Where(p => p.Quantity > 0)
                    .Where(p => p?.Name?.ToUpper()?.Contains(InventoryQuery.ToUpper()) ?? false)
                    .Select(p => new ProductViewModel(p)).ToList()
                    ?? new List<ProductViewModel>();
            }
        }

        private ShoppingCart? selectedCart;

        public ShoppingCart? SelectedCart
        {
            get
            {
                return selectedCart;
            }

            set
            {
                selectedCart = value;
                NotifyPropertyChanged(nameof(ProductsInCart));
                CalculateTotalPrice();
            }
        }

        public ObservableCollection<ShoppingCart> Carts
        {
            get
            {
                return new ObservableCollection<ShoppingCart>(ShoppingCartServiceProxy.Current.Carts);
            }
        }

        public List<ProductViewModel> ProductsInCart
        {
            get
            {
                return SelectedCart?.Contents?.Where(p => p != null)
                    .Select(p => new ProductViewModel(p)).ToList()
                    ?? new List<ProductViewModel>();
            }
        }

        private ProductViewModel? productToBuy;
        public ProductViewModel? ProductToBuy
        {
            get => productToBuy;

            set
            {
                productToBuy = value;

                if (productToBuy != null && productToBuy.Model == null)
                {
                    productToBuy.Model = new ProductDTO();
                }
                else if (productToBuy != null && productToBuy.Model != null)
                {
                    productToBuy.Model = new ProductDTO(productToBuy.Model);
                }

                NotifyPropertyChanged();
            }
        }

        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand CheckoutCommand { get; }

        private int totalItems;
        public int TotalItems
        {
            get => totalItems;
            set
            {
                totalItems = value;
                NotifyPropertyChanged();
            }
        }

        private decimal totalPrice;
        public decimal TotalPrice
        {
            get => totalPrice;
            set
            {
                totalPrice = value;
                NotifyPropertyChanged();
            }
        }

        private async void AddToCart(ProductViewModel product)
        {
            if (product?.Model == null || SelectedCart == null)
            {
                Console.WriteLine("Product or SelectedCart is null.");
                return;
            }

            // Add only one unit to the cart
            product.Model.Quantity = 1;
            await Task.Delay(100); // Adding a slight delay to prevent rapid clicks
            ShoppingCartServiceProxy.Current.AddToCart(product.Model, SelectedCart.Id);

            NotifyPropertyChanged(nameof(ProductsInCart));
            NotifyPropertyChanged(nameof(Products));
            CalculateTotalPrice();
            ((Command)AddToCartCommand).ChangeCanExecute(); // Update CanExecute state
        }

        private bool CanAddToCart(ProductViewModel product)
        {
            return product != null && product.Model != null && product.Model.Quantity > 0;
        }

        private void RemoveFromCart(ProductViewModel product)
        {
            if (product?.Model == null || SelectedCart == null)
            {
                Console.WriteLine("Product or SelectedCart is null.");
                return;
            }

            ShoppingCartServiceProxy.Current.RemoveFromCart(product.Model, SelectedCart.Id);

            NotifyPropertyChanged(nameof(ProductsInCart));
            NotifyPropertyChanged(nameof(Products));
            CalculateTotalPrice();
            ((Command)RemoveFromCartCommand).ChangeCanExecute(); // Update CanExecute state
        }

        private bool CanRemoveFromCart(ProductViewModel product)
        {
            return product != null && product.Model != null && product.Model.Quantity > 0;
        }

        private void CalculateTotalPrice()
        {
            if (SelectedCart == null) return;

            TotalItems = SelectedCart.Contents.Sum(p => p.Quantity);
            decimal subtotal = 0;

            foreach (var product in SelectedCart.Contents)
            {
                decimal effectivePrice = product.Price - product.Markdown;
                if (product.IsBogo)
                {
                    int paidQuantity = (product.Quantity / 2) + (product.Quantity % 2);
                    subtotal += paidQuantity * effectivePrice;
                }
                else
                {
                    subtotal += product.Quantity * effectivePrice;
                }
            }

            TotalPrice = subtotal * (1 + TaxRate / 100);
        }

        private void Checkout()
        {
            if (SelectedCart == null) return;

            CalculateTotalPrice();

            // Display a message or navigate to a new page to show the summary
            App.Current.MainPage.DisplayAlert("Checkout Summary", $"Total Items: {TotalItems}\nTotal Price (with Tax): {TotalPrice:C}", "OK");
        }

        public void Refresh()
        {
            InventoryQuery = string.Empty;
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(Carts));
            CalculateTotalPrice();
        }

        public void Search()
        {
            NotifyPropertyChanged(nameof(Products));
        }

        public void PlaceInCart()
        {
            if (ProductToBuy?.Model == null || SelectedCart == null)
            {
                Console.WriteLine("ProductToBuy or SelectedCart is null.");
                return;
            }

            ProductToBuy.Model.Quantity = 1;
            ShoppingCartServiceProxy.Current.AddToCart(ProductToBuy.Model, SelectedCart.Id);

            ProductToBuy = null;
            NotifyPropertyChanged(nameof(ProductsInCart));
            NotifyPropertyChanged(nameof(Products));
            CalculateTotalPrice();
        }

        private decimal LoadTaxRate()
        {
            // Load the tax rate from storage
            string taxRateString = Preferences.Get("TaxRate", "0");
            return decimal.TryParse(taxRateString, out decimal rate) ? rate : 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
