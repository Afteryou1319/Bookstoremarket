using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bookstoremarket
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Window
    {
        public ObservableCollection<Customerdata> Customers { get; set; } = new ObservableCollection<Customerdata>();
        private string connectionString = @"Data Source=D:\base\Basestore.db;Version=3;";
        public Customer()
        {
            InitializeComponent();
            CustomersDatabase();
            //Customers.Add(new Customerdata { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "123456789" });
            //Customers.Add(new Customerdata { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "987654321" });

            
            CustomerDataGrid.ItemsSource = Customers;
        }
        private void CustomersDatabase()
        {
            Customers.Clear();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Customer_Id, Customer_Name, Address, Email FROM Customers";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customers.Add(new Customerdata
                            {
                                Customer_Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Customer_Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Address = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                            });
                        }
                    }
                }
            }
        }
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new CustomerDetail();
            if (addWindow.ShowDialog() == true)
            {
                var newCustomer = addWindow.Customer;

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    int newCustomerId = 1; 
                    string getMaxIdQuery = "SELECT MAX(Customer_Id) FROM Customers";
                    using (var command = new SQLiteCommand(getMaxIdQuery, connection))
                    {
                        var result = command.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            newCustomerId = Convert.ToInt32(result) + 1;
                        }
                    }

                    string query = "INSERT INTO Customers (Customer_Id ,Customer_Name, Address, Email) VALUES " +
                        "( @Customer_Id,@Customer_Name, @Address, @Email)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Customer_Id", newCustomerId);
                        command.Parameters.AddWithValue("@Customer_Name", newCustomer.Customer_Name);
                        command.Parameters.AddWithValue("@Address", newCustomer.Address);
                        command.Parameters.AddWithValue("@Email", newCustomer.Email);
                        command.ExecuteNonQuery();
                    }
                }

                CustomersDatabase();
            }
        }

        private void SearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = Microsoft.VisualBasic.Interaction.InputBox("กรอกชื่อหรืออีเมลลูกค้าเพื่อค้นหา", "ค้นหาลูกค้า", "");

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Customer_Id, Customer_Name, Email, Phone FROM Customers WHERE Customer_Name LIKE @Query OR Email LIKE @Query";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Query", "%" + searchQuery + "%");
                        using (var reader = command.ExecuteReader())
                        {
                            var results = new ObservableCollection<Customerdata>();
                            while (reader.Read())
                            {
                                results.Add(new Customerdata
                                {
                                    Customer_Id = reader.GetInt32(0),
                                    Customer_Name = reader.GetString(1),
                                    Address = reader.GetString(2),
                                    Email = reader.GetString(3)
                                });
                            }

                            if (results.Any())
                            {
                                MessageBox.Show($"พบลูกค้า {results.Count} คน", "ผลการค้นหา");
                            }
                            else
                            {
                                MessageBox.Show("ไม่พบลูกค้าที่ต้องการค้นหา", "ผลการค้นหา");
                            }
                        }
                    }
                }
            }
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is Customerdata selectedCustomer)
            {
                var editWindow = new CustomerDetail(selectedCustomer);
                if (editWindow.ShowDialog() == true)
                {
                    var updatedCustomer = editWindow.Customer;

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "UPDATE Customers SET Name = @Name, Email = @Email, Phone = @Phone WHERE Customer_Id = @Customer_Id";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Name", updatedCustomer.Customer_Name);
                            command.Parameters.AddWithValue("@Email", updatedCustomer.Address);
                            command.Parameters.AddWithValue("@Phone", updatedCustomer.Email);
                            command.Parameters.AddWithValue("@Customer_Id", updatedCustomer.Customer_Id);
                            command.ExecuteNonQuery();
                        }
                    }

                    CustomersDatabase();
                }
            }
            else
            {
                MessageBox.Show("กรุณาเลือกข้อมูลลูกค้าที่ต้องการแก้ไข", "แจ้งเตือน");
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is Customerdata selectedCustomer)
            {
                if (MessageBox.Show($"คุณต้องการลบข้อมูลลูกค้า {selectedCustomer.Customer_Name} ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Customers WHERE Customer_Id = @Customer_Id";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Customer_Id", selectedCustomer.Customer_Id);
                            command.ExecuteNonQuery();
                        }
                    }

                    CustomersDatabase();
                }
            }
            else
            {
                MessageBox.Show("กรุณาเลือกข้อมูลลูกค้าที่ต้องการลบ", "แจ้งเตือน");
            }
        }
    }

    public class Customerdata
    {
        public int Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
    }
}