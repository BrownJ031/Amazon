using eCommerce.Library.DTO;

namespace Amazon.Library.Models
{
    public class Product
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public bool IsBogo { get; set; }
        public decimal Markdown { get; set; } // Add this line

        public string Property1 { get; set; } = string.Empty;
        public string Property2 { get; set; } = string.Empty;
        public string Property3 { get; set; } = string.Empty;
        public string Property4 { get; set; } = string.Empty;
        public string Property5 { get; set; } = string.Empty;
        public string Property6 { get; set; } = string.Empty;

        public Product()
        {
            Property1 = string.Empty;
            Property2 = string.Empty;
            Property3 = string.Empty;
            Property4 = string.Empty;
            Property5 = string.Empty;
            Property6 = string.Empty;
        }

        public Product(Product p)
        {
            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            Id = p.Id;
            Quantity = p.Quantity;
            IsBogo = p.IsBogo;
            Markdown = p.Markdown; // Add this line

            Property1 = p.Property1 ?? string.Empty;
            Property2 = p.Property2 ?? string.Empty;
            Property3 = p.Property3 ?? string.Empty;
            Property4 = p.Property4 ?? string.Empty;
            Property5 = p.Property5 ?? string.Empty;
            Property6 = p.Property6 ?? string.Empty;
        }

        public Product(ProductDTO d)
        {
            Name = d.Name;
            Description = d.Description;
            Price = d.Price;
            Id = d.Id;
            Quantity = d.Quantity;
            IsBogo = d.IsBogo;
            Markdown = d.Markdown; // Add this line

            Property1 = string.Empty;
            Property2 = string.Empty;
            Property3 = string.Empty;
            Property4 = string.Empty;
            Property5 = string.Empty;
            Property6 = string.Empty;
        }
    }
}
