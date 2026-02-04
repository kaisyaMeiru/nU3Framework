using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Repositories
{
    /// <summary>
    /// Framework 구성요소(Components) 관련 메타데이터 및 버전 관리를 위한 리포지토리 인터페이스입니다.
    /// 
    /// 책임:
    /// - 구성요소 마스터(ComponentMstDto) CRUD
    /// - 구성요소 버전(ComponentVerDto) 기록 및 활성화 상태 관리
    /// - 클라이언트와 비교하여 업데이트 대상 및 누락된 컴포넌트 검사
    /// 
    /// 구현체는 데이터 소스(DB, 파일 등)에 맞춰 동작해야 하며, 테스트용 목(Mock) 구현을 제공할 수 있습니다.
    /// </summary>
    public interface IComponentRepository
    {
        #region Component Master

        /// <summary>
        /// 모든 구성요소의 마스터 정보를 반환합니다.
        /// UI 목록 표시나 배포 검사에 사용됩니다.
        /// </summary>
        List<ComponentMstDto> GetAllComponents();

        /// <summary>
        /// 지정한 유형(type)에 해당하는 구성요소 목록을 반환합니다.
        /// </summary>
        List<ComponentMstDto> GetComponentsByType(ComponentType type);

        /// <summary>
        /// 지정한 그룹(groupName)에 속한 구성요소 목록을 반환합니다.
        /// 그룹은 배포/관리 편의를 위한 분류 정보입니다.
        /// </summary>
        List<ComponentMstDto> GetComponentsByGroup(string groupName);

        /// <summary>
        /// 필수(Required)로 지정된 구성요소 목록을 반환합니다.
        /// 부팅 시 또는 설치 시 반드시 필요한 컴포넌트를 확인하는 데 사용됩니다.
        /// </summary>
        List<ComponentMstDto> GetRequiredComponents();

        /// <summary>
        /// componentId에 해당하는 구성요소 마스터 정보를 반환합니다. 존재하지 않으면 null을 반환할 수 있습니다.
        /// </summary>
        ComponentMstDto GetComponent(string componentId);

        /// <summary>
        /// 구성요소 마스터 정보를 저장(삽입 또는 갱신)합니다.
        /// </summary>
        void SaveComponent(ComponentMstDto component);

        /// <summary>
        /// 구성요소를 삭제합니다(관리자 기능).
        /// </summary>
        void DeleteComponent(string componentId);

        #endregion

        #region Component Version

        /// <summary>
        /// 모든 구성요소의 활성 버전 목록을 반환합니다.
        /// 클라이언트 업데이트 검사에 사용됩니다.
        /// </summary>
        List<ComponentVerDto> GetActiveVersions();

        /// <summary>
        /// 지정한 유형(type)에 해당하는 구성요소의 활성 버전 목록을 반환합니다.
        /// </summary>
        List<ComponentVerDto> GetActiveVersionsByType(ComponentType type);

        /// <summary>
        /// 특정 구성요소의 활성 버전을 반환합니다.
        /// </summary>
        ComponentVerDto GetActiveVersion(string componentId);

        /// <summary>
        /// 특정 구성요소의 모든 버전(비활성 포함) 히스토리를 반환합니다.
        /// </summary>
        List<ComponentVerDto> GetVersionHistory(string componentId);

        /// <summary>
        /// 새로운 버전 정보를 추가합니다.
        /// </summary>
        void AddVersion(ComponentVerDto version);

        /// <summary>
        /// 지정한 구성요소의 이전 버전들을 비활성화 처리(soft delete)합니다.
        /// 보통 새 버전 등록 전에 호출하여 활성 버전이 단 하나만 존재하도록 합니다.
        /// </summary>
        void DeactivateOldVersions(string componentId);

        /// <summary>
        /// 지정한 구성요소의 활성 버전을 수동으로 설정합니다.
        /// </summary>
        void SetActiveVersion(string componentId, string version);

        #endregion

        #region Update Check

        /// <summary>
        /// 클라이언트에 설치된 컴포넌트 목록(clientComponents)을 비교하여 업데이트가 필요한 항목을 반환합니다.
        /// 반환되는 항목은 서버에서 활성 버전이 존재하고 클라이언트 버전보다 높은 컴포넌트들입니다.
        /// </summary>
        List<ComponentVerDto> CheckForUpdates(List<ClientComponentDto> clientComponents);

        /// <summary>
        /// 클라이언트에 없는(누락된) 구성요소를 반환합니다.
        /// 설치 마감 또는 초기 설치 시 사용됩니다.
        /// </summary>
        List<ComponentVerDto> GetMissingComponents(List<ClientComponentDto> clientComponents);

        #endregion
    }
}
