using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Repositories
{
    /// <summary>
    /// �޴� �� ���α׷� ��Ÿ�����Ϳ� �����ϱ� ���� �������丮 �������̽��Դϴ�.
    /// �޴� Ʈ��, ���α׷� ��� ���� ���� DB���� ��ȸ/�����ϴ� å���� �����ϴ�.
    /// </summary>
    public interface IMenuRepository
    {
        /// <summary>
        /// ��� �޴� �׸��� ��ȯ�մϴ�. �޴� Ʈ�� ������ ���˴ϴ�.
        /// </summary>
        List<MenuDto> GetAllMenus();

        /// <summary>
        /// �޴� ���̺��� ��� ����(�ʱ�ȭ)�մϴ�. �ַ� �籸�� �� ���˴ϴ�.
        /// </summary>
        void DeleteAllMenus();

    /// <summary>
    /// 메뉴 항목 DTO를 추가합니다.
    /// </summary>
    void AddMenu(MenuDto menu);

    /// <summary>
    /// 특정 부서의 메뉴 목록을 조회합니다.
    /// SYS_DEPT_MENU 테이블에서 부서별 메뉴를 조회합니다.
    /// </summary>
    /// <param name="deptCode">부서 코드</param>
    /// <returns>부서별 메뉴 목록</returns>
    List<MenuDto> GetMenusByDeptCode(string deptCode);

    /// <summary>
    /// 특정 사용자 + 부서 조합의 커스텀 메뉴를 조회합니다.
    /// SYS_USER_MENU 테이블에서 조회합니다.
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="deptCode">부서 코드</param>
    /// <returns>사용자별 커스텀 메뉴 목록, 없으면 빈 리스트</returns>
    List<MenuDto> GetMenusByUserAndDept(string userId, string deptCode);

    /// <summary>
    /// 특정 부서의 메뉴를 모두 삭제합니다.
    /// SYS_DEPT_MENU 테이블에서 해당 부서의 메뉴를 삭제합니다.
    /// </summary>
    /// <param name="deptCode">부서 코드</param>
    void DeleteMenusByDeptCode(string deptCode);

    /// <summary>
    /// 특정 사용자 + 부서 조합의 커스텀 메뉴를 모두 삭제합니다.
    /// SYS_USER_MENU 테이블에서 해당 사용자+부서의 메뉴를 삭제합니다.
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="deptCode">부서 코드</param>
    void DeleteMenusByUserAndDept(string userId, string deptCode);

    /// <summary>
    /// 부서별 메뉴를 추가합니다.
    /// SYS_DEPT_MENU 테이블에 삽입합니다.
    /// </summary>
    /// <param name="deptCode">부서 코드</param>
    /// <param name="menu">메뉴 정보</param>
    void AddMenuForDept(string deptCode, MenuDto menu);

    /// <summary>
    /// 사용자별 커스텀 메뉴를 추가합니다.
    /// SYS_USER_MENU 테이블에 삽입합니다.
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="deptCode">부서 코드</param>
    /// <param name="menu">메뉴 정보</param>
    void AddMenuForUser(string userId, string deptCode, MenuDto menu);
}

    /// <summary>
    /// ���α׷�(ȭ��) ������ �����ϴ� �������丮 �������̽��Դϴ�.
    /// ���α׷� ���, ��⺰ ���α׷� ��ȸ, ���α�ID�� �˻� �� upsert(����/����)�� �����մϴ�.
    /// </summary>
    public interface IProgramRepository
    {
        /// <summary>
        /// ��ϵ� ��� ���α׷� ������ ��ȯ�մϴ�.
        /// </summary>
        List<ProgramDto> GetAllPrograms();

        /// <summary>
        /// ������ moduleId�� ���� ���α׷� ����� ��ȯ�մϴ�.
        /// </summary>
        List<ProgramDto> GetProgramsByModuleId(string moduleId);

        /// <summary>
        /// progId�� ���α׷��� ��ȸ�մϴ�. ������ null�� ��ȯ�� �� �ֽ��ϴ�.
        /// </summary>
        ProgramDto? GetProgramByProgId(string progId);

        /// <summary>
        /// ���α׷� ������ �����ϰų� �����ϸ� ����(upsert)�մϴ�.
        /// </summary>
        void UpsertProgram(ProgramDto program);

        /// <summary>
        /// ������ ��⿡�� ���޵� ���α׷� ��Ͽ� ���� �׸��� ��Ȱ��ȭ�մϴ�.
        /// </summary>
        void DeactivateMissingPrograms(string moduleId, IEnumerable<string> activeProgIds);
    }
}
