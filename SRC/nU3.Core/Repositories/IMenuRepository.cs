using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Repositories
{
    /// <summary>
    /// 메뉴 및 프로그램 메타데이터에 접근하기 위한 리포지토리 인터페이스입니다.
    /// 메뉴 트리, 프로그램 등록 정보 등을 DB에서 조회/갱신하는 책임을 가집니다.
    /// </summary>
    public interface IMenuRepository
    {
        /// <summary>
        /// 모든 메뉴 항목을 반환합니다. 메뉴 트리 구성에 사용됩니다.
        /// </summary>
        List<MenuDto> GetAllMenus();

        /// <summary>
        /// 메뉴 테이블을 모두 삭제(초기화)합니다. 주로 재구성 시 사용됩니다.
        /// </summary>
        void DeleteAllMenus();

        /// <summary>
        /// 메뉴 항목을 추가합니다.
        /// </summary>
        void AddMenu(MenuDto menu);
    }

    /// <summary>
    /// 프로그램(화면) 정보를 관리하는 리포지토리 인터페이스입니다.
    /// 프로그램 목록, 모듈별 프로그램 조회, 프로그ID로 검색 및 upsert(삽입/갱신)를 제공합니다.
    /// </summary>
    public interface IProgramRepository
    {
        /// <summary>
        /// 등록된 모든 프로그램 정보를 반환합니다.
        /// </summary>
        List<ProgramDto> GetAllPrograms();

        /// <summary>
        /// 지정한 moduleId에 속한 프로그램 목록을 반환합니다.
        /// </summary>
        List<ProgramDto> GetProgramsByModuleId(string moduleId);

        /// <summary>
        /// progId로 프로그램을 조회합니다. 없으면 null을 반환할 수 있습니다.
        /// </summary>
        ProgramDto? GetProgramByProgId(string progId);

        /// <summary>
        /// 프로그램 정보를 삽입하거나 존재하면 갱신(upsert)합니다.
        /// </summary>
        void UpsertProgram(ProgramDto program);

        /// <summary>
        /// 지정된 모듈에서 전달된 프로그램 목록에 없는 항목을 비활성화합니다.
        /// </summary>
        void DeactivateMissingPrograms(string moduleId, IEnumerable<string> activeProgIds);
    }
}
