using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WpfAppCvSearch
{
    public class Utils
    {
        public static string GetVisionApiUri()
        {
            return (string)Properties.Settings.Default["VisionApiUri"];
        }

        public static string GetVisionApiKey()
        {
            return (string)Properties.Settings.Default["VisionApiKey"];

        }

        public static Uri GetServiceUri()
        {
            return new Uri("https://" + (string)Properties.Settings.Default["SearchServiceName"] + ".search.windows.net");
        }

        public static string GetSearchServiceName()
        {
            return (string)Properties.Settings.Default["SearchServiceName"];
        }

        public static string GetSearchServiceApiKey()
        {
            return (string)Properties.Settings.Default["SearchServiceApiKey"];
        }

        public static string GetSearchIndexName()
        {
            return (string)Properties.Settings.Default["SearchIndexName"];
        }

        public static int GetDefaultPageSize()
        {
            return 20;
        }

        public static string GetStorageContainerName()
        {
            return (string)Properties.Settings.Default["StorageContainerName"];
        }

        public static string GetStorageConnectionString()
        {
            string storageAccountName = (string)Properties.Settings.Default["StorageAccountName"];
            string storageKey = (string)Properties.Settings.Default["StorageAccountKey"];
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageKey};EndpointSuffix=core.windows.net";

            return connectionString;
        }

    }
}
