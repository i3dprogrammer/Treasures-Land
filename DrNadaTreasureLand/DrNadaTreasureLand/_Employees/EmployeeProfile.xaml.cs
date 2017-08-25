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
using TreasuresLand.Objects;
namespace DrNadaTreasureLand._Employees
{
    /// <summary>
    /// Interaction logic for EmployeeProfile.xaml
    /// </summary>
    public partial class EmployeeProfile
    {
        public Employee c;

        public EmployeeProfile()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Employee " + c.Name + " Profile";
            lbl_name.Content = c.Name;
            lbl_age.Content = c.Age + " Years Old";
            lbl_gender.Content = (c.Gender == 0) ? "Male" : "Female";
            lbl_phone.Content = c.Phone;
            lbl_qualifier.Content = c.Qualifier;
            lbl_address.Content = c.Address;
            lbl_nationalId.Content = c.NationalId;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog().GetValueOrDefault(false))
                {
                    this.Height -= 40;
                    pd.PrintVisual(this, this.Title);
                    this.Height += 40;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
