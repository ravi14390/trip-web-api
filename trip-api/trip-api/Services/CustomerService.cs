using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trip_api.DataAccess;
using trip_api.Models;

namespace trip_api.Services
{
    public class CustomerService
    {
        private readonly ICosmosConnection _connection;
        private readonly string _dbName;
        private readonly string _containerName;

        public CustomerService(ICosmosConnection con, IConfiguration config)
        {
            _connection = con;
            _dbName = config.GetValue<string>("Cosmos:DatabaseId");
            _containerName = config.GetValue<string>("Cosmos:CollectionCustomer");
        }
        public List<Customer> GetAllCustomers()
        {
            return _connection.GetData<Customer>(_dbName, _containerName);
        }
        
        public Customer GetCustomerByLoginName(string loginId)
        {
            string query = string.Format("select * from c where c.LoginId='{0}'",loginId);
            var result = _connection.GetDataByQuery<Customer>(_dbName, _containerName, query);
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }
        public Customer GetCustomerById(string id)
        {
            string query = string.Format("select * from c where c.id='{0}'", id);
            var result = _connection.GetDataByQuery<Customer>(_dbName, _containerName, query);
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        internal bool CheckIfExist(Customer customer)
        {
            string query = string.Format("select * from c where c.LoginId='{0}'", customer.LoginId);
            var result = _connection.GetDataByQuery<Customer>(_dbName, _containerName, query);
            if (result.Count > 0)
            {
                return true;
            }
            return false;
        }

        internal Customer CreateNewCustomer(Customer customer)
        {
            var c1 = _connection.CreateOrUpdateItem<Customer>(_dbName, _containerName, customer);
            return c1.Result;
        }

        
    }
}
