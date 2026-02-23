using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using nU3.Core.Interfaces;
using nU3.Core.Logic;

namespace nU3.Modules.EMR.CO.EmrCommon.Logic
{
    public class EMR_CO_0001BizLogic : BaseBizLogic
    {
        private readonly IDataService _dataService;
        private readonly IFileTransferService _fileService;
        private readonly IDBAccessService _dbService;

        public EMR_CO_0001BizLogic(IDBAccessService db, IFileTransferService file, IDataService dataService, System.Net.Http.HttpClient httpClient) : base(db, file)
        {
            _dataService = dataService;
            _fileService = file;
            _dbService = db;
        }

        public async Task<List<object>> SearchLotAddressAsync(object condition)
        {
            return new List<object>();
        }
    }
}
