using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainMenupage.xaml
    /// </summary>
    public partial class MainMenupage : Window
    {
        public MainMenupage()
        {
            InitializeComponent();
        }
        private void CustomerManagement_Click(object sender, RoutedEventArgs e)
        {
            Customer customPage = new Customer();
            customPage.Show();
           // MessageBox.Show("เปิดหน้าจัดการข้อมูลลูกค้า", "Customer Management", MessageBoxButton.OK, MessageBoxImage.Information);
            // นำทางไปยังหน้าจัดการข้อมูลลูกค้า
        }

        private void BookManagement_Click(object sender, RoutedEventArgs e)
        {
            Bookdata bookPage = new Bookdata();
            bookPage.Show();
            //MessageBox.Show("เปิดหน้าจัดการข้อมูลหนังสือ", "Book Management", MessageBoxButton.OK, MessageBoxImage.Information);
            // นำทางไปยังหน้าจัดการข้อมูลหนังสือ
        }

        private void OrderBooks_Click(object sender, RoutedEventArgs e)
        {
            OrderBook order = new OrderBook();
            order.Show();
            //MessageBox.Show("เปิดหน้าสั่งซื้อหนังสือ", "Order Books", MessageBoxButton.OK, MessageBoxImage.Information);
            // นำทางไปยังหน้าสั่งซื้อหนังสือ
        }
    }
}
