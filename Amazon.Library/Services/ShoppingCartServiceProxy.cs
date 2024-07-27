using Amazon.Library.Models;
using eCommerce.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amazon.Library.Services
{
    public class ShoppingCartServiceProxy
    {
        private static ShoppingCartServiceProxy? instance;
        private static readonly object instanceLock = new object();

        private readonly List<ShoppingCart> carts;
        public List<ShoppingCart> Carts => carts;

        private ShoppingCartServiceProxy()
        {
            carts = new List<ShoppingCart> { new ShoppingCart { Id = 1, Name = "My Cart", Contents = new List<ProductDTO>() } };
        }

        public static ShoppingCartServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    instance ??= new ShoppingCartServiceProxy();
                }
                return instance;
            }
        }

        public ShoppingCart AddCart(ShoppingCart cart)
        {
            if (cart.Id == 0)
            {
                cart.Id = carts.Select(c => c.Id).Max() + 1;
            }

            carts.Add(cart);
            return cart;
        }

        public void AddToCart(ProductDTO newProduct, int cartId)
        {
            try
            {
                var cartToUse = Carts.FirstOrDefault(c => c.Id == cartId);
                if (cartToUse == null || cartToUse.Contents == null)
                {
                    Console.WriteLine("Cart not found or is null.");
                    return;
                }

                var existingProduct = cartToUse.Contents.FirstOrDefault(existingProducts => existingProducts.Id == newProduct.Id);
                var inventoryProduct = InventoryServiceProxy.Current.Products.FirstOrDefault(invProd => invProd.Id == newProduct.Id);

                if (inventoryProduct == null || inventoryProduct.Quantity < newProduct.Quantity)
                {
                    Console.WriteLine("Insufficient stock or product not found in inventory.");
                    return;
                }

                inventoryProduct.Quantity -= newProduct.Quantity;

                if (existingProduct != null)
                {
                    existingProduct.Quantity += newProduct.Quantity;
                }
                else
                {
                    cartToUse.Contents.Add(newProduct);
                }

                // Log the updated quantities
                Console.WriteLine($"Product {newProduct.Id} added to cart. Inventory quantity after update: {inventoryProduct.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddToCart: {ex.Message}");
            }
        }

        public void RemoveFromCart(ProductDTO product, int cartId)
        {
            try
            {
                var cartToUse = Carts.FirstOrDefault(c => c.Id == cartId);
                if (cartToUse == null || cartToUse.Contents == null)
                {
                    Console.WriteLine("Cart not found or is null.");
                    return;
                }

                var existingProduct = cartToUse.Contents.FirstOrDefault(existingProducts => existingProducts.Id == product.Id);
                var inventoryProduct = InventoryServiceProxy.Current.Products.FirstOrDefault(invProd => invProd.Id == product.Id);

                if (existingProduct == null || inventoryProduct == null)
                {
                    Console.WriteLine("Product not found in cart or inventory.");
                    return;
                }

                existingProduct.Quantity--;
                inventoryProduct.Quantity++;

                if (existingProduct.Quantity <= 0)
                {
                    cartToUse.Contents.Remove(existingProduct);
                }

                // Log the updated quantities
                Console.WriteLine($"Product {product.Id} removed from cart. Inventory quantity after update: {inventoryProduct.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RemoveFromCart: {ex.Message}");
            }
        }
    }
}
