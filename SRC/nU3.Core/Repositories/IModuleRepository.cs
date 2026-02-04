using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Repositories
{
    /// <summary>
    /// 모듈(Components) 메타데이터 및 버전 정보를 저장/조회하는 리포지토리 인터페이스입니다.
    ///
    /// 책임:
    /// - 모듈 마스터 정보(ModuleMstDto) 조회/저장/삭제
    /// - 모듈 버전 정보(ModuleVerDto) 조회 및 버전 관리(활성 버전 조회, 구버전 비활성화 등)
    ///
    /// 구현체는 실제 데이터 소스(DB, 파일 등)에 따라 구체적으로 동작하며,
    /// UnitTest를 위해 목(Mock) 구현을 제공할 수 있습니다.
    /// </summary>
    public interface IModuleRepository
    {
        // ==================== Module Master ====================

        /// <summary>
        /// 모든 모듈의 마스터 정보를 조회합니다.
        /// UI 목록 표시나 업데이트 검사 시 사용됩니다.
        /// </summary>
        List<ModuleMstDto> GetAllModules();

        /// <summary>
        /// 지정한 moduleId에 해당하는 마스터 정보를 조회합니다.
        /// 존재하지 않으면 null을 반환할 수 있습니다.
        /// </summary>
        ModuleMstDto GetModule(string moduleId);

        /// <summary>
        /// 모듈 정보를 저장합니다. 존재하면 갱신(update), 없으면 삽입(insert) 방식으로 동작해야 합니다.
        /// </summary>
        void SaveModule(ModuleMstDto module);

        /// <summary>
        /// 지정한 moduleId에 해당하는 모듈 마스터 정보를 삭제합니다.
        /// 보통 관리자가 모듈을 제거할 때 호출됩니다.
        /// </summary>
        void DeleteModule(string moduleId);


        // ==================== Module Version ====================

        /// <summary>
        /// 시스템에서 활성(Active)으로 표시된 모듈 버전 목록을 반환합니다.
        /// 모듈 배포/업데이트 시 비교용으로 사용됩니다.
        /// </summary>
        List<ModuleVerDto> GetActiveVersions();

        /// <summary>
        /// 지정한 모듈의 이전(구) 버전들을 비활성화 처리합니다.
        /// 새 버전 등록 전/후에 호출하여 활성 버전 관리를 수행합니다.
        /// </summary>
        void DeactivateOldVersions(string moduleId);

        /// <summary>
        /// 새로운 모듈 버전을 추가합니다(버전 히스토리에 삽입).
        /// 이 메서드는 보통 파일 업로드 후 호출되어 서버 저장소의 메타데이터를 갱신합니다.
        /// </summary>
        void AddVersion(ModuleVerDto version);
    }
}
