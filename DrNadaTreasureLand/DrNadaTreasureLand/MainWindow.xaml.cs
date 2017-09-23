using DrNadaTreasureLand.Utilities;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TreasuresLand.Objects;
using TreasuresLand.SQL;
using DrNadaTreasureLand.Windows;
using System.Windows.Media;

namespace DrNadaTreasureLand
{
    public partial class MainWindow : MetroWindow
    {
        Timer coursesRefreshTimer;
        Timer mainRefreshTimer;

        List<Objects.Filter> filters = new List<Objects.Filter>();

        public MainWindow()
        {
            try
            {
                Globals.MainWindow = this;
                InitializeComponent();
                this.MinHeight = this.Height;
                this.MinWidth = this.Width;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void MainRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    listView_endingCourses.Items.Clear();
                    listView_unpaidInstructors.Items.Clear();
                    listView_instructorsHistory.Items.Clear();
                    listView_todayCourses.Items.Clear();

                    lbl_availableCourses.Content = Globals.Courses.Count;
                    int classesFinished = 0;
                    int classesOngoing = 0;

                    Globals.Courses.ToList().ForEach(x =>
                    {
                        classesFinished += x.Value.Classes.Where(y => y.Over == true).Count();
                        classesOngoing += x.Value.Classes.Where(y => y.Over == false).Count();
                    });

                    lbl_coursesFinished.Content = classesFinished;
                    lbl_ongoingCourses.Content = classesOngoing;

                    lbl_totalChildren.Content = Globals.Children.Count;
                    lbl_totalEmployees.Content = Globals.Employees.Count;
                    lbl_totalInstructors.Content = Globals.Instructors.Count;
                    lbl_childrenNoCourses.Content = Globals.Children.ToList().Where(x => (x.Value.RegisteredCourses == null || x.Value.RegisteredCourses.Count == 0)).Count();

                    lbl_unpaidInstructors.Content = Globals.InstructorSalaries.Where(x => x.Value.Exists(y => y.Paid == false)).Count();

                    Globals.Courses.ToList().ForEach(x =>
                    {
                        foreach (var y in x.Value.Classes)
                        {
                            if (y.StartDate == null || y.EndDate == null || y.Over == true) //First condition is probably useless, as we have alternative checks.
                                continue;

                            int daysDifference = (y.EndDate.Value - DateTime.Now).Days;

                            if (daysDifference < 7)
                                listView_endingCourses.Items.Add(new Objects.CheckedObject() { Class = y, Course = x.Value });

                            if (!Globals.Instructors.ToList().Exists(z => z.Value.TeachingCourses.Contains(y)))
                            { //Incase of the class having no instructor, should not happen as we MUST add the class with instructor.
                                Classes.RemoveCourseClass(y);
                                throw new Exception("There is no instructor assigned to " + y.Course.Name + " class, this shouldn't happen.");
                            }

                            if (DateTime.Now >= y.StartDate && y.DaysPerWeek.Contains(((DayOfWeek)DateTime.Now.DayOfWeek).ToString()))
                            {
                                if (!y.ShiftedDates.Exists(z => z.Day == DateTime.Now.Day && z.Month == DateTime.Now.Month && z.Year == DateTime.Now.Year))
                                {
                                    var ins = Globals.Instructors.ToList().Single(z => z.Value.TeachingCourses.Contains(y)).Value;
                                    listView_todayCourses.Items.Add(new Objects.CheckedObject() { Class = y, Course = x.Value, Instructor = ins });
                                }
                            }
                        }
                    });

                    //Globals.Instructors.ToList().ForEach(x =>
                    //{
                    //    if (Globals.InstructorSalaries.ContainsKey(x.Value.Id))
                    //    {
                    //        foreach (var item in Globals.InstructorSalaries[x.Value.Id])
                    //        {
                    //            if (!Globals.Courses.ContainsKey(item.CourseId) || item.Paid == true)
                    //                continue;

                    //            var newSalary = new Objects.UnpaidInstructorListViewItem()
                    //            {
                    //                Course = Globals.Courses[item.CourseId],
                    //                Instructor = Globals.Instructors[item.InstructorId],
                    //                Salary = item.Salary,
                    //            };

                    //            //if (!listView_unpaidInstructors.Items.Contains(newSalary))
                    //            listView_unpaidInstructors.Items.Add(newSalary);
                    //        }

                    //    }

                    //    //if (!listView_instructorsHistory.Items.Contains(x.Value))
                    //    listView_instructorsHistory.Items.Add(x.Value);
                    //});

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CoursesRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool refreshData = false;

            Globals.Courses.ToList().ForEach(x => //Get every course and loop through, x.Value = course
            {
                foreach (var y in x.Value.Classes) //Loop through every class in the course
                {
                    if (y.EndDate <= DateTime.Now.Date && y.Over == false) //If we passed the end date AND the class is not over yet
                    {
                        if (!Globals.Instructors.ToList().Exists(z => z.Value.TeachingCourses.Contains(y)))
                        {   //Incase of the class having no instructor, should not happen as we MUST add the class with instructor.
                            //Classes.RemoveCourseClass(y);
                            //throw new Exception("There is no instructor assigned to " + y.Course.Name + " class, this shouldn't happen.");
                            continue;
                        }

                        var ins = Globals.Instructors.ToList().Single(z => z.Value.TeachingCourses.Contains(y)).Value; //Get the class instructor
                        int childrenCount = Globals.Children.ToList().Where(z => z.Value.RegisteredCourses.Contains(y)).Count(); //Get Children Count taking this class.

                        Instructors.AddInstructorSalary(ins.Id, y.CourseId, x.Value.PricePerChild * x.Value.Cost * childrenCount / 100); //Add Salary

                        foreach (var child in Globals.Children.ToList().Where(z => z.Value.RegisteredCourses.Contains(y))) //Remove every child from the class
                        {
                            Children.RemoveChildClass(y.Id);
                        }

                        y.Over = true; //Set over to true
                        Classes.EditCourseClass(y); //Edit the class in the db set over to true
                        refreshData = true;
                    }
                }
            });

            if (refreshData)
                Globals.RefreshReferenceInformation(); //Refresh db information

        }

        private void UpdateCoursesListView(Course item)
        {
            if (courses_listView.Items.Count != Globals.Courses.Count)
            {
                UpdateListViews();
                return;
            }

            for (int i = 0; i < Globals.Courses.Count; i++)
            {
                if (Globals.Courses[i] != ((Course)courses_listView.Items[i]))
                {
                    UpdateListViews();
                    return;
                }
            }
        }

        public static DateTime GetDateOfTheNext(DateTime from, DayOfWeek dayOfWeek) //TODO: Should probably make it extension method.
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _Children.AddNewChild newChildForm = new _Children.AddNewChild();
            newChildForm.ShowDialog();
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (children_listView.SelectedIndex == -1)
                return;

            _Children.AddNewChild newChildForm = new _Children.AddNewChild();
            newChildForm.EditedChild = (Child)children_listView.SelectedItem;
            newChildForm.ShowDialog();
        }

        private void txt_filterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            children_listView.Items.Clear();
            UpdateListViews();
        }

        private void button1_Copy1_Click(object sender, RoutedEventArgs e)
        {
            Filter newFilter = new Filter();
            newFilter.FilterType = new Child();
            newFilter.filters = filters;
            newFilter.ShowDialog();
        }

        private void children_listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (children_listView.SelectedIndex == -1)
                return;

            _Children.ChildProfile newProf = new _Children.ChildProfile();
            newProf.c = (Child)children_listView.SelectedItem;
            newProf.ShowDialog();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _Courses.AddNewCourse nc = new _Courses.AddNewCourse();
            nc.ShowDialog();
        }

        private void button1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            Filter newFilter = new Filter();
            newFilter.FilterType = new Course();
            newFilter.filters = filters;
            newFilter.ShowDialog();
        }

        private void txt_filterCourses_TextChanged(object sender, TextChangedEventArgs e)
        {
            courses_listView.Items.Clear();
            UpdateListViews();
        }

        private void courses_listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (courses_listView.SelectedIndex == -1)
                return;
            try
            {
                _Courses.CourseProfile newCourse = new _Courses.CourseProfile();
                newCourse.c = (Course)courses_listView.SelectedItem;
                newCourse.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void children_listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txt_filterInstructors_TextChanged(object sender, TextChangedEventArgs e)
        {
            instructors_listView.Items.Clear();
            UpdateListViews();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            _Instructors.AddNewInstructor newIns = new _Instructors.AddNewInstructor();
            newIns.ShowDialog();
        }

        private void button1_Copy4_Click(object sender, RoutedEventArgs e)
        {
            if (instructors_listView.SelectedIndex == -1)
                return;

            _Instructors.AddNewInstructor nc = new _Instructors.AddNewInstructor();
            nc.EditedInstructor = (Instructor)((ListViewItem)instructors_listView.SelectedItem).Content;
            nc.ShowDialog();
        }

        private void button1_Copy5_Click(object sender, RoutedEventArgs e)
        {
            Filter newFilter = new Filter();
            newFilter.FilterType = new Instructor();
            newFilter.filters = filters;
            newFilter.ShowDialog();
        }

        private void instructors_listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (instructors_listView.SelectedIndex == -1)
                return;

            _Instructors.InstructorProfile pf = new _Instructors.InstructorProfile();
            pf.c = (Instructor)((ListViewItem)instructors_listView.SelectedItem).Content;
            pf.ShowDialog();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            _Employees.AddNewEmployee ne = new _Employees.AddNewEmployee();
            ne.ShowDialog();
        }

        private void button1_Copy6_Click(object sender, RoutedEventArgs e)
        {
            if (employees_listView.SelectedIndex == -1)
                return;

            _Employees.AddNewEmployee ne = new _Employees.AddNewEmployee();
            ne.EditedEmployee = (Employee)employees_listView.SelectedItem;
            ne.ShowDialog();
        }

        private void txt_filterEmployees_TextChanged(object sender, TextChangedEventArgs e)
        {
            employees_listView.Items.Clear();
            UpdateListViews();
        }

        private void employees_listView_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if (employees_listView.SelectedIndex == -1)
                return;

            _Employees.EmployeeProfile pf = new _Employees.EmployeeProfile();
            pf.c = (Employee)employees_listView.SelectedItem;
            pf.ShowDialog();
        }

        private void button1_Copy7_Click(object sender, RoutedEventArgs e)
        {
            Filter newFilter = new Filter();
            newFilter.FilterType = new Employee();
            newFilter.filters = filters;
            newFilter.ShowDialog();
        }

        public void UpdateListViews()
        {
            Dispatcher.Invoke(() =>
            {
                employees_listView.Items.Clear();
                instructors_listView.Items.Clear();
                children_listView.Items.Clear();
                courses_listView.Items.Clear();
                classes_listView.Items.Clear();

                Globals.Courses.ToList().ForEach(x =>
                {
                    if (x.Value.Name.ToLower().Contains(txt_filterCourses.Text.ToLower()))
                        courses_listView.Items.Add(x.Value);

                    for (int i = 0; i < x.Value.Classes.Count; i++)
                    {
                        if (x.Value.Name.ToLower().Contains(txt_filterClasses.Text.ToLower()))
                        {
                            var newClass = new Objects.CheckedObject() { Class = x.Value.Classes[i], Course = x.Value };
                            if (Globals.Instructors.ToList().Exists(z => z.Value.TeachingCourses.Contains(x.Value.Classes[i])))
                                newClass.Instructor = Globals.Instructors.ToList().First(z => z.Value.TeachingCourses.Contains(x.Value.Classes[i])).Value;
                            else
                                newClass.Instructor = new Instructor() { Name = "---" }; //hehe xd
                            classes_listView.Items.Add(newClass);
                        }

                    }
                });

                Globals.Instructors.ToList().ForEach(x =>
                {
                    if (x.Value.Name.ToLower().Contains(txt_filterInstructors.Text.ToLower()))
                    {
                        if (Globals.InstructorSalaries.ContainsKey(x.Key))
                        {
                            ListViewItem item = new ListViewItem();
                            item.Content = x.Value;

                            if (Globals.InstructorSalaries[x.Key].Exists(y => y.Paid == false))
                            {
                                item.Background = Brushes.LightGray;
                                x.Value.SalaryPaid = false;
                            }

                            instructors_listView.Items.Add(item);
                        }
                        else throw new Exception("There cannot exist an instructor without a salary histroy record.");
                    }
                });

                Globals.Children.ToList().ForEach(x =>
                {
                    if (x.Value.Name.ToLower().Contains(txt_filterChildren.Text.ToLower()))
                        children_listView.Items.Add(x.Value);
                });

                Globals.Employees.ToList().ForEach(x =>
                {
                    if (x.Value.Name.ToLower().Contains(txt_filterEmployees.Text.ToLower()))
                        employees_listView.Items.Add(x.Value);
                });
            });
        }

        private void button1_Copy2_Click_1(object sender, RoutedEventArgs e)
        {
            if (courses_listView.SelectedIndex == -1)
                return;

            _Courses.AddNewCourse nc = new _Courses.AddNewCourse();
            nc.EditedCourse = (Course)courses_listView.SelectedItem;
            nc.ShowDialog();
        }

        private async void button1_Copy2_Click(object sender, RoutedEventArgs e)
        {
            if (courses_listView.SelectedIndex == -1)
                return;

            if (await this.ShowMessageAsync("Deleting", "You are going to delete " + courses_listView.SelectedItems.Count + " Course are you sure?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                foreach (Course item in courses_listView.SelectedItems)
                {
                    Courses.RemoveCourse(item);
                }
                Globals.RefreshReferenceInformation();
            }
        }

        private async void button1_Copy9_Click(object sender, RoutedEventArgs e)
        {
            if (instructors_listView.SelectedIndex == -1)
                return;

            if (await this.ShowMessageAsync("Deleting", "You are going to delete " + instructors_listView.SelectedItems.Count + " Instructors are you sure?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                foreach (ListViewItem item in instructors_listView.SelectedItems)
                {
                    Instructors.RemoveInstructor((Instructor)item.Content);
                }
                Globals.RefreshReferenceInformation();
            }
        }

        private async void button1_Copy10_Click(object sender, RoutedEventArgs e)
        {
            if (children_listView.SelectedIndex == -1)
                return;

            if (await this.ShowMessageAsync("Deleting", "You are going to delete " + children_listView.SelectedItems.Count + " Children are you sure?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                foreach (Child item in children_listView.SelectedItems)
                {
                    Children.RemoveChild(item);
                }
                Globals.RefreshReferenceInformation();
            }
        }

        private async void button1_Copy11_Click(object sender, RoutedEventArgs e)
        {
            if (employees_listView.SelectedIndex == -1)
                return;

            if (await this.ShowMessageAsync("Deleting", "You are going to delete " + employees_listView.SelectedItems.Count + " Employees are you sure?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                foreach (Employee item in employees_listView.SelectedItems)
                {
                    Employees.RemoveEmployee(item);
                }
                Globals.RefreshReferenceInformation();
            }
        }

        private void courses_listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listView_instructorsHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView_instructorsHistory.SelectedIndex == -1)
                return;

            int selectedId = ((Instructor)listView_instructorsHistory.SelectedItem).Id;

            listView_salaryHistory.Items.Clear();
            if (Globals.InstructorSalaries.ContainsKey(selectedId))
                Globals.InstructorSalaries[selectedId].ForEach(x => listView_salaryHistory.Items.Add(x));
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //if (listView_unpaidInstructors.SelectedIndex == -1)
            //    return;

            //foreach (Objects.UnpaidInstructorListViewItem item in listView_unpaidInstructors.SelectedItems)
            //{
            //    if (await this.ShowMessageAsync("Payment", "You're going to pay " + item.Salary + " to " + Globals.Instructors[item.Instructor.Id].Name, MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            //    {
            //        Instructors.PayInstructorSalary(item.Instructor.Id, item.Course.Id);
            //    }
            //}

            //Globals.RefreshReferenceInformation();
        }

        private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (classes_listView.SelectedIndex == -1)
            {
                await this.ShowMessageAsync("Error!", "Select at least 1 class to show shifts manager", MessageDialogStyle.Affirmative);
                return;
            }

            ShiftsManager sm = new ShiftsManager();
            foreach (Objects.CheckedObject v in classes_listView.SelectedItems)
                sm.affectedCourses.Add(v.Class);
            sm.ShowDialog();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            _Classes.AddNewClass nc = new _Classes.AddNewClass();
            nc.ShowDialog();
        }

        private void button1_Copy12_Click(object sender, RoutedEventArgs e)
        {
            if (classes_listView.SelectedIndex == -1)
                return;

            _Classes.AddNewClass nc = new _Classes.AddNewClass();
            nc.EditedClass = ((Objects.CheckedObject)classes_listView.SelectedItem).Class;
            nc.ShowDialog();

        }

        private void classes_listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (classes_listView.SelectedIndex == -1)
                return;

            _Classes.ClassProfile nc = new _Classes.ClassProfile();
            nc.c = ((Objects.CheckedObject)classes_listView.SelectedItem).Class;
            nc.ShowDialog();
        }

        private async void button1_Copy14_Click(object sender, RoutedEventArgs e)
        {
            if (classes_listView.SelectedIndex == -1)
                return;

            if (await this.ShowMessageAsync("Deleting", "You are going to delete " + classes_listView.SelectedItems.Count + " Class are you sure?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                foreach (Objects.CheckedObject item in classes_listView.SelectedItems)
                {
                    Classes.RemoveCourseClass(item.Class);
                }
                Globals.RefreshReferenceInformation();
            }
        }

        private void button1_Copy14_Click_1(object sender, RoutedEventArgs e)
        {
            Filter newFilter = new Filter();
            newFilter.FilterType = new Class();
            newFilter.filters = filters;
            newFilter.ShowDialog();
        }

        public async void StartConnection()
        {
            await Connection.Connect();

            Globals.RefreshReferenceInformation();

            coursesRefreshTimer = new Timer(500);
            coursesRefreshTimer.Elapsed += CoursesRefreshTimer_Elapsed;
            coursesRefreshTimer.Start();

            //mainRefreshTimer = new Timer(5000);
            //mainRefreshTimer.Elapsed += MainRefreshTimer_Elapsed;
            //mainRefreshTimer.Start();

            //MainRefreshTimer_Elapsed(null, null);
        }

        private async void menu_salaryHistory_Click(object sender, RoutedEventArgs e)
        {
            if (instructors_listView.SelectedIndex == -1 || instructors_listView.SelectedItems.Count > 1)
            {
                await this.ShowMessageAsync("Error", "Please make sure to select one instructor!");
                return;
            }

            var ins = (Instructor)((ListViewItem)instructors_listView.SelectedItem).Content;

            if (Globals.InstructorSalaries[ins.Id].Count == 0)
            {
                await this.ShowMessageAsync("Warning", "There's no salary history for this instructor.");
                return;
            }

            SalaryHistory win = new SalaryHistory();
            win.c = ins;
            win.ShowDialog();

        }

        private void instructors_listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Connection.Disconnect();
            Application.Current.Shutdown();
        }
    }
}
