using System.Collections.Generic;
using System.Threading.Tasks;
using nU3.Models;

namespace nU3.Core.Repositories
{
    /// <summary>
    /// 사용자 정보 및 권한 관련 데이터를 관리하는 저장소 인터페이스입니다.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 모든 활성화된 사용자 목록을 조회합니다.
        /// </summary>
        Task<List<UserInfoDto>> GetAllUsersAsync();

        /// <summary>
        /// 사용자 ID를 기반으로 활성화된 사용자 정보를 조회합니다.
        /// </summary>
        Task<UserInfoDto?> GetUserByIdAsync(string userId);

        /// <summary>
        /// 사용자 ID를 기반으로 활성화된 사용자 정보를 동기적으로 조회합니다.
        /// </summary>
        UserInfoDto? GetUserById(string userId);

        /// <summary>
        /// 사용자가 소속된 부서 코드 목록을 조회합니다.
        /// </summary>
        Task<List<string>> GetUserDepartmentCodesAsync(string userId);

        /// <summary>
        /// 사용자 정보를 추가하거나 갱신합니다.
        /// </summary>
        Task SaveUserAsync(UserInfoDto user);

        /// <summary>
        /// 사용자의 부서 맵핑 정보를 갱신합니다.
        /// </summary>
        Task UpdateUserDepartmentsAsync(string userId, List<string> deptCodes);
    }
}
