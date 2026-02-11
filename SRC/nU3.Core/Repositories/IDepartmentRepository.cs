using System.Collections.Generic;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.Repositories
{
    /// <summary>
    /// 부서 정보 저장소 인터페이스
    /// 부서 마스터 데이터 조회 및 사용자-부서 매핑을 관리합니다.
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// 모든 활성화된 부서 목록을 조회합니다.
        /// </summary>
        /// <returns>부서 목록 (DisplayOrder 순서로 정렬)</returns>
        List<DepartmentDto> GetAllDepartments();

        /// <summary>
        /// 부서 코드로 특정 부서 정보를 조회합니다.
        /// </summary>
        /// <param name="deptCode">부서 코드</param>
        /// <returns>부서 정보, 없으면 null</returns>
        DepartmentDto? GetDepartmentByCode(string deptCode);

        /// <summary>
        /// 특정 사용자가 속한 부서 목록을 조회합니다.
        /// SYS_USER_DEPT 테이블에서 사용자-부서 매핑을 조회합니다.
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <returns>사용자가 속한 부서 목록</returns>
        Task<List<DepartmentDto>> GetDepartmentsByUserIdAsync(string userId);

        /// <summary>
        /// 부서 정보를 추가하거나 수정합니다.
        /// </summary>
        /// <param name="department">부서 정보</param>
        void UpsertDepartment(DepartmentDto department);

        /// <summary>
        /// 부서를 비활성화합니다.
        /// </summary>
        /// <param name="deptCode">부서 코드</param>
        void DeactivateDepartment(string deptCode);
    }
}
