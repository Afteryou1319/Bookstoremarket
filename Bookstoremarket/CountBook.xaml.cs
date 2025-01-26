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
    /// Interaction logic for CountBook.xaml
    /// </summary>
    public partial class CountBook : Window
    {
        public int Quantity { get; private set; }
        public string Id_customer { get; private set; }
        public CountBook()
        {
            InitializeComponent();
        }
        private void ConfirmQuantity_Click(object sender, RoutedEventArgs e)
        {
            Id_customer = IdCustomerText.Text;
            if (int.TryParse(QuantityTextBox.Text, out int quantity) && quantity > 0)
            {
                Quantity = quantity;
                DialogResult = true; // Close window with success
            }
            else
            {
                MessageBox.Show("กรุณากรอกจำนวนที่ถูกต้อง", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
