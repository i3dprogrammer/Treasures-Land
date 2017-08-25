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

namespace DrNadaTreasureLand._Classes
{
    /// <summary>
    /// Interaction logic for ClassProfile.xaml
    /// </summary>
    public partial class ClassProfile
    {
        public Class c;

        public ClassProfile()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_courseName.Content = Globals.Courses[c.CourseId].Name;
            lbl_instructoName.Content = Globals.Instructors.ToList().First(x => x.Value.TeachingCourses.Contains(c)).Value.Name;
            lbl_startDate.Content = c.StartDate;
            lbl_over.Content = (c.Full)?"Full" : "Vacant";
            lbl_complete.Content = (c.Over) ? "Completed" : "Incompleted";
            txt_days.Text = c.DaysPerWeek.Aggregate((x, y) => x + ", " + y) + "\n---";
            Globals.Children.ToList().Where(x => x.Value.RegisteredCourses.Contains(c)).ToList().ForEach(y => listView_children.Items.Add(y.Value));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog().GetValueOrDefault(false))
            {
                this.Height -= 35;
                pd.PrintVisual(this, this.Title);
                this.Height += 35;
            }
        }
    }
}
