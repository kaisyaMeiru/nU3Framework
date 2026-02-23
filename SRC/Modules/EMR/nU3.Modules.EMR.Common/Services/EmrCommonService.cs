using System;

namespace nU3.Modules.EMR.Common.Services
{
    public interface IEmrCommonService
    {
        string FormatMedicalRecordNumber(string rawId);
        bool IsEmergency(string departmentCode);
        string GetDepartmentName(string code);
    }

    /// <summary>
    /// EMR 공통 비즈니스 로직 구현체
    /// </summary>
    public class EmrCommonService : IEmrCommonService
    {
        public string FormatMedicalRecordNumber(string rawId)
        {
            if (string.IsNullOrWhiteSpace(rawId)) return string.Empty;
            // 예: 123 -> 00000123 (8자리 포맷팅)
            return rawId.PadLeft(8, '0');
        }

        public bool IsEmergency(string departmentCode)
        {
            return string.Equals(departmentCode, Constants.EmrConstants.Departments.Emergency, StringComparison.OrdinalIgnoreCase);
        }

        public string GetDepartmentName(string code)
        {
            return code?.ToUpper() switch
            {
                Constants.EmrConstants.Departments.InternalMedicine => "내과",
                Constants.EmrConstants.Departments.Surgery => "외과",
                Constants.EmrConstants.Departments.Orthopedics => "정형외과",
                Constants.EmrConstants.Departments.Pediatrics => "소아청소년과",
                Constants.EmrConstants.Departments.Emergency => "응급의학과",
                _ => "기타"
            };
        }
    }
}
