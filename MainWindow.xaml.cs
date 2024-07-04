using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace _06_Angebote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Product> Products => Product.Products; //expression-bodied members??? kp aber ich habe mir dadurch das ständige
                                                                           //schreiben der Klassennamen.blabla erspart, außer bei den Load Methoden ging es nicht
        public ObservableCollection<Customer> Customers => Customer.Customers;

        private ObservableCollection<Customer> recommendedCustomers; //nochmal zwei Collections, die nur die Empfehlungen anzeigen/der Suche entsprechen

        private ObservableCollection<Product> recommendedProducts;


        public MainWindow()
        {
            InitializeComponent();

            Customer.LoadCustomers();
            Product.LoadProducts();
        }

        private bool isPlaceholderActive = true; //damit das Wort "Suchen" selbst nicht grau erscheint wenn man aktiv etwas eingibt

        private void SearchInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isPlaceholderActive)
            {
                SearchInput.Text = "";
                SearchInput.Foreground = Brushes.Black;
                isPlaceholderActive = false;
            }
        }

        private void SearchInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchInput.Text))
            {
                SearchInput.Text = "Suchen";
                SearchInput.Foreground = Brushes.Gray;
                isPlaceholderActive = true;
                ClearDataGrid(); //wenn Eingabe leer ist und ich nicht im Eingabefeld bin, dann aktuelle Ausgabe leeren
            }
        }

        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isPlaceholderActive) //ich rufe nur die Methode auf, wenn ich im Eingabefeld drin bin
            {
                ApplySearchFilter(); //jedes Zeichen welches ich eingebe bzw jede Änderung im Textfeld ruft diese Methode auf
            }
        }

        private void CustomersOrProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isPlaceholderActive)
            {
                ApplySearchFilter();
            }
        }

        private void ApplySearchFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchInput.Text)) //wenn die Eingabe leer ist bzw gelöscht wird, dann auch Ausgabe leeren
            {
                ClearDataGrid();
                //return;
            }
            else
            {
                string searchText = SearchInput.Text.ToLower();
                if (CustomersOrProducts.SelectedIndex == 0) //erster Index d.h. erste Wert in der ComboBox
                {
                    var filteredCustomers = new ObservableCollection<Customer>(Customers.Where(c =>
                        c.FirstName.ToLower().Contains(searchText) ||
                        c.LastName.ToLower().Contains(searchText) ||
                        c.Age.ToString().Contains(searchText) ||
                        c.Email.ToLower().Contains(searchText)).ToList());

                    ClearDataGrid();

                    if (filteredCustomers.Count > 0)
                    {
                        PrepareDataGridForCustomers(filteredCustomers);
                        ShowButtons("Produkte", "Kunden");
                    }
                }
                else if (CustomersOrProducts.SelectedIndex == 1)
                {
                    var filteredProducts = new ObservableCollection<Product>(Products.Where(p =>
                        p.Name.ToLower().Contains(searchText) ||
                        p.Description.ToLower().Contains(searchText) ||
                        p.Price.ToString().Contains(searchText) ||
                        p.AgeMin.ToString().Contains(searchText) ||
                        p.AgeMax.ToString().Contains(searchText)).ToList());

                    ClearDataGrid();

                    if (filteredProducts.Count > 0)
                    {
                        PrepareDataGridForProducts(filteredProducts);
                        ShowButtons("Kunden", "Produkte");
                    }
                }
            }
        }

        private void ShowCustomers_Click(object sender, RoutedEventArgs e)
        {
            OutputDataGrid.SelectedItems.Clear(); //falls ich etwas zuvor ausgewählt hatte, soll das resettet werden
            OutputDataGrid.Items.SortDescriptions.Clear();

            if (Customers.Count == 0)
            {
                ClearDataGrid();
                MessageBox.Show("Es sind keine Kunden vorhanden.");
                return;
            }
            else
            {
                PrepareDataGridForCustomers(Customers);
                ShowButtons("Produkte", "Kunden");
            }
        }

        private void ShowProducts_Click(object sender, RoutedEventArgs e)
        {
            OutputDataGrid.SelectedItems.Clear();
            OutputDataGrid.Items.SortDescriptions.Clear();

            if (Products.Count == 0)
            {
                ClearDataGrid();
                MessageBox.Show("Es sind keine Produkte vorhanden.");
                return;
            }
            else
            {
                PrepareDataGridForProducts(Products);
                ShowButtons("Kunden", "Produkte");
            }
        }

        //ich erstelle die Spalten bzw die Tabelle dynamisch weil ich verschiedene Tabellen habe
        //oder über ContentControl zwei verschiedene Tabellen handhaben? Oder andere Form implementieren?
        private void PrepareDataGridForCustomers(ObservableCollection<Customer> customers)
        {
            OutputDataGrid.ItemsSource = customers;
            OutputDataGrid.Columns.Clear();
            OutputDataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Vorname", Binding = new Binding("FirstName") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Nachname", Binding = new Binding("LastName") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Alter", Binding = new Binding("Age") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "E-Mail", Binding = new Binding("Email") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
        }

        private void PrepareDataGridForProducts(ObservableCollection<Product> products)
        {
            OutputDataGrid.ItemsSource = products;
            OutputDataGrid.Columns.Clear();
            OutputDataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Beschreibung", Binding = new Binding("Description") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Preis", Binding = new Binding("Price") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Mindestalter", Binding = new Binding("AgeMin") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
            OutputDataGrid.Columns.Add(new DataGridTextColumn { Header = "Maximalalter", Binding = new Binding("AgeMax") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged } });
        }

        private void ShowButtons(string zuEmpfehlen, string zuLöschen)
        {
            Recommend.Content = zuEmpfehlen + " empfehlen";
            Delete.Content = zuLöschen + " löschen";
            ButtonsRecommendDelete.Visibility = Visibility.Visible;
        }

        private void CreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            //Kunde nur erstellen wenn alle Felder ausgefüllt
            if (!ValidateInput(FirstNameInput, LastNameInput, AgeInput, EMailInput))
            {
                return;
            }

            var newCustomer = new Customer(FirstNameInput.Text, LastNameInput.Text, int.Parse(AgeInput.Text), EMailInput.Text);
            Customers.Add(newCustomer);

            //Eingabefelder leeren
            ClearInputFields(FirstNameInput, LastNameInput, AgeInput, EMailInput);

            //Anzeigen der empfohlenen Produkte für den neuen Kunden
            RecommendProductsForNewCustomer(newCustomer);
        }

        private void CreateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(NameInput, DescriptionInput, PriceInput, AgeMinInput, AgeMaxInput))
            {
                return;
            }

            var newProduct = new Product(NameInput.Text, DescriptionInput.Text, int.Parse(PriceInput.Text), int.Parse(AgeMinInput.Text), int.Parse(AgeMaxInput.Text));

            Products.Add(newProduct);

            //Felder leeren
            ClearInputFields(NameInput, DescriptionInput, PriceInput, AgeMinInput, AgeMaxInput);

            //Anzeigen der empfohlenen Kunden für das neue Produkt
            RecommendCustomersForNewProduct(newProduct);
        }

        private bool ValidateInput(params TextBox[] fields)
        {
            foreach (var field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.Text))
                {
                    MessageBox.Show("Kein Feld darf leer sein!");
                    field.Focus(); //ich springe zurück zum ersten leeren Feld
                    return false;
                }
            }
            return true;
        }

        private void ClearInputFields(params TextBox[] fields)
        {
            foreach (var field in fields)
            {
                field.Text = string.Empty;
            }
        }

        private void RecommendProductsForNewCustomer(Customer customer)
        {
            var recommendedProducts = Products.Where(p =>
                p.AgeMin <= customer.Age && p.AgeMax >= customer.Age);

            if (!recommendedProducts.Any())
            {
                ClearDataGrid();
                MessageBox.Show("Keine Produkte zum Empfehlen gefunden!");
                return;
            }
            else
            {
                MessageBox.Show("Folgende Produkte sind für diesen Kunden zu empfehlen:"); //vor der Meldung sieht man aktuelle Grid, besser ohne
                PrepareDataGridForProducts(new ObservableCollection<Product>(recommendedProducts)); //ich mache eine neue Liste, die alte wird aber nicht überschrieben
                ShowButtons("Kunden", "Produkte");
            }
        }

        private void RecommendCustomersForNewProduct(Product product)
        {
            var recommendedCustomers = Customers.Where(c =>
                c.Age >= product.AgeMin && c.Age <= product.AgeMax);

            if (!recommendedCustomers.Any())
            {
                ClearDataGrid();
                MessageBox.Show("Keine Kunden zum Empfehlen gefunden!");
                return;
            }
            else
            {
                MessageBox.Show("Folgende Kunden sind für dieses Produkt zu empfehlen:");
                PrepareDataGridForCustomers(new ObservableCollection<Customer>(recommendedCustomers));
                ShowButtons("Produkte", "Kunden");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Test ItemsSource
        //public void dataSourceType()
        //{
        //    if (OutputDataGrid.ItemsSource == Customer.Customers)
        //    {
        //        MessageBox.Show("Kunden");
        //    }
        //    else if (OutputDataGrid.ItemsSource == Product.Products)
        //    {
        //        MessageBox.Show("Produkte");
        //    }
        //    else
        //    {
        //        MessageBox.Show("öjhsdlksghlhgölsfgö!!!!11111!1!!! WARUM GEHT ES NICHT?!?!?");
        //    }
        //}
        private void Recommend_Click(object sender, RoutedEventArgs e)
        {
            //da es alleine mit ItemsSource nicht klappt, nehme ich die Bedingung mit den Buttons dazu
            //da diese immer richtig angezeigt werden und ich mich darauf dann beziehen kann
            if (OutputDataGrid.ItemsSource == Customers || Recommend.Content.ToString() == "Produkte empfehlen")
            {
                if (OutputDataGrid.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Bitte einen Kunden auswählen!");
                }
                else
                {
                    RecommendProducts();
                }
            }
            //hier Produkte oder Kunden empfehlen abhängig vom Alter
            else if (OutputDataGrid.ItemsSource == Products || Recommend.Content.ToString() == "Kunden empfehlen")
            {
                if (OutputDataGrid.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Bitte ein Produkt auswählen!");
                }
                else
                {
                    //Produkte werden angezeigt, daher sollen Kunden empfohlen werden
                    RecommendCustomers();
                }
            }
        }

        private ObservableCollection<Product> RecommendProducts()
        {
            if (OutputDataGrid.SelectedItem is Customer selectedCustomer)
            {
                //var recommendedProducts = Products.Where(p =>
                //    p.AgeMin <= selectedCustomer.Age && p.AgeMax >= selectedCustomer.Age);
                //ich erstelle hier nun die Collection mit den Empfehlungen
                recommendedProducts = new ObservableCollection<Product>(Products.Where(p => p.AgeMin <= selectedCustomer.Age && p.AgeMax >= selectedCustomer.Age));

                if (!recommendedProducts.Any())
                {
                    MessageBox.Show("Keine Produkte zum Empfehlen gefunden!");
                    //return;
                }
                else
                {
                    MessageBox.Show("Folgende Produkte sind für diesen Kunden zu empfehlen:");
                    PrepareDataGridForProducts(recommendedProducts);
                    ShowButtons("Kunden", "Produkte");
                }
            }
            return recommendedProducts;
        }

        private ObservableCollection<Customer> RecommendCustomers()
        {
            if (OutputDataGrid.SelectedItem is Product selectedProduct)
            {
                recommendedCustomers = new ObservableCollection<Customer>(Customers.Where(c => c.Age >= selectedProduct.AgeMin && c.Age <= selectedProduct.AgeMax));

                if (!recommendedCustomers.Any())
                {
                    MessageBox.Show("Keine Kunden zum Empfehlen gefunden!");
                }
                else
                {
                    MessageBox.Show("Folgende Kunden sind für dieses Produkt zu empfehlen:");
                    //PrepareDataGridForCustomers(new ObservableCollection<Customer>(recommendedCustomers));
                    PrepareDataGridForCustomers(recommendedCustomers);
                    ShowButtons("Produkte", "Kunden");
                }

            }

            return recommendedCustomers;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ClearOutput_Click(object sender, RoutedEventArgs e)
        {
            ClearDataGrid();
        }

        private void ClearDataGrid()
        {
            OutputDataGrid.Columns.Clear();
            OutputDataGrid.HeadersVisibility = DataGridHeadersVisibility.None;
            ButtonsRecommendDelete.Visibility = Visibility.Hidden;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //if (OutputDataGrid.ItemsSource == Customers)
            if (Delete.Content.ToString() == "Kunden löschen")
            {
                if (OutputDataGrid.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Bitte mindestens einen Kunden auswählen!");
                    return;
                }

                foreach (Customer selectedCustomer in OutputDataGrid.SelectedItems.Cast<Customer>().ToList())
                {
                    Customers.Remove(selectedCustomer); //damit auch die Hauptliste aktuell ist
                    recommendedCustomers?.Remove(selectedCustomer); //das dient nur für die aktualisierte gefilterte Ansicht
                    //OutputDataGrid.ItemsSource = recommendedCustomers;
                    //OutputDataGrid.ItemsSource = Customers;
                    PrepareDataGridForCustomers(recommendedCustomers ?? Customers);
                }
            }
            //else if (OutputDataGrid.ItemsSource == Products)
            else if (Delete.Content.ToString() == "Produkte löschen")
            {
                if (OutputDataGrid.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Bitte mindestens ein Produkt auswählen!");
                    return;
                }

                foreach (Product selectedProduct in OutputDataGrid.SelectedItems.Cast<Product>().ToList())
                {
                    Products.Remove(selectedProduct);
                    recommendedProducts?.Remove(selectedProduct);
                    //OutputDataGrid.ItemsSource = recommendedProducts;
                    //OutputDataGrid.ItemsSource = Products;
                    PrepareDataGridForProducts(recommendedProducts ?? Products);
                }
            }
        }
    }
}

//validierung direkt bei Eingabe implementieren

//anstatt messagebox den Kunden oder das Produkt auch anzeigen zu den empfohlenen Produkten bzw Kunden