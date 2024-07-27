using Amazon.Library.Services;
using eCommerce.Library.DTO;
using System;

namespace eCommerce.MAUI.ViewModels
{
    public class ProductViewModel
    {
        public ProductDTO? Model { get; set; }

        public string DisplayPrice => Model == null ? string.Empty : $"{Model.Price:C}";
        public string DisplayMarkdownPrice => Model == null ? string.Empty : $"{(Model.Price - Model.Markdown):C}";

        public string PriceAsString
        {
            set
            {
                if (Model == null) return;

                if (decimal.TryParse(value, out var price))
                {
                    Model.Price = price;
                }
            }
        }

        public decimal Markdown
        {
            get => Model?.Markdown ?? 0;
            set
            {
                if (Model != null)
                {
                    Model.Markdown = value;
                }
            }
        }

        public bool IsBogo
        {
            get => Model?.IsBogo ?? false;
            set
            {
                if (Model != null)
                {
                    Model.IsBogo = value;
                }
            }
        }

        public ProductViewModel(int productId = 0)
        {
            if (productId == 0)
            {
                Model = new ProductDTO();
            }
            else
            {
                Model = InventoryServiceProxy.Current.Products.FirstOrDefault(p => p.Id == productId) ?? new ProductDTO();
            }
        }

        public ProductViewModel(ProductDTO? model)
        {
            Model = model ?? new ProductDTO();
        }

        public async void Add()
        {
            if (Model != null)
            {
                Model = await InventoryServiceProxy.Current.AddOrUpdate(Model);
            }
        }
    }
}
