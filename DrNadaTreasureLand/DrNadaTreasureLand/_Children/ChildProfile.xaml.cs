using System;
using System.Collections.Generic;
using System.IO;
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

namespace DrNadaTreasureLand._Children
{
    /// <summary>
    /// Interaction logic for ChildProfile.xaml
    /// </summary>
    public partial class ChildProfile
    {
        public Child c;

        public ChildProfile()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (c is null)
                return;

            this.Title = c.Name + " Profile";
            lbl_name.Content = c.Name + "";
            lbl_age.Content = c.Age + " Years Old";
            lbl_fatherJob.Content = c.FatherJob + "";
            lbl_fatherPhone.Content = c.FatherPhone + "";
            lbl_motherJob.Content = c.MotherJob + "";
            lbl_motherPhone.Content = c.MotherPhone + "";
            lbl_motherQualifier.Content = c.MotherQualifier + "";
            lbl_whatsApp.Content = c.WhatsAppPhone + "";
            lbl_gender.Content = (c.Gender == 0) ? "Male" : "Female";

            if (c.ChildBirthOrder == 0)
                lbl_birthOrder.Content = "Lonely Child";
            else if (c.ChildBirthOrder == 1)
                lbl_birthOrder.Content = "Youngest Child";
            else if (c.ChildBirthOrder == 2)
                lbl_birthOrder.Content = "Middle Child";
            else
                lbl_birthOrder.Content = "Oldest Child";

            lbl_guardian.Content = c.GuardianName + ((c.GuardianType == 0) ? " (Mother)" : " (Father)");

            if (c.EducationType == 0)
                lbl_educationType.Content = "Governmental Arabic";
            else if (c.EducationType == 1)
                lbl_educationType.Content = "Governmental Experimental";
            else if (c.EducationType == 2)
                lbl_educationType.Content = "Private - Arabic";
            else
                lbl_educationType.Content = "Private - Other Languages";

            int academyType = c.AcademicYear / 10;
            int academyYear = (int)(((double)c.AcademicYear / 10.0 - (c.AcademicYear / 10)) * 10);

            if (academyType == 0)
                lbl_academicYear.Content = "Elementary School - Year " + academyYear;
            else if (academyType == 1)
                lbl_academicYear.Content = "Prep School - Year " + academyYear;
            else if (academyType == 2)
                lbl_academicYear.Content = "High School - Year " + academyYear;

            lbl_traits.Text = DecipherBits(c.ChildTraits, new string[] { "Social", "Leading", "Edgy", "Cooperative", "Good Speaker" });
            lbl_problems.Text = DecipherBits(c.ChildHandling, new string[] { "Asks for help", "Worries and get angry", "Leaves the problem totally", "Tries to solve the problem by himself" });
            lbl_freeTime.Text = DecipherBits(c.ChildFreeTime, new string[] { "Drawing/Coloring", "Eletronic Games", "Watching TV", "Handwork" });

            int cost = 0;

            c.RegisteredCourses.ForEach(x => {
                var course = Globals.Courses.First(y => y.Value.Classes.Contains(x)).Value;
                cost += course.Cost;
                courses_listView.Items.Add(new Objects.CheckedObject() { Class = x, Course = course });
            });

            lbl_cost.Content = cost;

        }

        private string DecipherBits(string val, params string[] info)
        {
            if (val.Length != info.Length)
                throw new Exception("Cannot decipher elements of different length!");
            string data = "";

            for(int i = 0; i < val.Length; i++)
            {
                if (val[i] == '1')
                    data += ", " + info[i];
            }

            if (data == "")
                data = "...";
            else if (data.StartsWith(","))
                data = data.Remove(0, 2);

            return data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
