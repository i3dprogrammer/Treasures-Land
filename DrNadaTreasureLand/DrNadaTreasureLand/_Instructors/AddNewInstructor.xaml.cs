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
using DrNadaTreasureLand.Objects;
using Microsoft.Win32;
using MahApps.Metro.Controls.Dialogs;

namespace DrNadaTreasureLand._Instructors
{
    /// <summary>
    /// Interaction logic for AddNewCourse.xaml
    /// </summary>
    public partial class AddNewInstructor
    {
        public Instructor EditedInstructor;

        public AddNewInstructor()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                num_age.Value = 1;
                num_age.Minimum = 1;
                num_age.Maximum = 100;

                if (EditedInstructor == null)
                {
                    btn_clear.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Title = "Editing " + EditedInstructor.Name;
                    txt_address.Text = EditedInstructor.Address;
                    txt_instructorName.Text = EditedInstructor.Name;
                    txt_phone.Text = EditedInstructor.Phone;
                    txt_qualifier.Text = EditedInstructor.Qualifier;
                    cmb_gender.SelectedIndex = EditedInstructor.Gender;
                    num_age.Value = EditedInstructor.Age;
                    if (EditedInstructor.CV != null)
                    {
                        System.IO.File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "\\tempCV.pdf", EditedInstructor.CV);
                        lblBlock_CV.Text = System.IO.Directory.GetCurrentDirectory() + "\\tempCV.pdf";
                    }
                    btn_delete.Visibility = Visibility.Visible;
                    btn_addInstructor.Content = "Edit Instructor";
                }

                //Globals.Courses.ToList().ForEach(x =>
                //{
                //    for(int i = 0; i < x.Value.Classes.Count; i++)
                //    {
                //        var item = x.Value.Classes[i];
                //        if (!item.Over)
                //        {
                //            var newAvailableCourse = new CheckedObject()
                //            {
                //                Checked = false,
                //                Class = item,
                //                Course = x.Value,
                //                Number = i + 1,
                //            };

                //            if (EditedInstructor != null && EditedInstructor.TeachingCourses.Contains(item))
                //                newAvailableCourse.Checked = true;

                //            courses_listView.Items.Add(newAvailableCourse);
                //        }
                //    }
                //});
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PDF Files|*.pdf";
            if(ofd.ShowDialog() == true)
            {
                lblBlock_CV.Text = ofd.FileName;
            }
        }

        private async void btn_addInstructor_Click(object sender, RoutedEventArgs e)
        {
            string errorList = "";

            if (cmb_gender.SelectedIndex == -1)
                errorList += "Select a gender.\n";
            if (lblBlock_CV.Text != "...." && !System.IO.File.Exists(lblBlock_CV.Text))
                errorList += "The selected CV does not exist.\n";
            if (num_age.Value == null)
                errorList += "Numeric values cannot be null.\n";
            if (txt_address.Text == "")
                errorList += "Address cannot be empty.\n";
            if (txt_instructorName.Text == "")
                errorList += "Instructor name cannot be empty.\n";
            if (txt_qualifier.Text == "")
                errorList += "Qualifier cannot be empty.\n";
            if (txt_phone.Text == "")
                errorList += "Phone field cannot be empty.\n";

            foreach(char letter in txt_phone.Text)
            {
                if (!char.IsDigit(letter))
                {
                    errorList += "Phone can only consist of numbers!\n";
                    break;
                }
            }

            if(errorList != "")
            {
                await this.ShowMessageAsync("Check the following!", errorList);
                return;
            }

            if(Globals.Instructors.ToList().Exists(x => x.Value.Name == txt_instructorName.Text || x.Value.Phone == txt_phone.Text))
            {
                if (EditedInstructor == null || (EditedInstructor != null && EditedInstructor.Name != txt_instructorName.Text))
                {
                    if (await this.ShowMessageAsync("Are you sure", "There exist an instructor with the same name/phone, are you sure you want to continue", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Negative)
                    {
                        return;
                    }
                }
            }

            try
            {
                var newIns = new Instructor()
                {
                    Name = txt_instructorName.Text,
                    Address = txt_address.Text,
                    Phone = txt_phone.Text,
                    Gender = cmb_gender.SelectedIndex,
                    Qualifier = txt_qualifier.Text,
                    Age = Convert.ToInt32(num_age.Value),
                };

                if (System.IO.File.Exists(lblBlock_CV.Text))
                    newIns.CV = System.IO.File.ReadAllBytes(lblBlock_CV.Text);

                //foreach (CheckedObject item in courses_listView.Items)
                //{
                //    if (item.Checked)
                //    {
                //        newIns.TeachingCourses.Add(item.Class);
                //    }
                //}

                if(EditedInstructor == null)
                    Instructors.AddInstructor(newIns);
                else {
                    newIns.Id = EditedInstructor.Id;
                    Instructors.EditInstructor(newIns);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Globals.RefreshReferenceInformation();
        }

        private async void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShowMessageAsync("Are you sure", "You're deleting an instructor, are you sure you want to do that?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                Instructors.RemoveInstructor(EditedInstructor);
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
