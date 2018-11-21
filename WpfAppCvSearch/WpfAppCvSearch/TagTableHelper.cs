using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Data;

namespace WpfAppCvSearch
{
    public class TagTableHelper
    {
        private string connectionString;
        private string tableName = "TagTable";
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;

        public TagTableHelper(string connectionString, string tableName)
        {
            this.connectionString = connectionString;
            if (tableName != string.Empty)
                this.tableName = tableName;
            storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public void CreateLogTableIfNotExists()
        {
            var table = tableClient.GetTableReference(tableName);

            table.CreateIfNotExists();
        }

        public void BulkUpdateTable(DataTable dataTable, string tableMode)
        {
            var changedTable = dataTable.GetChanges(DataRowState.Added);
            if (changedTable != null)
            {
                foreach (DataRow row in changedTable.Rows)
                {
                    if (row[0].ToString() == string.Empty || row[1].ToString() == string.Empty)
                        continue;

                    var tagData = new TagTableEntity();
                    tagData.TagName = row[0].ToString();
                    tagData.RegularExpression = row[1].ToString();

                    InsertRow(tagData);
                }
            }

            changedTable = dataTable.GetChanges(DataRowState.Modified);
            if (changedTable != null)
            {
                foreach (DataRow row in changedTable.Rows)
                {
                    if (row[0].ToString() == string.Empty || row[1].ToString() == string.Empty)
                        continue;

                    var tagData = new TagTableEntity();
                    tagData.TagName = row[0].ToString();
                    tagData.RegularExpression = row[1].ToString();
                    tagData.ETag = row[3].ToString();

                    UpdateRow(tagData);
                }
            }

            changedTable = dataTable.GetChanges(DataRowState.Deleted);
            if (changedTable != null)
            {
                var changedTableDeleted = changedTable.Copy();
                changedTableDeleted.RejectChanges();
                foreach (DataRow row in changedTableDeleted.Rows)
                {
                    var tagData = new TagTableEntity();
                    tagData.TagName = row[0].ToString();
                    tagData.RegularExpression = row[1].ToString();
                    if (tableMode == TableMode.Display)
                        tagData.RegisteredDatetime = DateTime.Parse(row[2].ToString());
                    else
                        tagData.RegisteredDatetime = (DateTime)row[2];
                    tagData.ETag = row[3].ToString();

                    DeleteRow(tagData);
                }
            }
        }

        public void InsertRow(TagTableEntity tagData)
        {
            DateTime today = DateTime.Now;
            tagData.RowKey = tagData.TagName;
            tagData.PartitionKey = "1";
            tagData.RegisteredDatetime = today;
            var table = tableClient.GetTableReference(tableName);
            var operation = TableOperation.Insert(tagData);

            table.ExecuteAsync(operation).Wait();
        }

        public void UpdateRow(TagTableEntity tagData)
        {
            DateTime today = DateTime.Now;
            tagData.RowKey = tagData.TagName;
            tagData.PartitionKey = "1";
            tagData.RegisteredDatetime = today;
            var table = tableClient.GetTableReference(tableName);
            var operation = TableOperation.Replace(tagData);

            table.ExecuteAsync(operation).Wait();
        }

        public void DeleteRow(TagTableEntity tagData)
        {
            tagData.RowKey = tagData.TagName;
            tagData.PartitionKey = "1";
            var table = tableClient.GetTableReference(tableName);
            var operation = TableOperation.Delete(tagData);

            table.ExecuteAsync(operation).Wait();
        }

        public DataTable GetTableDisplayData()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("タグ名"));
            dataTable.Columns.Add(new DataColumn("正規表現"));
            dataTable.Columns.Add(new DataColumn("登録日時"));
            dataTable.Columns.Add(new DataColumn("マーカー"));
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0] };

            var table = tableClient.GetTableReference(tableName);
            var query = new TableQuery<TagTableEntity>();

            foreach (var entity in table.ExecuteQuery(query))
            {
                var dataRow = dataTable.NewRow();
                dataRow[0] = entity.RowKey;
                dataRow[1] = entity.RegularExpression;
                dataRow[2] = entity.RegisteredDatetime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                dataRow[3] = entity.ETag;

                dataTable.Rows.Add(dataRow);
            }
            dataTable.AcceptChanges();

            return dataTable;
        }

        public DataTable GetTableData()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("TagName"));
            dataTable.Columns.Add(new DataColumn("RegularExpression"));
            dataTable.Columns.Add(new DataColumn("RegisteredDatetime", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("ReplaceMarker"));
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0] };

            var table = tableClient.GetTableReference(tableName);
            var query = new TableQuery<TagTableEntity>();

            foreach (var entity in table.ExecuteQuery(query))
            {
                var dataRow = dataTable.NewRow();
                dataRow[0] = entity.RowKey;
                dataRow[1] = entity.RegularExpression;
                dataRow[2] = entity.RegisteredDatetime;
                dataRow[3] = entity.ETag;

                dataTable.Rows.Add(dataRow);
            }
            dataTable.AcceptChanges();

            return dataTable;
        }
    }
}
