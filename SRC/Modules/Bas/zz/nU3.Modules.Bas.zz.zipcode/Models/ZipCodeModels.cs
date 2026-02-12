using System;

namespace nU3.Modules.Bas.zz.zipcode.Models
{
    public class ZipCodeDto
    {
        public string ZipCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        // Legacy fields for mapping
        public string ZipCdHead { get; set; } = string.Empty;
        public string ZipCdFoot { get; set; } = string.Empty;
        public string PrunningAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string CityCntyArea { get; set; } = string.Empty;
        public string Blok { get; set; } = string.Empty;
        public string Mile { get; set; } = string.Empty;
        public string BldNo { get; set; } = string.Empty;
        public string BldNm { get; set; } = string.Empty;
    }

    public class RoadAddressDto
    {
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string CityCntyArea { get; set; } = string.Empty;
        public string RoadNm { get; set; } = string.Empty;
        public string BldNo { get; set; } = string.Empty;
        public string BldNm { get; set; } = string.Empty;
        public string Blok { get; set; } = string.Empty; // 읍면
        public string UnderYn { get; set; } = string.Empty; // 지하
    }

    public class SearchConditionDto
    {
        public string SearchCondition { get; set; } = string.Empty; // zipcode, address, combination
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class RoadSearchConditionDto
    {
        public string City { get; set; } = string.Empty;
        public string CityCntyArea { get; set; } = string.Empty;
        public string RoadNm { get; set; } = string.Empty;
        public string BldNo { get; set; } = string.Empty;
        public string BldNm { get; set; } = string.Empty;
        public string BldMainNo { get; set; } = string.Empty;
        public string BldSubNo { get; set; } = string.Empty;
    }

    public class VerificationDto
    {
        public string ZipCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public string SearchMode { get; set; } = string.Empty; // J or N
        public string Addr1 { get; set; } = string.Empty;
        public string Addr2 { get; set; } = string.Empty;
    }

    public class VerificationResultDto
    {
        public System.Collections.Generic.List<MultiListDto> MultiList { get; set; } = new();
        public System.Collections.Generic.List<RefMsgDto> RefMsg { get; set; } = new();
    }

    public class MultiListDto
    {
        public string Node { get; set; } = string.Empty;
        public string Zip1 { get; set; } = string.Empty;
        public string Zip2 { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gubun { get; set; } = string.Empty;
        // Add more fields as needed per XFDL ds_multilist
    }

    public class RefMsgDto
    {
        public string Rmg3 { get; set; } = string.Empty;
        public string Rcd3 { get; set; } = string.Empty;
    }
}
