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

namespace DrNadaTreasureLand.Windows
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class SalaryHistory
    {
        public Instructor c;
        public SalaryHistory()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (c == null)
                throw new ArgumentNullException("c", "Instructor cannot be null");

            if (Globals.InstructorSalaries.ContainsKey(c.Id))
            {
                foreach(var item in Globals.InstructorSalaries[c.Id])
                {
                    var lstItem = new Objects.InstructorSalaryListViewItem()
                    {
                        Paid = item.Paid,
                        Salary = item.Salary,
                        Instructor = c,
                    };
                    if (Globals.Courses.ContainsKey(item.CourseId))
                        lstItem.Course = Globals.Courses[item.CourseId];
                    else
                        lstItem.Course = new Course() { Name = "---" }; //HEHE XD

                    lstView_main.Items.Add(lstItem);
                }
            } else
            {
                throw new Exception("Instructor doesn't exist in salaries");
            }
        }
    }
}
