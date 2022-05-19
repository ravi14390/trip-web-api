using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using trip_api.Models;

namespace trip_api.DataAccess
{
    public class CosmosConnection : ICosmosConnection
    {
        private readonly DocumentClient _client;
        private readonly string _accountUrl;
        private readonly string _primarykey;
        public CosmosConnection(IConfiguration config)
        {
            _accountUrl = config.GetValue<string>("Cosmos:AccountURL");
            _primarykey = config.GetValue<string>("Cosmos:AuthKey");
            _client = new DocumentClient(new Uri(_accountUrl), _primarykey);
        }

        public async Task<T> CreateOrUpdateItem<T>(string dbName, string name, T itemObj)
        {
            try
            {
                var document = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbName, name), itemObj);
                T t1 = (dynamic)document.Resource;
                return t1;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<T> GetData<T>(string dbName, string name)
        {
            try
            {
                string _query = "select * from c";
                var result = _client.CreateDocumentQuery<T>((UriFactory.CreateDocumentCollectionUri(dbName, name)), _query);
                return result.ToList<T>();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public List<T> GetDataById<T>(string dbName, string name, string id)
        {
            try
            {
                string _query = String.Format("select * from c where c.id='{0}'", id);
                var result = _client.CreateDocumentQuery<T>((UriFactory.CreateDocumentCollectionUri(dbName, name)), _query);
                return result.ToList<T>();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public List<T> GetDataByQuery<T>(string dbName, string name, string query)
        {
            try
            {
                var result = _client.CreateDocumentQuery<T>((UriFactory.CreateDocumentCollectionUri(dbName, name)), query);
                return result.ToList<T>();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
