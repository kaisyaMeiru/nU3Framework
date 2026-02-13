using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nU3.Core.Interfaces;
using nU3.Core.Logic;
using nU3.Modules.Bas.zz.zipcode.Models;

namespace nU3.Modules.Bas.zz.zipcode.Logic
{
    public class ZipCodeBizLogic : BaseBizLogic
    {
        private readonly IDataService _dataService;
        private readonly System.Net.Http.HttpClient _httpClient;
        
        // Hardcoded for now based on user request
        private const string LEGACY_BASE_URL = "https://emr012edu.cmcnu.or.kr/cmcnu/.live"; 
        private const string INST_CD = "012";

        public ZipCodeBizLogic(
            IDBAccessService db, 
            IFileTransferService file,
            IDataService dataService,
            System.Net.Http.HttpClient httpClient) 
            : base(db, file)
        {
            _dataService = dataService;
            _httpClient = httpClient;
        }

        
        /// <summary>
        /// 지번 주소 검색 (TRZMP00101)
        /// </summary>
        public async Task<List<ZipCodeDto>> SearchLotAddressAsync(SearchConditionDto condition)
        {
            //c:\Project2_OPERATION\05.Framework\nU3.Framework\X-Reference\cmcnu_2.0\webapps\zz\zipcodeweb\
            //c:\Project2_OPERATION\05.Framework\nU3.Framework\X-Reference\cmcnu_2.0\components\zz\zipcodeapp\src\java\phis\nu\his\zz\zipcodeapp\zipcode\

            // Legacy URL Construction
            // https://emr012edu.cmcnu.or.kr/cmcnu/.live?submit_id=TRZMP00101&business_id=zz&instcd=012&searchcondition=zipcode&searchterm={term}

            var queryParams = new System.Text.StringBuilder();
            queryParams.Append($"submit_id=TRZMP00101");
            queryParams.Append($"&business_id=zz");
            queryParams.Append($"&instcd={INST_CD}");
            queryParams.Append($"&searchcondition={condition.SearchCondition}"); // zipcode, combination
            queryParams.Append($"&searchterm={System.Net.WebUtility.UrlEncode(condition.SearchTerm)}");

            var url = $"{LEGACY_BASE_URL}?{queryParams}";

            try 
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var xmlStream = await response.Content.ReadAsStreamAsync();
                return ParseZipCodeXml(xmlStream);
            }
            catch (Exception ex)
            {
                 // Log or rethrow
                 throw new ApplicationException($"우편번호 조회 실패: {ex.Message}", ex);
            }
        }

        private List<ZipCodeDto> ParseZipCodeXml(System.IO.Stream xmlStream)
        {
            var list = new List<ZipCodeDto>();
            try
            {
                var doc = System.Xml.Linq.XDocument.Load(xmlStream);
                var root = doc.Element("root");
                if (root == null) return list;

                var zipcodelist = root.Element("zipcodelist");
                if (zipcodelist == null) return list;

                foreach (var record in zipcodelist.Elements("record"))
                {
                    var dto = new ZipCodeDto
                    {
                        ZipCode = record.Element("zipcode")?.Value ?? "",
                        ZipCdHead = record.Element("zipcdhead")?.Value ?? "",
                        ZipCdFoot = record.Element("zipcdfoot")?.Value ?? "",
                        Address = record.Element("address")?.Value ?? "",
                        PrunningAddress = record.Element("prunningaddress")?.Value ?? "",
                        City = record.Element("city")?.Value ?? "",
                        CityCntyArea = record.Element("citycntyarea")?.Value ?? "",
                        Blok = record.Element("blok")?.Value ?? "",
                        BldNm = record.Element("bldnm")?.Value ?? ""
                    };
                    list.Add(dto);
                }
            }
            catch 
            {
                // XML Parsing Error - Ignore or Log
            }
            return list;
        }

        /// <summary>
        /// 도로명 주소 상세 검색
        /// Database query를 통한 도로명 주소 조회 (SQL: getroadcodelist)
        /// </summary>
        public async Task<List<RoadAddressDto>> SearchRoadAddressAsync(RoadSearchConditionDto condition)
        {            
            return await _dataService.QueryAsync<RoadAddressDto>(
                "zz_zipcodemgr.ZipCode", 
                "getroadcodelist", 
                condition);
        }

        /// <summary>
        /// 도로명 주소 간단 검색
        /// </summary>
        public async Task<List<ZipCodeDto>> SearchRoadAddressAsync(SearchConditionDto condition)
        {
            var roadCondition = new RoadSearchConditionDto
            {
                RoadNm = condition.SearchTerm
            };
            
            var roadResults = await SearchRoadAddressAsync(roadCondition);
            
            return roadResults.Select(r => new ZipCodeDto
            {
                ZipCode = r.ZipCode,
                City = r.City,
                CityCntyArea = r.CityCntyArea,
                Address = $"{r.City} {r.CityCntyArea} {r.RoadNm} {r.BldNo}".Trim(),
                BldNm = r.BldNm
            }).ToList();
        }










        public async Task<List<RoadAddressDto>> SearchRoadAddressAsync2(RoadSearchConditionDto condition)
        {
            // Similar logic can be applied here if URL pattern is known
            return await _dataService.QueryAsync<RoadAddressDto>("zz_zipcodeapp.ZipCode", "searchRoadAddress", condition);
        }


        /// <summary>
        /// 주소 검증 (TRZMP00104)
        /// </summary>
        public async Task<VerificationResultDto> VerifyAddressAsync(VerificationDto verification)
        {
            return await _dataService.ExecuteAsync<VerificationDto, VerificationResultDto>("zz_zipcodeapp.ZipCode", "verifyAddress", verification);
        }
    }
}
