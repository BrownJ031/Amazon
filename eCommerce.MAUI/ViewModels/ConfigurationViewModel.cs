using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace eCommerce.MAUI.ViewModels
{
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        private decimal taxRate;
        public decimal TaxRate
        {
            get => taxRate;
            set
            {
                taxRate = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        public ConfigurationViewModel()
        {
            // Load existing tax rate
            TaxRate = LoadTaxRate();
            SaveCommand = new Command(SaveTaxRate);
        }

        private decimal LoadTaxRate()
        {
            // Load the tax rate from storage
            string taxRateString = Preferences.Get("TaxRate", "0");
            return decimal.TryParse(taxRateString, out decimal rate) ? rate : 0;
        }

        private async void SaveTaxRate()
        {
            // Save the tax rate to storage
            Preferences.Set("TaxRate", TaxRate.ToString());

            // Notify other parts of the application about the change
            MessagingCenter.Send(this, "TaxRateChanged", TaxRate);

            // Navigate back to ShopView
            await Shell.Current.GoToAsync("//Shop");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
