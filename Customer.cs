using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace _06_Angebote
{
    public class Customer : INotifyPropertyChanged
    {
        //direkt instanziiert und initialisiert, dadurch nicht null, kann direkt verwendet werden ohne Prüfung auf null
        //public static ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();
        private static ObservableCollection<Customer> customers = new ObservableCollection<Customer>();

        private const string CustomersFilePath = "customers.json";

        private string firstName;
        private string lastName;
        private int age;
        private string email;

        public string FirstName
        {
            get { return firstName; } //wenn keine weitere Logik nötig, dann reicht auch einfach get => firstName
            set
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName)); //wenn ich untern [CallerMemberName] verwende dann reicht auch nur OnPropertyChanged(); bzw. ohne nameof
                //SaveCustomers(); //wenn ich das hinschreibe und Programm neustarte ohne vorher zu schließen dann sind alle Daten weg
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public int Age
        {
            get { return age; }
            set
            {
                age = value;
                OnPropertyChanged(nameof(Age));
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public Customer(string firstName, string lastName, int age, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Email = email;
        }

        public static void LoadCustomers()
        {
            if (File.Exists(CustomersFilePath))
            {
                string json = File.ReadAllText(CustomersFilePath);

                Customers = JsonConvert.DeserializeObject<ObservableCollection<Customer>>(json) ?? new ObservableCollection<Customer>();
                //?? = Null-Koaleszenz-Operator, wenn Wert null ist dann alternative Wertzuweisung bzw hier auch die Collection
            }
            else
            {
                Customers = new ObservableCollection<Customer>();
            }
            //binde CollectionChanged-Event
            //Customers.CollectionChanged += Customers_CollectionChanged;

            //binde PropertyChanged-Event für existierende Kunden
            //foreach (var customer in Customers)
            //{
            //    customer.PropertyChanged += Customer_PropertyChanged;
            //}
        }

        public static void SaveCustomers()
        {
            string json = JsonConvert.SerializeObject(Customers);
            File.WriteAllText(CustomersFilePath, json);
        }

        public static ObservableCollection<Customer> Customers
        {
            get { return customers; }
            set
            {
                if (customers != value)
                {
                    if (customers != null)
                    {
                        customers.CollectionChanged -= Customers_CollectionChanged;
                        foreach (var customer in customers)
                        {
                            customer.PropertyChanged -= Customer_PropertyChanged;
                        }
                    }
                    customers = value;
                    if (customers != null)
                    {
                        customers.CollectionChanged += Customers_CollectionChanged;
                        foreach (var customer in customers)
                        {
                            customer.PropertyChanged += Customer_PropertyChanged;
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                                                                      //alternativ um oben nameof zu sparen
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //SaveCustomers(); //wenn ich noch in der Zeile drin bin und dann neustarte dann sind alle Daten weg -.-
        }

        private static void Customers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Customer newCustomer in e.NewItems)
                {
                    newCustomer.PropertyChanged += Customer_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (Customer oldCustomer in e.OldItems)
                {
                    oldCustomer.PropertyChanged -= Customer_PropertyChanged; //Ereignis wieder entfernen sonst bleibt im Speicher und
                                                                             //führt zu Speicherlecks
                }
            }
            SaveCustomers();
        }

        private static void Customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var customer = sender as Customer;
            //if (customer != null)
            //{
            //    Console.WriteLine($"Eigenschaft {e.PropertyName} von {customer.FirstName} {customer.LastName} geändert.");
            //}

            SaveCustomers();
        }

    }
}