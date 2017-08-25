using MahApps.Metro.Controls.Dialogs;
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
using TreasuresLand.SQL;

namespace DrNadaTreasureLand._Employees
{
    /// <summary>
    /// Interaction logic for AddNewEmployee.xaml
    /// </summary>
    public partial class AddNewEmployee
    {
        public Employee EditedEmployee;

        public AddNewEmployee()
        {
            InitializeComponent();
        }

        private async void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync("Are you sure", "You're deleting an employee, are you sure you want to do that?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                Employees.RemoveEmployee(EditedEmployee);
                Globals.RefreshReferenceInformation();
                this.Close();
            }
        }

        private void btn_addInstructor_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_gender.SelectedIndex == -1)
                return;


            try
            {
                var newEmp = new Employee()
                {
                    Name = txt_employeeName.Text,
                    Address = txt_address.Text,
                    Phone = txt_phone.Text,
                    Gender = cmb_gender.SelectedIndex,
                    Qualifier = txt_qualifier.Text,
                    Age = Convert.ToInt32(num_age.Value),
                    NationalId = txt_nationalId.Text,
                };

                if(EditedEmployee == null)
                    Employees.AddEmployee(newEmp);
                else
                {
                    newEmp.Id = EditedEmployee.Id;
                    Employees.EditEmployee(newEmp);
                }

                Globals.RefreshReferenceInformation();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            num_age.Value = 1;
            num_age.Minimum = 1;
            num_age.Maximum = 100;

            if(EditedEmployee != null)
            {
                this.Title = "Editing " + EditedEmployee.Name;
                txt_address.Text = EditedEmployee.Address;
                txt_employeeName.Text = EditedEmployee.Name;
                txt_phone.Text = EditedEmployee.Phone;
                txt_qualifier.Text = EditedEmployee.Qualifier;
                txt_nationalId.Text = EditedEmployee.NationalId;
                cmb_gender.SelectedIndex = EditedEmployee.Gender;
                num_age.Value = EditedEmployee.Age;
                btn_delete.Visibility = Visibility.Visible;
                btn_addEmployee.Content = "Edit Employee";
            }
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            Globals.ClearForm(this);
        }
    }
}
