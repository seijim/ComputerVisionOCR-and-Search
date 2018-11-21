using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using System.Windows.Threading;
using System.Data;

namespace WpfAppCvSearch
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private static SettingWindow settingWindow;
        private static string imageDocFilePath = string.Empty;
        private static string regionAllTextJson = string.Empty;
        private static string regionAllText = string.Empty;

        public static AttrebutesInputResult attrebutesInputResult = null;
        private static int countProgress = 0;
        private static DispatcherTimer dispatcherTimer;


        public MainWindow()
        {
            InitializeComponent();

            ComboBoxFormat.Text = "TEXT";

            if ((string)Properties.Settings.Default["VisionApiUri"] == string.Empty ||
                (string)Properties.Settings.Default["VisionApiKey"] == string.Empty ||
                (string)Properties.Settings.Default["SearchServiceName"] == string.Empty ||
                (string)Properties.Settings.Default["SearchServiceApiKey"] == string.Empty ||
                (string)Properties.Settings.Default["StorageAccountName"] == string.Empty ||
                (string)Properties.Settings.Default["StorageAccountKey"] == string.Empty ||
                (string)Properties.Settings.Default["StorageContainerName"] == string.Empty)
            {
                settingWindow = new SettingWindow();
                settingWindow.ShowDialog();
            }
        }

        private void ButtonSelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = null;
            Nullable<bool> result = false;

            // ファイルを開くダイアログ
            dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.DefaultExt = ".jpg";
            dlg.Filter = "画像ファイル|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";
            dlg.Multiselect = true;
            result = dlg.ShowDialog();
            if (result == null || result == false)
            {
                return;
            }

            imageDocFilePath = Path.GetDirectoryName(dlg.FileNames[0]);
            ListBoxImages.Items.Clear();

            foreach (var fileName in dlg.FileNames)
            {
                ListBoxImages.Items.Add(Path.GetFileName(fileName));
            }
        }

        private async void ButtonAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxImages.Items.Count < 1 || ImageDoc.Source == null)
            {
                MessageBox.Show("画像ファイルを選択してください", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ButtonAnalysis.IsEnabled = false;

            var bitmapSource = (BitmapSource)ImageDoc.Source;
            var encoder = new JpegBitmapEncoder();
            var memoryStream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);
            byte[] buffer = memoryStream.GetBuffer();
            memoryStream.Dispose();

            var apiResult = await CallOcrApi(buffer);
            if (!apiResult.IsSuccess)
            {
                ButtonAnalysis.IsEnabled = true;
                MessageBox.Show(apiResult.ErrorMessage, "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            regionAllTextJson = apiResult.JsonResult;
            regionAllText = apiResult.TextResult;

            if (ComboBoxFormat.Text == "JSON")
                TextBoxJson.Text = regionAllTextJson;
            else
                TextBoxJson.Text = regionAllText.Replace("|||", Environment.NewLine).Replace("||", Environment.NewLine).Replace("|", "　");


            MessageBox.Show("イメージの解析が終了しました","メッセージ",MessageBoxButton.OK, MessageBoxImage.Information);
            ButtonAnalysis.IsEnabled = true;
        }

        private async Task<ApiResult> CallOcrApi(byte[] buffer)
        {
            var apiResult = new ApiResult();

            try
            {
                // HTTP Client
                var client = new HttpClient();
                var uri = Utils.GetVisionApiUri();

                // HTTP header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Utils.GetVisionApiKey());

                // HTTP content
                var content = new ByteArrayContent(buffer);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Call REST API
                var response = await client.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    apiResult.IsSuccess = false;
                    apiResult.ErrorMessage = $"Vision API の呼び出し時にエラーが発生しました。詳細⇒ {response.ReasonPhrase}";
                    return apiResult;
                }

                // Receive result
                string responseResult = await response.Content.ReadAsStringAsync();
                var joRegionAllText = (JObject)JsonConvert.DeserializeObject(responseResult);
                apiResult.JsonResult = JsonConvert.SerializeObject(joRegionAllText, Formatting.Indented);

                apiResult.TextResult = string.Empty;

                var regions = (JArray)joRegionAllText["regions"];
                if (regions.Count >= 1)
                {
                    foreach (var region in regions)
                    {
                        string regionText = string.Empty;

                        var lines = (JArray)region["lines"];
                        if (lines.Count >= 1)
                        {
                            foreach (var line in lines)
                            {
                                var words = (JArray)line["words"];
                                if (words.Count >= 1)
                                {
                                    foreach (var word in words)
                                    {
                                        regionText += (string)word["text"];
                                    }
                                    regionText += "|";
                                }
                            }
                            regionText += "||";
                        }
                        apiResult.TextResult += regionText + "|||";
                    }
                }

                apiResult.IsSuccess = true;
                return apiResult;
            }
            catch (Exception ex)
            {
                apiResult.IsSuccess = false;
                apiResult.ErrorMessage = "エラーが発生しました。詳細⇒ " + ex.ToString();
                return apiResult;
            }
        }

        private void MenuItemSetting_Click(object sender, RoutedEventArgs e)
        {
            settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }

        private void ListBoxImages_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxImages.SelectedItem == null)
            {
                return;
            }

            string filePath = imageDocFilePath + "\\" + ListBoxImages.SelectedItem.ToString();
            // Load targeted image
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            FileStream stream = File.OpenRead(filePath);
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            stream.Close();

            ImageDoc.Source = bitmapImage;
        }

        private void ComboBoxFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (regionAllTextJson == string.Empty || regionAllText == string.Empty)
                return;

            if ((string)ComboBoxFormat.SelectedValue == "1")
                TextBoxJson.Text = regionAllTextJson;
            else
                TextBoxJson.Text = regionAllText.Replace("|||", Environment.NewLine).Replace("||", Environment.NewLine).Replace("|", "　");

        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TextBoxStatusMessage.Text = $"***** {countProgress} 件目を処理中です *****";
        }

        private async void ButtonResister_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxImages.Items.Count <= 0)
            {
                MessageBox.Show($"画像ファイルが一覧に読み込まれていません", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ButtonResister.IsEnabled = false;

            // Launch AttirbuteWindow
            var attributeWindow = new AttributeWindow();
            attributeWindow.ShowDialog();
            if (!attrebutesInputResult.IsSuccess)
            {
                ButtonResister.IsEnabled = true;
                return;
            }

            // Register data to Azure Search Index
            try
            {
                dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                dispatcherTimer.Start();

                var apiResult = await RegisterToSearchIndex();
                dispatcherTimer.Stop();

                if (apiResult.IsSuccess)
                {
                    TextBoxStatusMessage.Text = apiResult.TextResult;
                    MessageBox.Show("検索インデックスへのデータ登録に成功しました", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    TextBoxStatusMessage.Text = apiResult.ErrorMessage;
                    MessageBox.Show("検索インデックスへのデータ登録に失敗しました", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                ButtonResister.IsEnabled = true;
            }
        }

        private async Task<ApiResult> RegisterToSearchIndex()
        {
            var thisResult = new ApiResult();
            countProgress = 0;

            try
            {
                var docs = new QcDocs();
                docs.value = new List<QcDocument>();

                // Blob Storage
                var cloudStorageAccount = CloudStorageAccount.Parse(Utils.GetStorageConnectionString());
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var storageContainerName = Utils.GetStorageContainerName();
                var container = cloudBlobClient.GetContainerReference(storageContainerName);

                foreach (string fileName in ListBoxImages.Items)
                {
                    ++countProgress;

                    // Read image file
                    string filePath = imageDocFilePath + "\\" + fileName;
                    var fileInfo = new FileInfo(filePath);
                    var fileStream = new FileStream(filePath, FileMode.Open);
                    var binaryReader = new BinaryReader(fileStream);
                    byte[] buffer = binaryReader.ReadBytes((int)fileInfo.Length);
                    fileStream.Close();

                    // Call Vision API (OCR)
                    var apiResult = await CallOcrApi(buffer);
                    if (!apiResult.IsSuccess)
                    {
                        thisResult.IsSuccess = false;
                        thisResult.ErrorMessage = apiResult.ErrorMessage;
                        return thisResult;
                    }
                    string ocrContent = apiResult.TextResult.Replace("|||", Environment.NewLine).Replace("||", Environment.NewLine).Replace("|", "　");

                    string fileNameWithoutExtention = Path.GetFileNameWithoutExtension(filePath);
                    string fileExtention = Path.GetExtension(filePath).Replace(".", "");

                    var doc = new QcDocument();
                    doc.id = fileNameWithoutExtention + "_" + fileExtention;
                    doc.document_type = fileExtention.ToUpper();
                    doc.project_name = attrebutesInputResult.Document.project_name;
                    doc.project_location = attrebutesInputResult.Document.project_location;
                    doc.blob_path = $"{attrebutesInputResult.Document.posting_date.ToString("yyyyMMdd")}/{fileName}";
                    doc.ocr_content = ocrContent;
                    doc.additional_information = "";
                    doc.posting_yearmonth = attrebutesInputResult.Document.posting_yearmonth;
                    doc.posting_date = attrebutesInputResult.Document.posting_date;
                    doc.post_until = new DateTimeOffset(new DateTime(2100, 1, 1));
                    doc.posting_updated = doc.posting_date;
                    doc.process_date = doc.posting_date;
                    //var geo = new GeoLocation();
                    //geo.type = "Point";
                    //geo.coordinates = new double[2] { -0.15, 0.981 };
                    //doc.geo_location = geo;

                    doc.tags = new List<string>();

                    var tagTableHelper = new TagTableHelper(Utils.GetStorageConnectionString(), "");
                    var dataTable = tagTableHelper.GetTableData();

                    foreach(DataRow row in dataTable.Rows)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(ocrContent, (string)row["RegularExpression"]))
                        {
                            doc.tags.Add((string)row["TagName"]);
                        }
                    }

                    // Upload image to Blob Storage
                    var blockBlob = container.GetBlockBlobReference(doc.blob_path);
                    Stream stream = File.Open(filePath, FileMode.Open);
                    blockBlob.UploadFromStream(stream);
                    stream.Close();

                    docs.value.Add(doc);
                }

                string json = JsonConvert.SerializeObject(docs);

                // Register data to Search Index
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("api-key", Utils.GetSearchServiceApiKey());
                Uri uri = new Uri(Utils.GetServiceUri(), "/indexes/" + Utils.GetSearchIndexName() + "/docs/index");
                HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(httpClient, HttpMethod.Post, uri, json);
                response.EnsureSuccessStatusCode();

                thisResult.IsSuccess = true;
                thisResult.TextResult = $"検索インデックス ({Utils.GetSearchIndexName()}) に、{countProgress} 件のデータ登録に成功しました";
                return thisResult;
            }
            catch (Exception ex)
            {
                thisResult.IsSuccess = false;
                thisResult.TextResult = $"検索インデックス ({Utils.GetSearchIndexName()}) へのデータ登録に失敗しました : {ex.ToString()}";
                return thisResult;
            }
        }

        private void MenuItemSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchWindow();
            searchWindow.Show();
        }

        private void MenuItemTagRegistration_Click(object sender, RoutedEventArgs e)
        {
            var tagRegWindow = new TagRegWindow();
            tagRegWindow.ShowDialog();
        }
    }
}
