using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using nU3.Core.Interfaces;
using nU3.Core.Logic;

namespace DataAccessTest
{
    public class SampleBizLogic : IBizLogic
    {
        private readonly IDataService _dataService;

        public SampleBizLogic(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<bool> SaveItemAsync(string name, int value)
        {
            var req = new { Name = name, Value = value };
            // Simulate saving data using legacy backend service
            // Expecting serviceId="sampleService", method="saveItem"
            return await _dataService.ExecuteAsync<object, bool>("sampleService", "saveItem", req);
        }

        public async Task<List<string>> GetItemsAsync()
        {
            // Simulate retrieving data
            // Expecting serviceId="sampleService", method="getItems"
            return await _dataService.QueryAsync<string>("sampleService", "getItems");
        }
    }
}
