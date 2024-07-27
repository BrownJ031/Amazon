using Amazon.Library.Utilities;
using eCommerce.Library.DTO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Amazon.Library.Services
{
    public class InventoryServiceProxy
    {
        private static InventoryServiceProxy? instance;
        private static object instanceLock = new object();

        private List<ProductDTO> products = new List<ProductDTO>();

        public ReadOnlyCollection<ProductDTO> Products
        {
            get
            {
                return products.AsReadOnly();
            }
        }

        public async Task<IEnumerable<ProductDTO>> Get()
        {
            var result = await new WebRequestHandler().Get("/api/Inventory");
            if (string.IsNullOrEmpty(result))
            {
                Console.WriteLine("Warning: Received null or empty response from /Inventory endpoint.");
                products = new List<ProductDTO>();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<ProductDTO>>(result) ?? new List<ProductDTO>();
            }
            return products;
        }

        public async Task<ProductDTO> AddOrUpdate(ProductDTO p)
        {
            var result = await new WebRequestHandler().Post("/api/Inventory", p);
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception("Error: Received null or empty response from AddOrUpdate endpoint.");
            }
            return JsonConvert.DeserializeObject<ProductDTO>(result);
        }

        public async Task<ProductDTO?> Delete(int id)
        {
            var response = await new WebRequestHandler().Delete($"/api/Inventory/{id}");
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine($"Warning: Received null or empty response for delete request of ID {id}.");
                return null;
            }
            return JsonConvert.DeserializeObject<ProductDTO>(response);
        }

        public async Task<IEnumerable<ProductDTO>> Search(Query? query)
        {
            if (query == null || string.IsNullOrEmpty(query.QueryString))
            {
                return await Get();
            }

            var result = await new WebRequestHandler().Post("/api/Inventory/Search", query);
            if (string.IsNullOrEmpty(result))
            {
                Console.WriteLine("Warning: Received null or empty response from /Inventory/Search endpoint.");
                products = new List<ProductDTO>();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<ProductDTO>>(result) ?? new List<ProductDTO>();
            }
            return Products;
        }

        public void UpdateProduct(ProductDTO product)
        {
            var existingProduct = products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
            }
        }

        private InventoryServiceProxy()
        {
            products = new List<ProductDTO>();
            try
            {
                var response = new WebRequestHandler().Get("/api/Inventory").Result;
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Warning: Received null or empty response from /Inventory endpoint during initialization.");
                    products = new List<ProductDTO>();
                }
                else
                {
                    products = JsonConvert.DeserializeObject<List<ProductDTO>>(response) ?? new List<ProductDTO>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during initialization: {e.Message}");
                products = new List<ProductDTO>();
            }
        }

        public static InventoryServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new InventoryServiceProxy();
                    }
                }
                return instance;
            }
        }
    }
}
