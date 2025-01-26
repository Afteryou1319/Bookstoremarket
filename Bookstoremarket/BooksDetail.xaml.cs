using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Xml.Linq;

namespace Bookstoremarket
{
    /// <summary>
    /// Interaction logic for BooksDetail.xaml
    /// </summary>
    public partial class BooksDetail : Window
    {
        public Booksdata Booklist { get; private set; }

        public BooksDetail(Booksdata booklist = null)
        {
            InitializeComponent();

            if (booklist != null)
            {
                Booklist = booklist;
                namebook.Text = booklist.Title;
                desbook.Text = booklist.Description;
                price.Text = booklist.Price.ToString();
            }
            else
            {
                Booklist = new Booksdata();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(price.Text, out int priceValue))
            {
                // แสดงหน้าต่างแจ้งเตือน
                MessageBox.Show("กรุณากรอกราคาให้เป็นตัวเลข", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // ยุติการทำงานของเมธอด
            }
            Booklist.Title = namebook.Text;
            Booklist.Description = desbook.Text;
            Booklist.Price = Convert.ToInt32(price.Text); 

            DialogResult = true;
            Close();
        }
    }
}
