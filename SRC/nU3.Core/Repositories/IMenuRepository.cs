using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Repositories
{
    /// <summary>
    /// 메뉴 및 프로그램 관련 저장소 인터페이스를 정의합니다.
    /// <para>메뉴 트리 관리, 프로그램 메타데이터 조회/수정 등의 기능을 DB 레이어와 분리하여 제공하는 계약입니다.</para>
    /// </summary>
    public interface IMenuRepository
    {
        /// <summary>
        /// 모든 메뉴 항목을 반환합니다. 트리 구조 생성에 사용합니다.
        /// </summary>
        List<MenuDto> GetAllMenus();

        /// <summary>
        /// 메뉴 테이블의 모든 항목을 삭제합니다. (초기화 용도)
        /// </summary>
        void DeleteAllMenus();

        /// <summary>
        /// 메뉴 항목 DTO를 추가합니다.
        /// </summary>
        /// <param name="menu">추가할 메뉴 정보</param>
        void AddMenu(MenuDto menu);

        /// <summary>
        /// 특정 부서의 메뉴 목록을 조회합니다.
        /// <para>SYS_DEPT_MENU 테이블에서 해당 부서 코드에 연결된 메뉴를 반환합니다.</para>
        /// </summary>
        /// <param name="deptCode">부서 코드</param>
        /// <returns>부서별 메뉴 목록</returns>
        List<MenuDto> GetMenusByDeptCode(string deptCode);

        /// <summary>
        /// 특정 사용자와 부서 조합의 커스텀 메뉴를 조회합니다.
        /// <para>SYS_USER_MENU 테이블을 참조하여 사용자 맞춤 메뉴를 반환합니다.</para>
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="deptCode">부서 코드</param>
        /// <returns>사용자별 커스텀 메뉴 목록 (없으면 빈 리스트)</returns>
        List<MenuDto> GetMenusByUserAndDept(string userId, string deptCode);

        /// <summary>
        /// 특정 부서의 메뉴를 모두 삭제합니다.
        /// </summary>
        /// <param name="deptCode">부서 코드</param>
        void DeleteMenusByDeptCode(string deptCode);

        /// <summary>
        /// 특정 사용자와 부서 조합의 커스텀 메뉴를 모두 삭제합니다.
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="deptCode">부서 코드</param>
        void DeleteMenusByUserAndDept(string userId, string deptCode);

        /// <summary>
        /// 부서별 메뉴를 추가합니다. SYS_DEPT_MENU 테이블에 삽입됩니다.
        /// </summary>
        /// <param name="deptCode">부서 코드</param>
        /// <param name="menu">메뉴 정보</param>
        void AddMenuForDept(string deptCode, MenuDto menu);

        /// <summary>
        /// 사용자별 커스텀 메뉴를 추가합니다. SYS_USER_MENU 테이블에 삽입됩니다.
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="deptCode">부서 코드</param>
        /// <param name="menu">메뉴 정보</param>
        void AddMenuForUser(string userId, string deptCode, MenuDto menu);
    }

    /// <summary>
    /// 프로그램(화면) 메타데이터를 관리하는 저장소 인터페이스입니다.
    /// <para>프로그램 목록 조회, 모듈별 프로그램 조회, ProgId로 프로그램 조회, upsert 및 누락된 프로그램 비활성화 기능을 제공합니다.</para>
    /// </summary>
    public interface IProgramRepository
    {
        /// <summary>
        /// 모든 프로그램 정보를 반환합니다.
        /// </summary>
        List<ProgramDto> GetAllPrograms();

        /// <summary>
        /// 지정된 모듈 ID에 속한 프로그램 목록을 반환합니다.
        /// </summary>
        /// <param name="moduleId">모듈 ID</param>
        /// <returns>해당 모듈의 프로그램 목록</returns>
        List<ProgramDto> GetProgramsByModuleId(string moduleId);

        /// <summary>
        /// ProgId에 해당하는 프로그램 정보를 반환합니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="progId">프로그램 ID</param>
        /// <returns>해당 프로그램 정보 또는 null</returns>
        ProgramDto? GetProgramByProgId(string progId);

        /// <summary>
        /// 프로그램 정보를 삽입하거나 업데이트(upsert)합니다.
        /// </summary>
        /// <param name="program">저장할 프로그램 정보</param>
        void UpsertProgram(ProgramDto program);

        /// <summary>
        /// 모듈에 존재하지 않는(누락된) 프로그램을 비활성화 처리합니다.
        /// <para>모듈 내의 활성 ProgId 목록과 비교하여 없는 항목을 비활성화합니다.</para>
        /// </summary>
        /// <param name="moduleId">모듈 ID</param>
        /// <param name="activeProgIds">현재 활성화되어야 하는 ProgId 목록</param>
        void DeactivateMissingPrograms(string moduleId, IEnumerable<string> activeProgIds);
    }
}
