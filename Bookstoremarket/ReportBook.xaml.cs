using System;
using System.Collections.Generic;
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
    /// Interaction logic for ReportBook.xaml
    /// </summary>
    public partial class ReportBook : Window
    {
        private string connectionString = @"Data Source=D:\base\Basestore.db;Version=3;";
        public ReportBook(List<string> orderSummary)
        {
            InitializeComponent();

            foreach (var item in orderSummary)
            {
                OrderSummaryListBox.Items.Add(item);
            }
        }

        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // เชื่อมต่อฐานข้อมูล SQLite
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // ดึงข้อมูลการสั่งซื้อจากรายการสรุป
                    var IdCustomer = OrderSummaryListBox.Items
                     .Cast<string>()
                     .FirstOrDefault(x => x.StartsWith("รหัสผู้ซื้อ"))
                     ?.Split(':')[1]
                     ?.Trim();
                    var isbn = OrderSummaryListBox.Items
                     .Cast<string>()
                     .FirstOrDefault(x => x.StartsWith("รหัสหนังสือ"))
                     ?.Split(':')[1]
                     ?.Trim();
                    var namebook = OrderSummaryListBox.Items
                        .Cast<string>()
                        .FirstOrDefault(x => x.StartsWith("ชื่อหนังสือ"))
                        ?.Split(':')[1]
                        ?.Trim(); 
                    var quantity = OrderSummaryListBox.Items
                        .Cast<string>()
                        .FirstOrDefault(x => x.StartsWith("จำนวน"))
                        ?.Split(':')[1]
                        ?.Trim();
                    var totalPrice = OrderSummaryListBox.Items
                        .Cast<string>()
                        .FirstOrDefault(x => x.StartsWith("รวม"))
                        ?.Split(':')[1]
                        ?.Replace("บาท", "")
                        ?.Trim();

                    if (isbn != null && quantity != null && totalPrice != null)
                    {
                        // แปลงข้อมูลให้อยู่ในรูปแบบที่ถูกต้อง
                        int parsedQuantity = int.Parse(quantity);
                        int parsedTotalPrice = int.Parse(totalPrice);

                        // เพิ่มข้อมูลลงในตาราง Transactions
                        string query = "INSERT INTO Transactions (ISBN, Customer_Id, Quatity, Total_Price) VALUES (@ISBN, @CustomerId, @Quantity, @TotalPrice)";
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ISBN", isbn);
                            command.Parameters.AddWithValue("@CustomerId", IdCustomer); // ค่า Customer_Id (สามารถปรับตามระบบจริง)
                            command.Parameters.AddWithValue("@Quantity", parsedQuantity);
                            command.Parameters.AddWithValue("@TotalPrice", parsedTotalPrice);
                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("การสั่งซื้อสำเร็จและบันทึกลงฐานข้อมูล!", "ยืนยัน", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("เกิดข้อผิดพลาดในการดึงข้อมูลรายการสรุป", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Close(); // ปิดหน้าต่างสรุปรายการ
            }
            
        }
    }
}
