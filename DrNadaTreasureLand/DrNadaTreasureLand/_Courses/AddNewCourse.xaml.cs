using MahApps.Metro.Controls;
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

namespace DrNadaTreasureLand._Courses
{
    /// <summary>
    /// Interaction logic for AddNewCourse.xaml
    /// </summary>
    public partial class AddNewCourse
    {
        public Course EditedCourse;

        List<CheckBox> AllCheckBoxses = new List<CheckBox>();


        public AddNewCourse()
        {
            this.InitializeComponent();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Check for validation errors
            string errorList = "";

            //if (date_start.SelectedDate == null)
            //    errorList += "Start date cannot be null.\n";
            if (num_cost.Value == null || num_fromYear.Value == null || num_level.Value == null || num_pricePerChild == null || num_toYear.Value == null || num_totalSessionsCount.Value == null || num_sessionHours.Value == null)
                errorList += "Numeric values cannot be null.\n";
            if (num_fromYear.Value > num_toYear.Value)
                errorList += "Start age cannot be higher than end age.\n";
            //if (AllCheckBoxses.Where(x => x.IsChecked == true).Count() == 0)
            //    errorList += "You must choose at least 1 day to continue.\n";

            if (Globals.Courses.ToList().Exists(x => x.Value.Name == txt_courseName.Text))
            {
                if(EditedCourse == null || (EditedCourse != null && EditedCourse.Name != txt_courseName.Text)){
                    errorList += "There exists a course with this name already, please add another name idenitifer.\n";
                }
            }

            //if (date_start.SelectedDate.Value.Date.CompareTo(DateTime.Now) == -1)
            //    errorList += "The start date must be in the feature.\n";

            //bool startDateIsCorrect = false;

            //AllCheckBoxses.ForEach(x => {
            //    if (x.IsChecked == true && ((DayOfWeek)date_start.SelectedDate.Value.DayOfWeek).ToString() == (string)x.Content)
            //    {
            //        startDateIsCorrect = true;
            //    }
            //});

            //if (startDateIsCorrect == false)
            //    errorList += "The start date must be one of the selected days per week.\n";

            //If errors exists show them
            if(errorList != "")
            {
                await this.ShowMessageAsync("Check the following!", errorList);
                return;
            }

            //Initalize new course with the data
            var course = new Course()
            {
                Name = txt_courseName.Text,
                Cost = Convert.ToInt32(num_cost.Value),
                AcademicYearStart = Convert.ToInt32(num_fromYear.Value),
                AcademicYearEnd = Convert.ToInt32(num_toYear.Value),
                //Full = (bool)toggle_full.IsChecked,
                //Over = false,
                Level = Convert.ToInt32(num_level.Value),
                PricePerChild = Convert.ToInt32(num_pricePerChild.Value),
                //StartDate = (DateTime)date_start.SelectedDate,
                TotalSessions = Convert.ToInt32(num_totalSessionsCount.Value),
                SessionDuration = Convert.ToInt32(num_sessionHours.Value),
            };

            //Add course days per week

            //foreach (var item in AllCheckBoxses)
            //{
            //    if (item.IsChecked == true)
            //        course.DaysPerWeek.Add(item.Content.ToString());
            //}


            try
            {
                if(EditedCourse == null) //Check if we are adding a new course or editing existing one.
                    Courses.AddCourse(course);
                else
                {
                    course.Id = EditedCourse.Id;
                    //course.ShiftedDays = EditedCourse.ShiftedDays;
                    Courses.EditCourse(course);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            Globals.RefreshReferenceInformation();

            this.Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //LAZY WAY, TODO: Fix this by using VisualTreeHelper/LogicalTreeHelper
            //AllCheckBoxses.Add(cmb_day1);
            //AllCheckBoxses.Add(cmb_day2);
            //AllCheckBoxses.Add(cmb_day3);
            //AllCheckBoxses.Add(cmb_day4);
            //AllCheckBoxses.Add(cmb_day5);
            //AllCheckBoxses.Add(cmb_day6);
            //AllCheckBoxses.Add(cmb_day7);

            //Setting up numeric the minimum and maximum for numeric value inputs.
            num_sessionHours.Value = 1;
            num_sessionHours.Minimum = 1;
            num_sessionHours.Maximum = 24;
            num_totalSessionsCount.Value = 1;
            num_totalSessionsCount.Minimum = 1;
            num_totalSessionsCount.Maximum = 1000000;
            num_cost.Value = 0;
            num_cost.Minimum = 0;
            num_cost.Maximum = 1000000;
            num_pricePerChild.Value = 0;
            num_pricePerChild.Minimum = 0;
            num_pricePerChild.Maximum = 1000000;
            num_level.Value = 1;
            num_level.Minimum = 1;
            num_level.Maximum = 1000000;
            num_fromYear.Value = 1;
            num_fromYear.Minimum = 1;
            num_fromYear.Maximum = 100;
            num_toYear.Value = 1;
            num_toYear.Minimum = 1;
            num_toYear.Maximum = 100;


            //If we are editing a course, fill the fields
            if (EditedCourse != null)
            {
                this.Title = "Editing " + EditedCourse.Name;
                txt_courseName.Text = EditedCourse.Name;
                num_cost.Value = EditedCourse.Cost;
                num_fromYear.Value = EditedCourse.AcademicYearStart;
                num_toYear.Value = EditedCourse.AcademicYearEnd;
                num_level.Value = EditedCourse.Level;
                num_pricePerChild.Value = EditedCourse.PricePerChild;
                num_sessionHours.Value = EditedCourse.SessionDuration;
                //date_start.SelectedDate = EditedCourse.StartDate;
                num_totalSessionsCount.Value = EditedCourse.TotalSessions;
                //toggle_full.IsChecked = EditedCourse.Full;

                //foreach(var item in AllCheckBoxses)
                //{
                //    if(EditedCourse.DaysPerWeek.Exists(x => x == (string)item.Content))
                //    {
                //        item.IsChecked = true;
                //    }
                //}

                btn_actionCourse.Content = "Edit Course";
                btn_deleteCourse.Visibility = Visibility.Visible;
            }
        }

        private async void btn_deleteCourse_Click(object sender, RoutedEventArgs e)
        {
            bool childrenCheck = false;
            bool instructorCheck = false;
            bool salaryCheck = false;

            Globals.Children.ToList().ForEach(x => {
                if (x.Value.RegisteredCourses.Exists(z => z.CourseId == EditedCourse.Id))
                    childrenCheck = true;
            });

            Globals.Instructors.ToList().ForEach(x =>
            {
                if (x.Value.TeachingCourses.Exists(z => z.CourseId == EditedCourse.Id))
                    instructorCheck = true;
            });

            Globals.Instructors.ToList().ForEach(x =>
            {
                Globals.InstructorSalaries[x.Value.Id].ForEach(y =>
                {
                    if (y.CourseId == EditedCourse.Id)
                        salaryCheck = true;
                });
            });


            if (childrenCheck)
            {
                if(await this.ShowMessageAsync("Are you sure", "There's children in this course, are you sure you want to delete it?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
                    return;
            }

            if (instructorCheck) {
                if (await this.ShowMessageAsync("Are you sure", "There's instructor teaching this course, are you sure you want to delete it?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
                    return;
            }

            if (salaryCheck)
            {
                if (await this.ShowMessageAsync("Are you sure", "Deleting this course will make accessing salary course information for this specific course not valid, are you sure?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
                    return;
            }

            if (childrenCheck || instructorCheck || salaryCheck || await this.ShowMessageAsync("Are you sure", "You're deleting a course, are you sure you want to do that?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                Courses.RemoveCourse(EditedCourse);
                Globals.RefreshReferenceInformation();
                this.Close();
            }
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            Globals.ClearForm(this);
        }
    }
}
