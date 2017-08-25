using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TreasuresLand.Objects;

namespace DrNadaTreasureLand
{
    class Globals
    {
        public static MainWindow MainWindow;
        public static Dictionary<int, Course> Courses = new Dictionary<int, Course>();
        public static Dictionary<int, Instructor> Instructors = new Dictionary<int, Instructor>();
        public static Dictionary<int, Child> Children = new Dictionary<int, Child>();
        public static Dictionary<int, Employee> Employees = new Dictionary<int, TreasuresLand.Objects.Employee>();
        public static Dictionary<int, List<Instructor._Salary>> InstructorSalaries = new Dictionary<int, List<Instructor._Salary>>();

        public static void RefreshReferenceInformation()
        {
            try
            {
                Courses.Clear();
                Instructors.Clear();
                Children.Clear();
                Employees.Clear();
                InstructorSalaries.Clear();

                TreasuresLand.SQL.Courses.GetAllCourses().ForEach(x => Globals.Courses.Add(x.Id, x));
                TreasuresLand.SQL.Children.GetAllChildren().ForEach(x => Globals.Children.Add(x.Id, x));
                TreasuresLand.SQL.Employees.GetAllEmployees().ForEach(x => Globals.Employees.Add(x.Id, x));
                TreasuresLand.SQL.Instructors.GetAllInstructors().ForEach(x =>
                {
                    Globals.Instructors.Add(x.Id, x);
                    Globals.InstructorSalaries.Add(x.Id, TreasuresLand.SQL.Instructors.GetPreviousInstructorSalary(x.Id));
                });

                MainWindow.UpdateListViews();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void ClearForm(DependencyObject debObj)
        {
            foreach (var item in Globals.FindVisualChildren<TextBox>(debObj))
            {
                item.Text = "";
            }

            foreach (var item in Globals.FindVisualChildren<CheckBox>(debObj))
            {
                item.IsChecked = false;
            }

            foreach (var item in Globals.FindVisualChildren<ComboBox>(debObj))
            {
                item.SelectedIndex = -1;
            }

            foreach (var item in Globals.FindVisualChildren<NumericUpDown>(debObj))
            {
                item.Value = item.Minimum;
            }

            foreach (var item in Globals.FindVisualChildren<DatePicker>(debObj))
            {
                item.SelectedDate = null;
            }

            foreach (var item in Globals.FindVisualChildren<ToggleSwitch>(debObj))
            {
                item.IsChecked = false;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++) //TODO: Use LogicalTreeHelper to find hidden instances as well
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }
}
