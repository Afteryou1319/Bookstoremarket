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
    /// Interaction logic for CustomerDetail.xaml
    /// </summary>
    public partial class CustomerDetail : Window
    {
       public Customerdata Customer { get; private set; }

        public CustomerDetail(Customerdata customer = null)
        {
            InitializeComponent();

            if (customer != null)
            {
                Customer = customer;
                name.Text = customer.Customer_Name;
                address.Text = customer.Address;
                email.Text = customer.Email;
            }
            else
            {
                Customer = new Customerdata();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Customer.Customer_Name = name.Text;
            Customer.Address = address.Text;
            Customer.Email = email.Text;

            DialogResult = true;
            Close();
        }
    }
}