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

namespace DrNadaTreasureLand.Windows
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter
    {

        public object FilterType;
        public List<Objects.Filter> filters;

        public Filter()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var item in FilterType.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(string) || item.PropertyType == typeof(int))
                {
                    cmb_filterName.Items.Add(item.Name);
                }
            }

            foreach(var item in filters)
            {
                filter_listView.Items.Add(item);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = new Objects.Filter()
            {
                Filtered = FilterType.GetType().Name,
                Property = cmb_filterName.Text,
                FilterType = cmb_filterType.Text,
                Data = txt_filterExpression.Text,
                Action = cmb_filterAction.Text,
            };

            if (!filter_listView.Items.Contains(item))
            {
                filter_listView.Items.Add(item);
                filters.Add(item);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (filter_listView.SelectedIndex == -1)
                return;

            filters.Remove((Objects.Filter)filter_listView.SelectedItem);
            filter_listView.Items.RemoveAt(filter_listView.SelectedIndex);
        }
    }
}
