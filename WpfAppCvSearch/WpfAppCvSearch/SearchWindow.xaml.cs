using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAzure.Storage;
using System.IO;

namespace WpfAppCvSearch
{
    /// <summary>
    /// SearchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window
    {
        private static int totalPages = 1;
        private static long? foundRows;
        private static bool searchProcessing = false;

        public SearchWindow()
        {
            InitializeComponent();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ButtonSearch.IsEnabled = false;
            searchProcessing = true;

            int currentPage;

            if (TextBoxCurrentPage.Text == string.Empty)
            {
                currentPage = 1;
                TextBoxCurrentPage.Text = currentPage.ToString();
            }
            else
            {
                int i;
                if (int.TryParse(TextBoxCurrentPage.Text, out i))
                    currentPage = i;
                else
                    currentPage = 1;

                if (currentPage > totalPages)
                    currentPage = totalPages;

                TextBoxCurrentPage.Text = currentPage.ToString();
            }

            try
            {
                AzureDocumentSearch docSearch = new AzureDocumentSearch();
                var result = docSearch.Search(TextBoxInput.Text, "", "", "", "", "", "", currentPage);
                foundRows = result.Count;
                if (foundRows != null)
                {
                    decimal pages = (decimal)((long)foundRows) / Utils.GetDefaultPageSize();
                    totalPages = (int)Math.Ceiling(pages);
                }
                else
                {
                    totalPages = 1;
                }
                LabelTotalPages.Content = $"/ {totalPages}";

                var qcdocViewList = new List<QcDocumentView>();
                foreach (var row in result.Results)
                {
                    var document = row.Document;
                    var qcDocView = new QcDocumentView();
                    qcDocView.id = (string)document["id"];
                    qcDocView.document_type = (string)document["document_type"];
                    qcDocView.project_name = (string)document["project_name"];
                    qcDocView.project_location = (string)document["project_location"];
                    qcDocView.blob_path = (string)document["blob_path"];
                    qcDocView.ocr_content = (string)document["ocr_content"];
                    var dto1 = (DateTimeOffset)document["posting_date"];
                    qcDocView.posting_date = dto1.LocalDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                    var dto2 = (DateTimeOffset)document["posting_updated"];
                    qcDocView.posting_updated = dto2.LocalDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                    qcDocView.tags = string.Empty;
                    var taglist = (string[])document["tags"];
                    foreach (var tag in taglist)
                    {
                        qcDocView.tags += tag + ", ";
                    }
                    if (taglist.Length > 0)
                        qcDocView.tags = qcDocView.tags.Substring(0, qcDocView.tags.Length - 2);

                    qcdocViewList.Add(qcDocView);
                }

                DataGridResult.ItemsSource = qcdocViewList;
                TextBoxStatusMessage.Text = $"見つかった件数：{foundRows}";
            }
            catch(Exception ex)
            {
                MessageBox.Show($"検索実行時にエラーが発生しました : {ex.ToString()}", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ButtonSearch.IsEnabled = true;
            searchProcessing = false;

        }

        private void DataGridResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchProcessing)
                return;

            // Download a blob file to local disk and display the image.
            try
            {
                var doc = (QcDocumentView)DataGridResult.SelectedItems[0];
                string currentPath = Directory.GetCurrentDirectory();
                string currentTempPath = currentPath + "\\temp";
                if (!Directory.Exists(currentTempPath))
                {
                    Directory.CreateDirectory(currentTempPath);
                }

                string fileName = doc.blob_path.Replace("/", "-");
                string filePath = currentTempPath + "\\" + fileName;

                string connectionString = Utils.GetStorageConnectionString();
                var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var storageContainerName = Utils.GetStorageContainerName();
                var container = cloudBlobClient.GetContainerReference(storageContainerName);
                var blockBlob = container.GetBlockBlobReference(doc.blob_path);
                blockBlob.DownloadToFile(filePath, FileMode.OpenOrCreate);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                FileStream fs2 = File.OpenRead(filePath);
                bitmapImage.StreamSource = fs2;
                bitmapImage.EndInit();
                fs2.Close();

                ImagePreview.Source = bitmapImage;
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Blob Storage からのデータ取得に失敗しました : {ex.ToString()}", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
