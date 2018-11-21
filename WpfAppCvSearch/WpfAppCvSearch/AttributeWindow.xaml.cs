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

namespace WpfAppCvSearch
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class AttributeWindow : Window
    {
        public AttributeWindow()
        {
            InitializeComponent();

            TextBoxPostingDate.Text = DateTime.Now.ToString();
            MainWindow.attrebutesInputResult = new AttrebutesInputResult();
            MainWindow.attrebutesInputResult.IsSuccess = false;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.attrebutesInputResult.IsSuccess = false;
            this.Close();
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxProjectName.SelectedItem == null)
            {
                MessageBox.Show("プロジェクト名が選択されていません", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ComboBoxProjectLocation.SelectedItem ==  null)
            {
                MessageBox.Show("プロジェクトの場所が選択されていません", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (TextBoxPostingDate.Text == string.Empty)
            {
                MessageBox.Show("登録日時が入力されていません", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                try
                {
                    DateTime.Parse(TextBoxPostingDate.Text);
                }
                catch
                {
                    MessageBox.Show("登録日時のフォーマットが無効です", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var doc = new QcDocument();
            doc.project_name = ComboBoxProjectName.Text;
            doc.project_location = ComboBoxProjectLocation.Text;
            doc.posting_date = new DateTimeOffset(DateTime.Parse(TextBoxPostingDate.Text));
            doc.posting_yearmonth = doc.posting_date.ToString("yyyyMM");

            MainWindow.attrebutesInputResult.Document = doc;
            MainWindow.attrebutesInputResult.IsSuccess = true;
            this.Close();
        }
    }
}
