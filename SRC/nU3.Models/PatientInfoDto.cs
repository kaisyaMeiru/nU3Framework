using System;

namespace nU3.Models
{
    /// <summary>
    /// ȯ�� �⺻ ���� ��
    /// </summary>
    public class PatientInfoDto
    {
        /// <summary>
        /// 환자 ID (Primary Key)
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 입원번호
        /// </summary>
        public string InNumber { get; set; }

        /// <summary>
        /// 환자 ID (Primary Key)
        /// </summary>
        public string ChartNo { get; set; }

        /// <summary>
        /// ȯ�� �̸�
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// �ֹε�Ϲ�ȣ (��ȣȭ)
        /// </summary>
        public string ResidentNo { get; set; }

        /// <summary>
        /// 성별 (0:Unspecified, 1:Male, 2:Female, 9:Other)
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 진료과 이름
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 담당의 이름
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// 병실 번호
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 입원일
        /// </summary>
        public DateTime AdmDate { get; set; }

        /// <summary>
        /// 담당의 ID
        /// </summary>
        public string DoctorID { get; set; }

        /// <summary>
        /// 진료과 ID
        /// </summary>
        public string DeptID { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// ���� (���)
        /// </summary>
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// ������ (0:Unknown, 1:A+, 2:A-, 3:B+, 4:B-, 5:O+, 6:O-, 7:AB+, 8:AB-)
        /// </summary>
        public int BloodType { get; set; }

        /// <summary>
        /// ��ȭ��ȣ
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// �޴���ȭ
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// �̸���
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ������ȣ
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// �ּ�
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// �� �ּ�
        /// </summary>
        public string AddressDetail { get; set; }

        /// <summary>
        /// ���� ���� (1:�ǰ�����, 2:�Ƿ�޿�, 3:����, 4:�ڵ���, 5:�Ǽ�, 6:����, 99:��޿�)
        /// </summary>
        public int InsuranceType { get; set; }

        /// <summary>
        /// ���� ��ȣ
        /// </summary>
        public string InsuranceNo { get; set; }

        /// <summary>
        /// ��ȣ�� �̸�
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// ��ȣ�� ����
        /// </summary>
        public string GuardianRelation { get; set; }

        /// <summary>
        /// ��ȣ�� ����ó
        /// </summary>
        public string GuardianPhone { get; set; }

        /// <summary>
        /// ��󿬶�ó
        /// </summary>
        public string EmergencyContact { get; set; }

        /// <summary>
        /// ��󿬶�ó ����
        /// </summary>
        public string EmergencyRelation { get; set; }

        /// <summary>
        /// �˷����� ����
        /// </summary>
        public string Allergies { get; set; }

        /// <summary>
        /// ������ȯ ����
        /// </summary>
        public string ChronicDiseases { get; set; }

        /// <summary>
        /// ���� ���� �๰
        /// </summary>
        public string CurrentMedications { get; set; }

        /// <summary>
        /// ���� ����
        /// </summary>
        public bool IsSmoker { get; set; }

        /// <summary>
        /// ���� ����
        /// </summary>
        public bool IsDrinker { get; set; }

        /// <summary>
        /// ȯ�� ���� (0:Waiting, 1:InProgress, 2:Completed, 3:OnHold, 4:Cancelled, 10:Admitted, 11:Discharged)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// VIP ����
        /// </summary>
        public bool IsVIP { get; set; }

        /// <summary>
        /// ��� ����
        /// </summary>
        public bool IsDeceased { get; set; }

        /// <summary>
        /// �����
        /// </summary>
        public DateTime? DeceasedDate { get; set; }

        /// <summary>
        /// ���� �����
        /// </summary>
        public DateTime RegisteredDate { get; set; }

        /// <summary>
        /// ���� �����
        /// </summary>
        public string RegisteredBy { get; set; }

        /// <summary>
        /// ������ �湮��
        /// </summary>
        public DateTime? LastVisitDate { get; set; }

        /// <summary>
        /// �����Ͻ�
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public string Remarks { get; set; }
    }
}
