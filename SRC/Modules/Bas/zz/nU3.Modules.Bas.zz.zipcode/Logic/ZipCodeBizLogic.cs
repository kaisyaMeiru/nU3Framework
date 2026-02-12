using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using nU3.Core.Interfaces;
using nU3.Core.Logic;
using nU3.Modules.Bas.zz.zipcode.Models;

namespace nU3.Modules.Bas.zz.zipcode.Logic
{
    public class ZipCodeBizLogic : BaseBizLogic
    {
        private readonly IDataService _dataService; // Generic Service Adapter

        public ZipCodeBizLogic(
            IDBAccessService db, 
            IFileTransferService file,
            IDataService dataService) 
            : base(db, file)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// 지번 주소 검색 (TRZMP00101)
        /// Wraps generic call to: zz_zipcodeapp.ZipCode.searchLotAddress
        /// </summary>
        public async Task<List<ZipCodeDto>> SearchLotAddressAsync(SearchConditionDto condition)
        {
            // Maps to <select id="searchLotAddress" ...> in ZipCode.xml on backend
            return await _dataService.QueryAsync<ZipCodeDto>(
                "zz_zipcodeapp.ZipCode", 
                "searchLotAddress", 
                condition); 
        }

        /// <summary>
        /// 도로명 주소 검색 (TRZMP00102)
        /// Wraps generic call to: zz_zipcodeapp.ZipCode.searchRoadAddress
        /// </summary>
        public async Task<List<RoadAddressDto>> SearchRoadAddressAsync(RoadSearchConditionDto condition)
        {            
            // Maps to <select id="searchRoadAddress" ...> in ZipCode.xml on backend
            return await _dataService.QueryAsync<RoadAddressDto>(
                "zz_zipcodeapp.ZipCode", 
                "searchRoadAddress", 
                condition);
        }

        /// <summary>
        /// 도로명 주소 검색 (Overload for mixed usage)
        /// </summary>
        public async Task<List<ZipCodeDto>> SearchRoadAddressAsync(SearchConditionDto condition)
        {
            // Fallback or specific logic if needed. 
            // For now, redirect to Lot Search or throw if inappropriate
            return await SearchLotAddressAsync(condition);
        }

        /// <summary>
        /// 주소 검증 (TRZMP00104)
        /// Wraps generic call to: zz_zipcodeapp.ZipCode.verifyAddress
        /// </summary>
        public async Task<VerificationResultDto> VerifyAddressAsync(VerificationDto verification)
        {
            // Maps to <select id="verifyAddress" ...> in ZipCode.xml on backend
            return await _dataService.QuerySingleAsync<VerificationResultDto>(
                "zz_zipcodeapp.ZipCode", 
                "verifyAddress", 
                verification);
        }
    }
}
