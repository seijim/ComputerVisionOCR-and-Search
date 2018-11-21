using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage.Table;


namespace WpfAppCvSearch
{
    public class ApiResult
    {
        public bool IsSuccess { set; get; }
        public string TextResult { set; get; }
        public string JsonResult { set; get; }
        public string ErrorMessage { set; get; }
    }

    public class TagRule
    {
        public string TagName {set;get;}
        public string RegularExpression { set; get; }
    }

    public class AttrebutesInputResult
    {
        public bool IsSuccess { set; get; }
        public QcDocument Document { set; get; }
        public string ErrorMessage { set; get; }
    }

    public class GeoLocation
    {
        public string type { set; get; }
        public double[] coordinates { set; get; }
    }

    public class QcDocument
    {
        public string id { set; get; }
        public string document_type { set; get; }
        public string project_name { set; get; }
        public string project_location { set; get; }
        public string blob_path { set; get; }
        public string ocr_content { set; get; }
        public string additional_information { set; get; }
        public string posting_yearmonth { set; get; }
        public DateTimeOffset posting_date { set; get; }
        public DateTimeOffset post_until { set; get; }
        public DateTimeOffset posting_updated { set; get; }
        public DateTimeOffset process_date { set; get; }
        public GeoLocation geo_location { set; get; }
        public List<string> tags { set; get; }
    }

    public class QcDocumentView
    {
        public string id { set; get; }
        public string document_type { set; get; }
        public string project_name { set; get; }
        public string project_location { set; get; }
        public string blob_path { set; get; }
        public string ocr_content { set; get; }
        public string posting_date { set; get; }
        public string posting_updated { set; get; }
        public string tags { set; get; }
    }

    public class QcDocs
    {
        public List<QcDocument> value { set; get; }
    }

    public class TagTableEntity : TableEntity, IComparable<TagTableEntity>
    {
        public string TagName { set; get; }
        public string RegularExpression { set; get; }
        public DateTime RegisteredDatetime { set; get; }

        public int CompareTo(TagTableEntity other)
        {
            return string.Compare(this.RowKey, other.RowKey
                , StringComparison.OrdinalIgnoreCase);
        }

    }

    public class TableMode
    {
        public static string Display = "Display";
        public static string Normal = "Normal";
    }
}
