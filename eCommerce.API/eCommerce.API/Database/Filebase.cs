﻿using Amazon.Library.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eCommerce.API.Database
{
    public class Filebase
    {
        private string _root;
        private static Filebase _instance;

        public static Filebase Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Filebase();
                }

                return _instance;
            }
        }

        private Filebase()
        {
            _root = @"C:\temp\Products";
        }

        public Product AddOrUpdate(Product p)
        {
            if (p.Id <= 0)
            {
                p.Id = NextProductId;
            }

            string path = $"{_root}\\{p.Id}.json";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(p));
            return p;
        }

        public List<Product> Products
        {
            get
            {
                var root = new DirectoryInfo(_root);
                var prods = new List<Product>();
                foreach (var appFile in root.GetFiles())
                {
                    var prod = JsonConvert.DeserializeObject<Product>(File.ReadAllText(appFile.FullName));
                    if (prod != null)
                    {
                        prods.Add(prod);
                    }
                }
                return prods;
            }
        }

        public Product Delete(int id)
        {
            string path = $"{_root}\\{id}.json";
            if (File.Exists(path))
            {
                var product = JsonConvert.DeserializeObject<Product>(File.ReadAllText(path));
                File.Delete(path);
                return product;
            }
            throw new FileNotFoundException($"Product with ID {id} not found");
        }

        public int NextProductId
        {
            get
            {
                if (!Products.Any())
                {
                    return 1;
                }

                return Products.Select(p => p.Id).Max() + 1;
            }
        }
    }
}

