using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using nU3.Core.Interfaces;
using nU3.Core.Logic;

namespace nU3.Modules.NUR.NR.NURNAMESPACE.Logic
{
    public class NURPROGRAMIDBizLogic : BaseBizLogic
    {
        private readonly IDataService _dataService;
        private readonly IFileTransferService _fileService;
        private readonly IDBAccessService _dbService;

        public NURPROGRAMIDBizLogic(IDBAccessService db, IFileTransferService file, IDataService dataService, System.Net.Http.HttpClient httpClient) : base(db, file)
        {
            _dataService = dataService;
            _fileService = file;
            _dbService = db;
        }

        public async Task<List<object>> SearchLotAddressAsync(object condition)
        {            
            return new List<object>();
        }

        public void DataProcessing()
        {
            // 单捞磐 贸府 肺流 备泅
        }
    }
}
