using System;
using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Context
{
    /// <summary>
    /// 애플리케이션의 작업 컨텍스트를 표현합니다.
    /// 모듈 간에 공통으로 공유되는 상태(현재 선택된 환자, 검사, 사용자 등)를 관리합니다.
    /// </summary>
    public class WorkContext
    {
        /// <summary>
        /// 현재 로그인한 사용자 정보
        /// </summary>
        public UserInfoDto CurrentUser { get; set; }

        /// <summary>
        /// 현재 선택된 환자 정보
        /// </summary>
        public PatientInfoDto CurrentPatient { get; set; }

        /// <summary>
        /// 현재 선택된 검사 정보
        /// </summary>
        public ExamOrderDto CurrentExam { get; set; }

        /// <summary>
        /// 현재 모듈에 대한 권한 정보
        /// </summary>
        public ModulePermissions Permissions { get; set; }

        /// <summary>
        /// 모듈 내부 통신을 위한 커스텀 데이터 (임의의 키/값 저장소)
        /// 필요에 따라 추가적인 정보를 모듈 간에 전달할 때 사용합니다.
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        public WorkContext()
        {
            CustomData = new Dictionary<string, object>();
            Permissions = new ModulePermissions();
        }

        /// <summary>
        /// 현재 컨텍스트의 복본을 생성합니다.
        /// (얕은 복사 수행, 참조 타입 객체는 공유됨)
        /// </summary>
        public WorkContext Clone()
        {
            var clone = new WorkContext
            {
                CurrentUser = this.CurrentUser,
                CurrentPatient = this.CurrentPatient,
                CurrentExam = this.CurrentExam,
                Permissions = this.Permissions?.Clone(), // Deep copy permissions as they are modified per module
                CustomData = new Dictionary<string, object>(this.CustomData)
            };
            return clone;
        }
    }

    /// <summary>
    /// 모듈별 접근 권한을 정의합니다.
    /// </summary>
    public class ModulePermissions
    {
        public bool CanRead { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public bool CanExport { get; set; }
        public bool CanApprove { get; set; }
        public bool CanCancel { get; set; }

        /// <summary>
        /// 모든 권한을 허용으로 설정합니다 (관리자용 편의 메서드)
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

        public ModulePermissions Clone()
        {
            return (ModulePermissions)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// 컨텍스트 변경 이벤트 인자
    /// </summary>
    public class ContextChangedEventArgs : EventArgs
    {
        public string ChangedProperty { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public DateTime ChangedAt { get; set; }

        public ContextChangedEventArgs(string propertyName, object oldValue, object newValue)
        {
            ChangedProperty = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
            ChangedAt = DateTime.Now;
        }
    }
}
