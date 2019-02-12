using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace SimpleEchoBot.AzureStorage
{

    [Serializable]
    public class AzureTableManager
    {
        // private property  
        private static CloudTable table;

        // Constructor   
        public AzureTableManager(string _CloudTableName)
        {
            if (string.IsNullOrEmpty(_CloudTableName))
            {
                throw new ArgumentNullException("Table", "Table Name can't be empty");
            }
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                table = tableClient.GetTableReference(_CloudTableName);
                table.CreateIfNotExists();
            }
            catch (StorageException storageExceptionObj)
            {
                throw storageExceptionObj;
            }
            catch (Exception exceptionObj)
            {
                throw exceptionObj;
            }
        }
        public async Task InsertEntity<T>(T entity, bool forInsert = true) where T : TableEntity, new()
        {
            try
            {
                if (forInsert)
                {
                    var insertOperation = TableOperation.Insert(entity);
                    await table.ExecuteAsync(insertOperation);
                }
                else
                {
                    var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);
                    await table.ExecuteAsync(insertOrMergeOperation);
                }
            }
            catch (Exception exceptionObj)
            {
                throw exceptionObj;
            }
        }
        public async Task<List<T>> RetrieveEntity<T>(string query = null) where T : TableEntity, new()
        {
            try
            {
                // Create the Table Query Object for Azure Table Storage  
                TableQuery<T> dataTableQuery = new TableQuery<T>();
                if (!string.IsNullOrEmpty(query))
                {
                    dataTableQuery = new TableQuery<T>().Where(query);
                }

                var dataEnumerable = table.ExecuteQuery(dataTableQuery);
                List<T> dataList = new List<T>();

                foreach(var data in dataEnumerable)
                {
                    dataList.Add(data);
                }
                return dataList;
            }
            catch (Exception exceptionObj)
            {
                
                throw;
            }
        }
        public async Task<bool> DoesEntityExist(string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);

                return result.Result != null;
            }
            catch (StorageException e)
            {
                throw;
            }
        }
        public bool DeleteEntity<T>(T entity) where T : TableEntity, new()
        {
            try
            {
                var DeleteOperation = TableOperation.Delete(entity);
                table.Execute(DeleteOperation);
                return true;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }
        public async Task BatchInsert(TableBatchOperation tableOperations)
        {
            try
            {
                if (tableOperations != null)
                {
                    await table.ExecuteBatchAsync(tableOperations);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}