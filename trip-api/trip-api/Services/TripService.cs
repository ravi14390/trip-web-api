using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trip_api.DataAccess;
using trip_api.Models;

namespace trip_api.Services
{
    public class TripService
    {
        private readonly ICosmosConnection _connection;
        private readonly string _dbName;
        private readonly string _containerName;

        public TripService(ICosmosConnection con, IConfiguration config)
        {
            _connection = con;
            _dbName = config.GetValue<string>("Cosmos:DatabaseId");
            _containerName = config.GetValue<string>("Cosmos:CollectionTrip");
        }
        public List<Trip> GetAllTrips()
        {
            return _connection.GetData<Trip>(_dbName, _containerName);
        }
        public Trip GetTripById(string id)
        {
            var result = _connection.GetDataById<Trip>(_dbName, _containerName, id);
            return (result?.Count > 0) ? result[0] : null;
        }
        public Trip CreateNewTrip(Trip trip)
        {
            var t1 = _connection.CreateOrUpdateItem<Trip>(_dbName, _containerName, trip);
            return t1.Result;
        }

        public bool CheckOverlapping(Trip trip)
        {
            string query = String.Format("select * from c where c.CustomerId='{0}' AND ((c.StartDate between '{1}' and '{2}') or (c.EndDate between '{1}' and '{2}') or ('{1}' between c.StartDate and c.EndDate))", trip.CustomerId,trip.StartDate.ToString("s"), trip.EndDate.ToString("s"));
            var result = _connection.GetDataByQuery<Trip>(_dbName, _containerName, query);
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        public Trip CancelTrip(string _id)
        {
            var result = _connection.GetDataById<Trip>(_dbName, _containerName, _id);
            if (result.Count > 0)
            { 
                result[0].Cancelled = true;
                var t1 = _connection.CreateOrUpdateItem<Trip>(_dbName, _containerName, result[0]);
                return t1.Result;
            }
            return null;
        }

        public bool CanCancel(string id)
        {
            var result = _connection.GetDataById<Trip>(_dbName, _containerName,id);
            if (result.Count > 0)
            {
                double days = (result[0].StartDate - DateTime.Now).TotalDays;
                if (Math.Round(days) >= 7)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
