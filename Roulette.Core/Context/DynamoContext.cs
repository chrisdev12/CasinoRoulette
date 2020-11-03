using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roulette.Core.Context
{
    public class DynamoContext
    {
        private readonly AmazonDynamoDBClient _Client;
        private readonly DynamoDBContext _Context;
        public DynamoContext(AmazonDynamoDBClient Client)
        {
            _Client =  Client;
            _Context = new DynamoDBContext(Client);
        }
        #region queries
        public async Task<List<T>> GetAll<T>(string tableName)
        {
            Table table = Table.LoadTable(_Client, tableName);
            Search response = table.Scan(new ScanFilter());

            var dbResponse = await response.GetRemainingAsync();
            List<T> data = new List<T>();
            foreach (var item in dbResponse)
            {
                data.Add(_Context.FromDocument<T>(item));
            }

            return data;
        }
        public async Task<T> Get<T>(T objectToLoad)
        {
            return await _Context.LoadAsync(objectToLoad);
        }
        public async Task Insert<T>(T objectToInsert)
        {
            await _Context.SaveAsync(objectToInsert);
        }
        public async Task Update<T>(T objectTUpdated)
        {
            T objectRetrieved = await _Context.LoadAsync(objectTUpdated);
            objectRetrieved = objectTUpdated;

            await _Context.SaveAsync(objectRetrieved);
        }
        public async Task Delete<T>(T objectToDelete)
        {
            await _Context.DeleteAsync(objectToDelete);
        }
        #endregion
    }
}
