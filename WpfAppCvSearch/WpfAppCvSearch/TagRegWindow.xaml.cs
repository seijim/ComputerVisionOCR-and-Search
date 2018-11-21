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
using System.Data;

namespace WpfAppCvSearch
{
    /// <summary>
    /// TagRegWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TagRegWindow : Window
    {
        private static TagTableHelper tagTableHeler;
        private static DataTable dataTable;

        public TagRegWindow()
        {
            InitializeComponent();

            tagTableHeler = new TagTableHelper(Utils.GetStorageConnectionString(), "");
            dataTable = tagTableHeler.GetTableDisplayData();
            DataGridTags.ItemsSource = dataTable.DefaultView;
        }

        private void ButtonResisterTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tagTableHeler.BulkUpdateTable(dataTable, TableMode.Display);
                dataTable = tagTableHeler.GetTableDisplayData();
                DataGridTags.ItemsSource = dataTable.DefaultView;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"エラーが発生しました。詳細=> {ex.ToString()}", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
