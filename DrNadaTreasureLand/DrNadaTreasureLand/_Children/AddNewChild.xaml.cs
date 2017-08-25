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
using MahApps.Metro.Controls;
using TreasuresLand.Objects;
using TreasuresLand.SQL;
using DrNadaTreasureLand.Objects;
using MahApps.Metro.Controls.Dialogs;

namespace DrNadaTreasureLand._Children
{
    /// <summary>
    /// Interaction logic for AddNewChild.xaml
    /// </summary>
    public partial class AddNewChild
    {

        public Child EditedChild;

        public AddNewChild()
        {
            InitializeComponent();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox_Copy9_Checked(object sender, RoutedEventArgs e)
        {

        }

        private string CheckPhoneNumber(TextBox txt)
        {
            foreach (char letter in txt.Text)
            {
                if (!char.IsDigit(letter))
                {
                    return "Phone can only consist of numbers.\n";
                }
            }
            return "";
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            string errorList = "";

            if (cmb_childAcademicType.SelectedIndex == -1)
                errorList += "Choose an academic type.\n";
            if (cmb_childBirthOrder.SelectedIndex == -1)
                errorList += "Choose child birth order.\n";
            if (cmb_childEducationType.SelectedIndex == -1)
                errorList += "Choose the education type.\n";
            if (cmb_childGender.SelectedIndex == -1)
                errorList += "Choose a gender.\n";
            if (txt_childName.Text == "")
                errorList += "Child name cannot be empty.\n";
            if (txt_whatsAppPhone.Text == "")
                errorList += "WhatsApp Number cannot be empty.\n";
            if (num_childAcademicYear.Value == null || num_childAge == null)
                errorList += "Numeric values cannot be null.\n";
            if (txt_guardianName.Text == "")
                errorList += "Guardian Name cannot be empty!\n";
            if (num_childAcademicYear != null && num_childAcademicYear.Value >= 10)
                errorList += "Maximum value for allowed academic year is 10";

            errorList += CheckPhoneNumber(txt_whatsAppPhone);
            if (!errorList.Contains("Phone can only consist of numbers."))
                errorList += CheckPhoneNumber(txt_fatherPhone);
            if (!errorList.Contains("Phone can only consist of numbers."))
                errorList += CheckPhoneNumber(txt_motherPhone);

            if (errorList != "")
            {
                await this.ShowMessageAsync("Check the following!", errorList, MessageDialogStyle.Affirmative);
                return;
            }

            if (Globals.Children.ToList().Exists(x => x.Value.Name == txt_childName.Text || x.Value.WhatsAppPhone == txt_whatsAppPhone.Text))
            {
                if (EditedChild == null || (EditedChild != null && EditedChild.Name != txt_childName.Text))
                {
                    if (await this.ShowMessageAsync("Are you sure", "There exist a child with the same name/whatsApp number\nAre you sure you want to continue", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
                    {
                        return;
                    }
                }
            }

            Child newChild = new Child()
            {
                Name = txt_childName.Text,
                Age = Convert.ToInt32(num_childAge.Value),
                GuardianName = txt_guardianName.Text,
                GuardianType = (rb_mother.IsChecked == true) ? 0 : 1,
                FatherPhone = txt_fatherPhone.Text,
                MotherPhone = txt_motherPhone.Text,
                FatherJob = txt_fatherJob.Text,
                MotherJob = txt_motherJob.Text,
                WhatsAppPhone = txt_whatsAppPhone.Text,
                MotherQualifier = txt_motherQualification.Text,
                Gender = cmb_childGender.SelectedIndex,
                AcademicYear = (cmb_childAcademicType.SelectedIndex * 10) + (Convert.ToInt32(num_childAcademicYear.Value)),
                EducationType = cmb_childEducationType.SelectedIndex,
                ChildBirthOrder = cmb_childBirthOrder.SelectedIndex,
                ChildTraits = CalcBits(chk_traits_social, chk_traits_leading, chk_traits_edgy, chk_traits_cooperative, chk_traits_goodSpeaker),
                ChildHandling = CalcBits(chk_problem_askHelp, chk_problem_worries, chk_problem_leaveProblem, chk_problem_solveProblem),
                ChildFreeTime = CalcBits(chk_freeTime_drawing, chk_freeTime_games, chk_freeTime_tv, chk_freeTime_handwork),
                RegisteredCourses = new List<Class>(),
            };

            foreach (CheckedObject val in courses_listView.Items)
            {
                if (val.Checked)
                    newChild.RegisteredCourses.Add(val.Class);
            }

            try
            {
                if (EditedChild == null)
                    Children.AddChild(newChild);
                else
                {
                    newChild.Id = EditedChild.Id;
                    Children.EditChild(newChild);
                }

                //int coursesCost = 0;
                //for(int i = 0; i < newChild.RegisteredCourses.Count; i++)
                //{
                //    if (i == 0) //100% of first course
                //        coursesCost += Globals.Courses[newChild.RegisteredCourses[i].CourseId].Cost;
                //    else        //50% of every course after
                //        coursesCost += Globals.Courses[newChild.RegisteredCourses[i].CourseId].Cost/2;
                //}
                //await this.ShowMessageAsync("Courses Price", "Total price for chosen courses are: " + coursesCost + " EGP.");

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Globals.RefreshReferenceInformation();
            this.Close();
        }

        private string CalcBits(params CheckBox[] text)
        {
            string ret = "";
            
            foreach(var item in text)
            {
                if (item.IsChecked == true)
                {
                    ret = ret + "1";
                } else
                {
                    ret = ret + "0";
                }
            }

            return ret;
        }

        private void EvalBits(string text, params CheckBox[] chks)
        {
            if (text.Length != chks.Count())
                throw new Exception("Cannot evaluate check boxes with bits of different length!");

            for(int i = 0; i < text.Length; i++)
            {
                if (text[i] == '1')
                    chks[i].IsChecked = true;
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            num_childAcademicYear.Value = 1;
            num_childAcademicYear.Minimum = 1;
            num_childAcademicYear.Maximum = 9;
            num_childAge.Value = 1;
            num_childAge.Minimum = 1;
            num_childAge.Maximum = 100;

            if (EditedChild != null)
            {
                this.Title = "Editing " + EditedChild.Name;

                txt_childName.Text = EditedChild.Name;
                txt_fatherJob.Text = EditedChild.FatherJob;
                txt_fatherPhone.Text = EditedChild.FatherPhone;
                txt_guardianName.Text = EditedChild.GuardianName;
                txt_motherJob.Text = EditedChild.MotherJob;
                txt_motherPhone.Text = EditedChild.MotherPhone;
                txt_motherQualification.Text = EditedChild.MotherQualifier;
                txt_whatsAppPhone.Text = EditedChild.WhatsAppPhone;
                num_childAge.Value = EditedChild.Age;
                cmb_childGender.SelectedIndex = EditedChild.Gender;
                cmb_childBirthOrder.SelectedIndex = EditedChild.ChildBirthOrder;
                cmb_childEducationType.SelectedIndex = EditedChild.EducationType;
                cmb_childAcademicType.SelectedIndex = EditedChild.AcademicYear / 10;
                num_childAcademicYear.Value = ((double)EditedChild.AcademicYear / 10.0 - (EditedChild.AcademicYear / 10)) * 10;
                rb_father.IsChecked = (EditedChild.GuardianType == 1);
                EvalBits(EditedChild.ChildTraits, chk_traits_social, chk_traits_leading, chk_traits_edgy, chk_traits_cooperative, chk_traits_goodSpeaker);
                EvalBits(EditedChild.ChildHandling, chk_problem_askHelp, chk_problem_worries, chk_problem_leaveProblem, chk_problem_solveProblem);
                EvalBits(EditedChild.ChildFreeTime, chk_freeTime_drawing, chk_freeTime_games, chk_freeTime_tv, chk_freeTime_handwork);
                btn_actionChild.Content = "Edit Child";

                btn_delete.Visibility = Visibility.Visible;
            }

            UpdateCoursesListView();
        }

        private void UpdateCoursesListView()
        {
            try
            {
                courses_listView.Items.Clear();
                Globals.Courses.ToList().ForEach(x =>
                {
                    for (int i = 0; i < x.Value.Classes.Count; i++)
                    {
                        var item = x.Value.Classes[i];
                        if ((!item.Full && !item.Over) || (EditedChild != null && EditedChild.RegisteredCourses.Contains(item)))
                        {
                            if (num_childAge.Value >= x.Value.AcademicYearStart && num_childAge.Value <= x.Value.AcademicYearEnd)
                            {
                                var newAvailableCourse = new CheckedObject()
                                {
                                    Checked = false,
                                    Class = item,
                                    Course = x.Value,
                                    Instructor = Globals.Instructors.Single(z => z.Value.TeachingCourses.Contains(item)).Value,
                                };

                                if (EditedChild != null && EditedChild.RegisteredCourses.Contains(item))
                                    newAvailableCourse.Checked = true;

                                courses_listView.Items.Add(newAvailableCourse);
                            }
                        }
                    }
                });
            } catch (Exception ex) { }
        }

        private async void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync("Are you sure", "You're deleting a child, are you sure you want to do that?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                Children.RemoveChild(EditedChild);
                Globals.RefreshReferenceInformation();
                this.Close();
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Globals.ClearForm(this);
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void num_childAge_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            UpdateCoursesListView();
        }
    }
}
