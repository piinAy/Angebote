using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace _06_Angebote
{
    public class Product : INotifyPropertyChanged
    {
        //public static ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        private static ObservableCollection<Product> products = new ObservableCollection<Product>();

        private const string ProductsFilePath = "products.json";

        private string name;
        private string description;
        private decimal price;
        private int ageMin;
        private int ageMax;

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => price;
            set { price = value; OnPropertyChanged(); }
        }

        public int AgeMin
        {
            get => ageMin;
            set { ageMin = value; OnPropertyChanged(); }
        }

        public int AgeMax
        {
            get => ageMax;
            set { ageMax = value; OnPropertyChanged(); }
        }

        public Product(string name, string description, decimal price, int ageMin, int ageMax)
        {
            Name = name;
            Description = description;
            Price = price;
            AgeMin = ageMin;
            AgeMax = ageMax;
        }

        public static void LoadProducts()
        {
            if (File.Exists(ProductsFilePath))
            {
                string json = File.ReadAllText(ProductsFilePath);
                //try
                //{
                Products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json) ?? new ObservableCollection<Product>();
                //}
                //catch (Exception)
                //{
                //    MessageBox.Show("Keine Produkte vorhanden!");
                //    Products = new ObservableCollection<Product>();
                //}
            }
            else
            {
                Products = new ObservableCollection<Product>();
            }
        }

        //public static void LoadProducts()
        //{
        //    if (File.Exists(ProductsFilePath))
        //    {
        //        string json = File.ReadAllText(ProductsFilePath);
        //        Products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);
        //    }
        //    //else
        //    //{
        //    //    Products = new ObservableCollection<Product>();
        //    //}
        //}

        public static void SaveProducts()
        {
            string json = JsonConvert.SerializeObject(Products);
            File.WriteAllText(ProductsFilePath, json);
        }

        public static ObservableCollection<Product> Products //damit Änderungen in der Liste aktualisiert werden
        {
            get { return products; }
            set
            {
                if (products != value)
                {
                    if (products != null)
                    {
                        products.CollectionChanged -= Products_CollectionChanged;
                    }
                    products = value;
                    if (products != null)
                    {
                        products.CollectionChanged += Products_CollectionChanged;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Product newProduct in e.NewItems)
                {
                    newProduct.PropertyChanged += Product_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (Product oldProduct in e.OldItems)
                {
                    oldProduct.PropertyChanged -= Product_PropertyChanged;
                }
            }

            SaveProducts();
        }

        private static void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var product = sender as Product;

            SaveProducts();
        }
    }
}

//public ObservableCollection<Product> Products
//{
//    get { return products; }
//    set
//    {
//        if (products != value)
//        {
//            products = value;
//            OnPropertyChanged(nameof(Products));
//        }
//    }
//}