﻿using System;
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
    /// Interaction logic for OrderBook.xaml
    /// </summary>
    public partial class OrderBook : Window
    {
        public OrderBook()
        {
            InitializeComponent();
            BookDetailsGrid.ItemsSource = Books; // Bind DataGrid to ObservableCollection
        }
        private string connectionString = @"Data Source=D:\base\Basestore.db;Version=3;";
        public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();

        // Search Button Click Event
        private void SearchBook_Click(object sender, RoutedEventArgs e)
        {
            string isbn = ISBNTextBox.Text;

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // คำสั่ง SQL สำหรับค้นหาหนังสือจาก ISBN
                    string query = "SELECT Title, Description, Price FROM Books WHERE ISBN = @ISBN";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ISBN", isbn);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // สร้าง Book จากข้อมูลที่ค้นพบ
                                var book = new Book
                                {
                                    ISBN = isbn,
                                    Title = reader.GetString(0),
                                    Description = reader.GetString(1),
                                    Price = reader.GetDecimal(2)
                                };

                                // ล้างข้อมูลเก่าและเพิ่มหนังสือที่ค้นพบ
                                Books.Clear();
                                Books.Add(book);
                            }
                            else
                            {
                                // ถ้าไม่พบหนังสือ
                                MessageBox.Show("ไม่พบหนังสือที่มีหมายเลข ISBN นี้", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
            }
            else
            {
                // กรณีไม่ได้กรอก ISBN
                MessageBox.Show("กรุณากรอกเลข ISBN", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            if (Books.Count > 0)
            {
                var selectedBook = Books[0]; // Assuming single book selection for now

                var quantityWindow = new CountBook();
                if (quantityWindow.ShowDialog() == true)
                {
                    int quantity = quantityWindow.Quantity;
                    string idcustommer = quantityWindow.Id_customer;

                    var orderSummary = new List<string>
                    {
                        $"รหัสผู้ซื้อ: {idcustommer}",
                        $"รหัสหนังสือ: {selectedBook.ISBN}",
                        $"ชื่อหนังสือ: {selectedBook.Title}",
                        $"ประเภท: {selectedBook.Description}",
                        $"ราคา: {selectedBook.Price} บาท",
                        $"จำนวน: {quantity}",
                        $"รวม: {selectedBook.Price * quantity} บาท"
                    };

                    var summaryWindow = new ReportBook(orderSummary);
                    summaryWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("กรุณาค้นหาหนังสือก่อนดำเนินการต่อ", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // Book Model
    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
