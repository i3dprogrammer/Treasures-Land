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


namespace DrNadaTreasureLand._Classes
{
    /// <summary>
    /// Interaction logic for AddNewClass.xaml
    /// </summary>
    public partial class AddNewClass
    {
        public Class EditedClass;

        public AddNewClass()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check for validation errors
                string errorList = "";

                if (date_start.SelectedDate == null && toggle_full.IsChecked == true)
                    errorList += "Start date cannot be null while class is full!\n";
                if (cmb_courses.SelectedIndex == -1)
                    errorList += "You must select a Course to add new class!\n";
                //if (cmb_instructors.SelectedIndex == -1)
                //    errorList += "You must select an Instructor to add new class!\n";


                if (date_start.SelectedDate != null) //Check if the date in future, and the start date matches a selected day.
                {
                    //if (date_start.SelectedDate.Value.Date.CompareTo(DateTime.Now) == -1)
                    //    errorList += "The start date must be in the future.\n";

                    bool startDateIsCorrect = false;

                    Globals.FindVisualChildren<CheckBox>(this).ToList().ForEach(x =>
                    {
                        if (x.IsChecked == true && x.Content != null && ((DayOfWeek)date_start.SelectedDate.Value.DayOfWeek).ToString() == (string)x.Content)
                        {
                            startDateIsCorrect = true;
                        }
                    });

                    if (startDateIsCorrect == false)
                        errorList += "The start date must be one of the selected days per week.\n";
                }

                //If errors exists show them
                if (errorList != "")
                {
                    await this.ShowMessageAsync("Check the following!", errorList);
                    return;
                }

                //Get data from the window
                var course = ((Course)cmb_courses.SelectedItem);

                var newClass = new Class()
                {
                    CourseId = course.Id,
                    Over = false,
                    StartDate = date_start.SelectedDate,
                    Full = toggle_full.IsChecked.Value,
                    StartTime = time_start.SelectedTime,
                    EndTime = time_start.SelectedTime,
                };             

                foreach (var item in Globals.FindVisualChildren<CheckBox>(this))
                {
                    if (item.IsChecked == true && item.Content != null)
                        newClass.DaysPerWeek.Add(item.Content.ToString());
                }

                if (EditedClass == null)
                {
                    int x = 1;
                    while (course.Classes.Exists(y => y.Number == x))
                        x++;
                    newClass.Number = x;
                    newClass = Classes.AddCourseClass(newClass); //Add the new class to the database.
                }
                else
                {
                    newClass.Id = EditedClass.Id;
                    newClass.Number = EditedClass.Number;
                    Classes.EditCourseClass(newClass);
                }

                //Remove old data, incase of editing.. if the class is new there should be no data to remove.
                Children.RemoveChildClass(newClass.Id);
                //For each selected children, add it to the database
                foreach (Objects.CheckedObject item in listView_children.Items)
                {
                    //if the child is selected && the child dosen't have the selected class already
                    if (item.Checked == true)
                        Children.AddChildClass(item.Child.Id, newClass.Id);
                }

                if (cmb_instructors.SelectedIndex != -1)
                {
                    //Remove old data
                    Instructors.RemoveInstructorClass(newClass.Id);
                    Instructors.AddInstructorClass(((Instructor)cmb_instructors.SelectedItem).Id, newClass.Id);
                }

                Globals.RefreshReferenceInformation();
                this.Close();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Globals.Courses.ToList().ForEach(x => cmb_courses.Items.Add(x.Value));
            Globals.Instructors.ToList().ForEach(x => cmb_instructors.Items.Add(x.Value));

            if(EditedClass != null)
            {
                try
                {
                    cmb_courses.SelectedItem = Globals.Courses[EditedClass.CourseId];
                    if(Globals.Instructors.ToList().Exists(x => x.Value.TeachingCourses.Contains(EditedClass)))
                        cmb_instructors.SelectedItem = Globals.Instructors.ToList().First(x => x.Value.TeachingCourses.Contains(EditedClass)).Value;
                    toggle_full.IsChecked = EditedClass.Full;
                    date_start.SelectedDate = EditedClass.StartDate;
                    time_start.SelectedTime = EditedClass.StartTime;
                    time_end.SelectedTime = EditedClass.EndTime;

                    foreach (var item in Globals.FindVisualChildren<CheckBox>(this))
                    {
                        if (item.Content != null)
                        {
                            if (EditedClass.DaysPerWeek.Contains(item.Content))
                                item.IsChecked = true;
                        }
                    }

                    btn_delete.Visibility = Visibility.Visible;
                    btn_add.Content = "Edit Class";
                    this.Title = "Editing Class";
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        private void cmb_courses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_courses.SelectedIndex == -1)
                return;

            listView_children.Items.Clear();
            var course = ((Course)cmb_courses.SelectedItem);
            lbl_ageReq.Content = "Age restriction from " + course.AcademicYearStart + " To " + course.AcademicYearEnd; ;

            Globals.Children.ToList().ForEach(x =>
            {
                if (x.Value.Age >= course.AcademicYearStart && x.Value.Age <= course.AcademicYearEnd)
                {
                    var newChild = new Objects.CheckedObject()
                    {
                        Checked = false,
                        Child = x.Value,
                    };

                    if (EditedClass != null && x.Value.RegisteredCourses.Contains(EditedClass))
                        newChild.Checked = true;

                    listView_children.Items.Add(newChild);
                }
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Globals.ClearForm(this);
        }

        private async void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync("Warning", "Are you sure?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
                return;

            Classes.RemoveCourseClass(EditedClass);
        }
    }
}
