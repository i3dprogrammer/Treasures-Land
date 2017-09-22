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

namespace DrNadaTreasureLand._Instructors
{
    /// <summary>
    /// Interaction logic for InstructorProfile.xaml
    /// </summary>
    public partial class InstructorProfile
    {
        public Instructor c;

        public InstructorProfile()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Title = "Instructor " + c.Name + " Profile";
                lbl_name.Content = c.Name;
                lbl_age.Content = c.Age + " Years Old";
                lbl_gender.Content = (c.Gender == 0) ? "Male" : "Female";
                lbl_phone.Content = c.Phone;
                lbl_qualifier.Content = c.Qualifier;
                lbl_address.Content = c.Address;

                for (int i = 0; i < c.TeachingCourses.Count; i++)
                {
                    var x = c.TeachingCourses[i];
                    Console.WriteLine(c.TeachingCourses.Count);
                    if (x.Over)
                        continue;

                    courses_listView.Items.Add(new Objects.CheckedObject()
                    {
                        Class = x,
                        Course = Globals.Courses.First(y => y.Value.Classes.Contains(x)).Value,
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (c.CV != null)
            {
                System.IO.File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "\\tempCV.pdf", c.CV);
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + "\\tempCV.pdf");
            }
            else
            {
                await this.ShowMessageAsync("Error", "This instructor has no CV file.");
            }
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
