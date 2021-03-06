﻿using System;
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

namespace DrNadaTreasureLand._Courses
{
    /// <summary>
    /// Interaction logic for CourseProfile.xaml
    /// </summary>
    public partial class CourseProfile
    {

        public Course c;

        public string CashType = "EGP.";

        public CourseProfile()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog().GetValueOrDefault(false))
                {
                    this.Height -= 35;
                    pd.PrintVisual(this, this.Title);
                    this.Height += 35;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Title = c.Name + " Properties";
                lbl_courseName.Content = c.Name;
                lbl_cost.Content = c.Cost + " (" + CashType + ")";
                lbl_from.Content = c.AcademicYearStart + " Years old";
                lbl_to.Content = c.AcademicYearEnd + " Years old";
                lbl_level.Content = c.Level;
                lbl_totalSessions.Content = c.TotalSessions + " Days";
                lbl_salaryPerChild.Content = c.PricePerChild + "%";

                lbl_sessionHours.Content = c.SessionDuration + " Hours";

                foreach (var item in (Globals.Children.ToList().Where(x => x.Value.RegisteredCourses.Exists(y => y.CourseId == c.Id))))
                {
                    listView_children.Items.Add(item.Value);
                }

                for (int i = 0; i < c.Classes.Count; i++)
                {
                    var aClass = new Objects.CheckedObject();
                    aClass.Course = c;
                    aClass.Class = c.Classes[i];
                    if (Globals.Instructors.ToList().Exists(x => x.Value.TeachingCourses.Contains(c.Classes[i])))
                        aClass.Instructor = Globals.Instructors.ToList().Single(x => x.Value.TeachingCourses.Contains(c.Classes[i])).Value;
                    else
                        aClass.Instructor = new Instructor() { Name = "---" }; //hehe xd
                    listView_sections.Items.Add(aClass);
                }

                lbl_totalChildren.Content = listView_children.Items.Count;

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }
    }
}
