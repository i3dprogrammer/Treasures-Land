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
using MahApps.Metro.Controls.Dialogs;
using TreasuresLand.Objects;
using TreasuresLand.SQL;

namespace DrNadaTreasureLand
{
    /// <summary>
    /// Interaction logic for ShiftsManager.xaml
    /// </summary>
    public partial class ShiftsManager
    {

        private class DayAndDate
        {
            public string Day { get; set; }
            public DateTime Date { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || this.GetType() != obj.GetType())
                    return false;

                DayAndDate other = (DayAndDate)obj;

                return (other.Day == this.Day && other.Date == this.Date);
            }
        }

        public List<Class> affectedCourses = new List<Class>();

        public ShiftsManager()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (date_excluded.SelectedDate == null)
                return;

            var val = new DayAndDate() { Date = date_excluded.SelectedDate.Value, Day = ((DayOfWeek)date_excluded.SelectedDate.Value.DayOfWeek).ToString() };

            if (!listView_excluded.Items.Contains(val))
                listView_excluded.Items.Add(val);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (listView_excluded.SelectedIndex == -1) //Redundant
                return;
            try
            {
                var x = new List<DayAndDate>();

                foreach (DayAndDate item in listView_excluded.SelectedItems)
                {
                    x.Add(item);
                }

                foreach (var item in x)
                    listView_excluded.Items.Remove(item);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (affectedCourses.Count == 1)
                Classes.ClearClassShiftedDates(affectedCourses[0].Id);

            foreach (Class item in affectedCourses)
            {
                foreach (DayAndDate dad in listView_excluded.Items)
                {
                    if (!item.ShiftedDates.Contains(dad.Date) && item.DaysPerWeek.Contains(dad.Day))
                        Classes.AddClassShiftedDate(item.Id, dad.Date);
                }
            }

            Globals.RefreshReferenceInformation();
            this.Close();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var item in affectedCourses)
            {
                if(item.StartDate == null || item.EndDate == null)
                {
                    await this.ShowMessageAsync("", "Cannot SHIFT classes with no Start Date.");
                    Close();
                }

                if(item.EndDate != null && item.EndDate <= DateTime.Now)
                {
                    await this.ShowMessageAsync("", "Cannot SHIFT, some classes are already over.");
                    Close();
                }
            }

            if(affectedCourses.Count == 1)
            {
                foreach (var item in affectedCourses[0].ShiftedDates)
                {
                    var val = new DayAndDate() { Date = item, Day = ((DayOfWeek)item.DayOfWeek).ToString() };
                    listView_excluded.Items.Add(val);
                }
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("", "This adds 7 days(Counting today) to the list.");
            var date = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                var item = new DayAndDate() { Date = date, Day = ((DayOfWeek)date.DayOfWeek).ToString() };
                if(!listView_excluded.Items.Contains(item))
                    listView_excluded.Items.Add(item);
                date = date.AddDays(1);
            }
        }
    }
}
