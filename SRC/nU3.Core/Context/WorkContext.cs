using System;
using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Context
{
    /// <summary>
    /// 애플리케이션의 작업 컨텍스트를 표현합니다.
    /// 모듈 간에 공통으로 공유되는 상태(현재 선택된 환자, 검사, 사용자 정보 등)를 보관하고 전달하는 역할을 합니다.
    /// 컨텍스트는 모듈이 활성화될 때 초기값으로 전달되거나, 이벤트를 통해 브로드캐스트되어 다른 모듈이 동일한 상태를 공유하도록 합니다.
    /// </summary>
    public class WorkContext
    {
        /// <summary>
        /// 현재 작업에서 선택된 환자 정보 객체입니다.
        /// 이 값은 모듈 간 환자 변경을 전달하기 위해 사용되며, null일 수 있습니다.
        /// 예: 환자 리스트에서 환자를 클릭하면 이 속성이 해당 환자로 설정됩니다.
        /// </summary>
        public PatientInfoDto CurrentPatient { get; set; }

        /// <summary>
        /// 현재 선택된 검사(검체 오더) 정보입니다.
        /// 검사 상세 화면이나 검사의 결과 조회 등에서 사용됩니다.
        /// </summary>
        public ExamOrderDto CurrentExam { get; set; }

        /// <summary>
        /// 현재 선택된 예약(Appointment) 정보입니다.
        /// 진료 예약 화면이나 스케줄링 관련 모듈에서 참조됩니다.
        /// </summary>
        public AppointmentDto CurrentAppointment { get; set; }

        /// <summary>
        /// 현재 로그인한 사용자에 대한 정보입니다.
        /// 인증/인가 처리 및 화면 권한 결정에 사용됩니다.
        /// </summary>
        public UserInfoDto CurrentUser { get; set; }

        /// <summary>
        /// 현재 컨텍스트가 적용되는 모듈의 권한 집합입니다.
        /// 읽기/쓰기/삭제/출력 등 모듈별 권한 체크에 사용됩니다.
        /// </summary>
        public ModulePermissions Permissions { get; set; }

        /// <summary>
        /// 확장용 추가 데이터 사전입니다. 모듈 간에 특수한 키-값 데이터를 주고받을 때 사용합니다.
        /// 예: 임시 필터 값, 임상 상태 플래그 등 모듈에 특화된 데이터를 보관할 수 있습니다.
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// 이 컨텍스트 객체가 생성된 일시입니다. 디버깅이나 상태 추적에 사용됩니다.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 부모 모듈이나 호출자에서 전달한 파라미터를 보관하는 사전입니다.
        /// 화면 전환 시 모드, 키값, 초기 필터 등 다양한 시나리오에서 사용됩니다.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        public WorkContext()
        {
            AdditionalData = new Dictionary<string, object>();
            Parameters = new Dictionary<string, object>();
            CreatedAt = DateTime.Now;
            Permissions = new ModulePermissions();
        }

        /// <summary>
        /// Parameters 사전에 키-값을 설정합니다. 이미 존재하면 덮어씁니다.
        /// 주로 화면 전환 시 모드(mode), 선택 항목 ID 등을 전달할 때 사용합니다.
        /// </summary>
        /// <param name="key">파라미터 키</param>
        /// <param name="value">저장할 값</param>
        public void SetParameter(string key, object value)
        {
            Parameters[key] = value;
        }

        /// <summary>
        /// Parameters에서 지정한 키의 값을 타입으로 변환하여 반환합니다.
        /// 키가 없거나 형식이 일치하지 않으면 제공한 기본값을 반환합니다.
        /// </summary>
        /// <typeparam name="T">기대하는 반환 타입</typeparam>
        /// <param name="key">파라미터 키</param>
        /// <param name="defaultValue">찾지 못했을 때 반환할 기본값</param>
        /// <returns>해당 키의 값 또는 기본값</returns>
        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// AdditionalData에 키-값을 설정합니다. 모듈 간 확장 데이터를 전달할 때 사용됩니다.
        /// </summary>
        public void SetData(string key, object value)
        {
            AdditionalData[key] = value;
        }

        /// <summary>
        /// AdditionalData에서 지정한 키의 값을 타입으로 변환하여 반환합니다.
        /// 키가 없거나 형식이 일치하지 않으면 제공한 기본값을 반환합니다.
        /// </summary>
        public T GetData<T>(string key, T defaultValue = default)
        {
            if (AdditionalData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// 현재 컨텍스트를 깊이가 아닌 얕은 복사(Shallow copy) 형태로 복제합니다.
        /// 컬렉션은 새로운 Dictionary로 복사하지만 내부 객체들은 참조를 공유합니다.
        /// 이 메서드는 컨텍스트를 수정하지 않고 안전하게 다른 모듈에 전달할 때 유용합니다.
        /// </summary>
        /// <returns>복제된 WorkContext 인스턴스</returns>
        public WorkContext Clone()
        {
            return new WorkContext
            {
                CurrentPatient = this.CurrentPatient,
                CurrentExam = this.CurrentExam,
                CurrentAppointment = this.CurrentAppointment,
                CurrentUser = this.CurrentUser,
                Permissions = this.Permissions?.Clone(),
                AdditionalData = new Dictionary<string, object>(this.AdditionalData),
                Parameters = new Dictionary<string, object>(this.Parameters),
                CreatedAt = this.CreatedAt
            };
        }
    }

    /// <summary>
    /// 모듈별로 적용되는 권한 집합을 표현합니다.
    /// 화면이나 기능별 권한 판별을 위해 사용되며, 필요하면 CustomPermissions에 추가적인 권한을 정의할 수 있습니다.
    /// </summary>
    public class ModulePermissions
    {
        /// <summary>읽기 권한 허용 여부(기본 true)</summary>
        public bool CanRead { get; set; } = true;
        /// <summary>생성(추가) 권한 여부</summary>
        public bool CanCreate { get; set; } = false;
        /// <summary>수정 권한 여부</summary>
        public bool CanUpdate { get; set; } = false;
        /// <summary>삭제 권한 여부</summary>
        public bool CanDelete { get; set; } = false;
        /// <summary>출력(프린트) 권한 여부</summary>
        public bool CanPrint { get; set; } = false;
        /// <summary>데이터/리포트 내보내기 권한 여부</summary>
        public bool CanExport { get; set; } = false;
        /// <summary>승인(Approve) 권한 여부</summary>
        public bool CanApprove { get; set; } = false;
        /// <summary>취소(Cancel) 권한 여부</summary>
        public bool CanCancel { get; set; } = false;

        /// <summary>
        /// 모듈별로 추가적으로 정의할 수 있는 사용자 정의 권한 맵입니다.
        /// 키는 권한 명칭, 값은 허용 여부입니다.
        /// </summary>
        public Dictionary<string, bool> CustomPermissions { get; set; }

        public ModulePermissions()
        {
            CustomPermissions = new Dictionary<string, bool>();
        }

        /// <summary>
        /// CustomPermissions에 항목을 설정합니다. 존재하면 덮어씁니다.
        /// </summary>
        public void SetCustomPermission(string key, bool value)
        {
            CustomPermissions[key] = value;
        }

        /// <summary>
        /// 특정 사용자 정의 권한의 허용 여부를 반환합니다.
        /// 키가 없으면 false를 반환합니다.
        /// </summary>
        public bool HasCustomPermission(string key)
        {
            return CustomPermissions.TryGetValue(key, out var value) && value;
        }

        /// <summary>
        /// 권한 객체를 복제하여 새로운 인스턴스로 반환합니다.
        /// CustomPermissions는 새로운 Dictionary로 복사됩니다(내부 키/값은 동일한 참조 유지).
        /// </summary>
        public ModulePermissions Clone()
        {
            return new ModulePermissions
            {
                CanRead = this.CanRead,
                CanCreate = this.CanCreate,
                CanUpdate = this.CanUpdate,
                CanDelete = this.CanDelete,
                CanPrint = this.CanPrint,
                CanExport = this.CanExport,
                CanApprove = this.CanApprove,
                CanCancel = this.CanCancel,
                CustomPermissions = new Dictionary<string, bool>(this.CustomPermissions)
            };
        }

        /// <summary>
        /// 모든 권한을 비활성화(해제)합니다. CustomPermissions도 모두 제거됩니다.
        /// 화면 초기화나 권한 리셋 시 사용됩니다.
        /// </summary>
        public void ClearAll()
        {
            CanRead = false;
            CanCreate = false;
            CanUpdate = false;
            CanDelete = false;
            CanPrint = false;
            CanExport = false;
            CanApprove = false;
            CanCancel = false;
            CustomPermissions.Clear();
        }

        /// <summary>
        /// 모든 표준 권한을 허용 상태로 설정합니다. 관리 권한 부여 시 사용됩니다.
        /// </summary>
        public void GrantAll()
        {
            CanRead = true;
            CanCreate = true;
            CanUpdate = true;
            CanDelete = true;
            CanPrint = true;
            CanExport = true;
            CanApprove = true;
            CanCancel = true;
        }
    }

    /// <summary>
    /// 컨텍스트 변경을 설명하는 이벤트 인자 타입입니다.
    /// OldContext와 NewContext를 통해 변경 전후 상태를 비교할 수 있으며,
    /// ChangedProperty를 통해 어떤 속성이 변경되었는지 부가 정보를 제공할 수 있습니다.
    /// </summary>
    public class ContextChangedEventArgs : EventArgs
    {
        /// <summary>변경 전 컨텍스트</summary>
        public WorkContext OldContext { get; set; }
        /// <summary>변경 후 컨텍스트</summary>
        public WorkContext NewContext { get; set; }
        /// <summary>변경된 속성 이름(예: "CurrentPatient")</summary>
        public string ChangedProperty { get; set; }
        /// <summary>변경 전 값(선택적)</summary>
        public object OldValue { get; set; }
        /// <summary>변경 후 값(선택적)</summary>
        public object NewValue { get; set; }
        /// <summary>변경 시각</summary>
        public DateTime ChangedAt { get; set; }

        public ContextChangedEventArgs()
        {
            ChangedAt = DateTime.Now;
        }
    }
}
