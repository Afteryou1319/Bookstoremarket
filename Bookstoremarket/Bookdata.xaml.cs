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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bookstoremarket
{
    /// <summary>
    /// Interaction logic for Bookdata.xaml
    /// </summary>
    public partial class Bookdata : Window
    {
        public ObservableCollection<Booksdata> Booklists { get; set; } = new ObservableCollection<Booksdata>();
        private string connectionString = @"Data Source=D:\base\Basestore.db;Version=3;";
        public Bookdata()
        {
            InitializeComponent();
            BookData();


            BookDataGrid.ItemsSource = Booklists;
        }
        private void BookData()
        {
            Booklists.Clear();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ISBN, Title, Description, Price FROM Books";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Booklists.Add(new Booksdata
                            {
                                ISBN = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Price = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new BooksDetail();
            if (addWindow.ShowDialog() == true)
            {
                var newCustomer = addWindow.Booklist;

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    int newCustomerId = 1;
                    string getMaxIdQuery = "SELECT MAX(ISBN) FROM Books";
                    using (var command = new SQLiteCommand(getMaxIdQuery, connection))
                    {
                        var result = command.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            newCustomerId = Convert.ToInt32(result) + 1;
                        }
                    }

                    string query = "INSERT INTO Books (ISBN ,Title, Description, Price) VALUES " +
                        "( @ISBN,@Title, @Description, @Price)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ISBN", newCustomerId);
                        command.Parameters.AddWithValue("@Title", newCustomer.Title);
                        command.Parameters.AddWithValue("@Description", newCustomer.Description);
                        command.Parameters.AddWithValue("@Price", newCustomer.Price);
                        command.ExecuteNonQuery();
                    }
                }

                BookData();
            }
        }
        private void SearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = Microsoft.VisualBasic.Interaction.InputBox("กรอกชื่อหนังสือหรือรายละเอียดเพื่อค้นหา", "ค้นหาหนังสือ", "");

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ISBN, Title, Description, Price FROM Books WHERE Title LIKE @Query OR Description LIKE @Query";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Query", "%" + searchQuery + "%");
                        using (var reader = command.ExecuteReader())
                        {
                            var results = new ObservableCollection<Booksdata>();
                            while (reader.Read())
                            {
                                results.Add(new Booksdata
                                {
                                    ISBN = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Price = reader.GetInt32(3)
                                });
                            }

                            if (results.Any())
                            {
                                MessageBox.Show($"พบหนังสือ {results.Count} เล่ม", "ผลการค้นหา");
                            }
                            else
                            {
                                MessageBox.Show("ไม่พบหนังสือที่ต้องการค้นหา", "ผลการค้นหา");
                            }
                        }
                    }
                }
            }
        }
        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (BookDataGrid.SelectedItem is Booksdata selectedCustomer)
            {
                var editWindow = new BooksDetail(selectedCustomer);
                if (editWindow.ShowDialog() == true)
                {
                    var updatedCustomer = editWindow.Booklist;

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "UPDATE Books SET Title = @Title, Description = @Description, Price = @Price WHERE ISBN = @ISBN";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Title", updatedCustomer.Title);
                            command.Parameters.AddWithValue("@Description", updatedCustomer.Description);
                            command.Parameters.AddWithValue("@Price", updatedCustomer.Price);
                            command.Parameters.AddWithValue("@ISBN", updatedCustomer.ISBN);
                            command.ExecuteNonQuery();
                        }
                    }

                    BookData();
                }
            }
            else
            {
                MessageBox.Show("กรุณาเลือกข้อมูลหนังสือที่ต้องการแก้ไข", "แจ้งเตือน");
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (BookDataGrid.SelectedItem is Booksdata selectedCustomer)
            {
                if (MessageBox.Show($"คุณต้องการลบข้อมูลหนังสือ {selectedCustomer.Title} ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Books WHERE ISBN = @ISBN";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ISBN", selectedCustomer.ISBN);
                            command.ExecuteNonQuery();
                        }
                    }

                    BookData();
                }
            }
            else
            {
                MessageBox.Show("กรุณาเลือกข้อมูลหนังสือที่ต้องการลบ", "แจ้งเตือน");
            }
        }
    }
        public class Booksdata
        {
            public int ISBN { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int Price { get; set; }
        }
}
