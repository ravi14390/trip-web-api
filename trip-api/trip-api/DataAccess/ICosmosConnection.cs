using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trip_api.DataAccess
{
    public interface ICosmosConnection
    {
        List<T> GetData<T>(string dbName, string name);
        List<T> GetDataById<T>(string dbName, string name,string id);
        List<T> GetDataByQuery<T>(string dbName, string name, string query);
        Task<T> CreateOrUpdateItem<T>(string dbName, string name,T itemObj);
    }
}
