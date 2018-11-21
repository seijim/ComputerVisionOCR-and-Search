using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Net.Http;


namespace WpfAppCvSearch
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        readonly string cSearchSchemaFile = "/Resources/qcdocs.schema.json";
        readonly string cSuccess = "success";
        Uri ServiceUri = null;

        public SettingWindow()
        {
            InitializeComponent();

            if ((string)Properties.Settings.Default["VisionApiKey"] != string.Empty)
            {
                TextBoxVisionApiUrl.Text = (string)Properties.Settings.Default["VisionApiUri"];
                TextBoxVisionApiKey.Text = (string)Properties.Settings.Default["VisionApiKey"];
            }

            if ((string)Properties.Settings.Default["SearchServiceApiKey"] != string.Empty)
            {
                TextBoxSearchServiceName.Text = (string)Properties.Settings.Default["SearchServiceName"];
                TextBoxSearchServiceApiKey.Text = (string)Properties.Settings.Default["SearchServiceApiKey"];
                TextBoxSearchIndexName.Text = (string)Properties.Settings.Default["SearchIndexName"];
            }

            TextBoxStorageAccountName.Text = (string)Properties.Settings.Default["StorageAccountName"];
            TextBoxStorageAccountKey.Text = (string)Properties.Settings.Default["StorageAccountKey"];
            TextBoxStorageContainerName.Text = (string)Properties.Settings.Default["StorageContainerName"];

            // Azure Search Service Uri
            if (TextBoxSearchServiceName.Text != string.Empty)
                ServiceUri = new Uri("https://" + TextBoxSearchServiceName.Text + ".search.windows.net");
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["VisionApiUri"] = TextBoxVisionApiUrl.Text.Trim();
            Properties.Settings.Default["VisionApiKey"] = TextBoxVisionApiKey.Text.Trim();
            Properties.Settings.Default["SearchServiceName"] = TextBoxSearchServiceName.Text.Trim();
            Properties.Settings.Default["SearchServiceApiKey"] = TextBoxSearchServiceApiKey.Text.Trim();
            Properties.Settings.Default["SearchIndexName"] = TextBoxSearchIndexName.Text.Trim();
            Properties.Settings.Default["StorageAccountName"] = TextBoxStorageAccountName.Text.Trim();
            Properties.Settings.Default["StorageAccountKey"] = TextBoxStorageAccountKey.Text.Trim();
            Properties.Settings.Default["StorageContainerName"] = TextBoxStorageContainerName.Text.Trim();
            Properties.Settings.Default.Save();

            // Create Azure Table Storage
            var tagTable = new TagTableHelper(Utils.GetStorageConnectionString(), "");
            try
            {
                tagTable.CreateLogTableIfNotExists();
            }
            catch
            {
            }

            this.Close();
        }

        private void ButtonCreateSearchIndex_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceUri == null)
            {
                MessageBox.Show("SearchServiceName に無効な値が設定されています", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Read resouce file (Search Schema Json file)
            Uri fileUri = new Uri(cSearchSchemaFile, UriKind.Relative);
            var info = Application.GetResourceStream(fileUri);
            var stream = info.Stream;
            var reader = new StreamReader(stream);
            string searchSchemaJson = reader.ReadToEnd();

            // Delete & Create Search Index
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("api-key", TextBoxSearchServiceApiKey.Text);

            DeleteIndex(httpClient, TextBoxSearchIndexName.Text.Trim());
            string result = CreateTargetIndex(httpClient, TextBoxSearchIndexName.Text.Trim(), searchSchemaJson);
            if (result == cSuccess)
            {
                MessageBox.Show($"検索インデックス ({TextBoxSearchIndexName.Text.Trim()}) の作成に成功しました", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"検索インデックス ({TextBoxSearchIndexName.Text.Trim()}) の作成に失敗しました : {result}", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string DeleteIndex(HttpClient httpClient, string IndexName)
        {
            try
            {
                try
                {
                    var uri = new Uri(ServiceUri, "/indexes/" + IndexName);
                    HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(httpClient, HttpMethod.Delete, uri);
                    response.EnsureSuccessStatusCode();
                    return cSuccess;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string CreateTargetIndex(HttpClient httpClient, string IndexName, string jsonBody)
        {
            try
            {
                var uri = new Uri(ServiceUri, "/indexes");
                HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(httpClient, HttpMethod.Post, uri, jsonBody);
                response.EnsureSuccessStatusCode();
                return cSuccess;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }
    }
}
