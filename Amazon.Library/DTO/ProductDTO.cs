using Amazon.Library.Models;

namespace eCommerce.Library.DTO
{
    public class ProductDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Id { get; set; }
        public bool IsBogo { get; set; }
        public int Quantity { get; set; }
        public decimal Markdown { get; set; } // Add this line

        public ProductDTO(Product p)
        {
            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            Id = p.Id;
            Quantity = p.Quantity;
            IsBogo = p.IsBogo;
            Markdown = p.Markdown; // Ensure this line maps the property
        }

        public ProductDTO(ProductDTO p)
        {
            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            Id = p.Id;
            Quantity = p.Quantity;
            IsBogo = p.IsBogo;
            Markdown = p.Markdown; // Ensure this line maps the property
        }

        public ProductDTO() { }
    }
}
