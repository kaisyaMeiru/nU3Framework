using nU3.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nU3.Core.Helpers
{
    /// <summary>
    /// [일반사용]
    /// var role = UserRole.Admin;
    /// role.GetDisplayName();        // "관리자"
    /// role.GetDisplayDescription(); // "시스템 관리자"
    /// role.GetDisplayOrder();       // 1
    // [ComboBox 바인딩]
    // cboStatus.DataSource = EnumHelper.GetEnumComboBoxItems<PatientStatus>();
    // cboStatus.DisplayMember = "Text";
    // cboStatus.ValueMember = "Value";
    // [Display Order로 정렬]
    // var departments = EnumHelper.GetEnumValuesByDisplayOrder<Department>();
    /// </summary>
    public static class EnumHelper
    {

        /// <summary>
        /// Enum 값의 Display 특성에서 Name을 가져옵니다.
        /// Display 특성이 없으면 Description 특성을 확인하고, 둘 다 없으면 Enum 값의 이름을 반환합니다.
        /// </summary>
        /// <param name="enumValue">Enum 값</param>
        /// <returns>Display Name 또는 Description 또는 Enum 값 이름</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            if (enumValue == null)
                return string.Empty;

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
                return enumValue.ToString();

            // Display 특성 확인
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name))
                return displayAttribute.Name;

            // Description 특성 확인
            var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null && !string.IsNullOrWhiteSpace(descriptionAttribute.Description))
                return descriptionAttribute.Description;

            // 기본값: Enum 이름
            return enumValue.ToString();
        }

        /// <summary>
        /// Enum 값의 Display 특성에서 Description을 가져옵니다.
        /// </summary>
        /// <param name="enumValue">Enum 값</param>
        /// <returns>Display Description 또는 빈 문자열</returns>
        public static string GetDisplayDescription(this Enum enumValue)
        {
            if (enumValue == null)
                return string.Empty;

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
                return string.Empty;

            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Description ?? string.Empty;
        }

        /// <summary>
        /// Enum 값의 Display 특성에서 ShortName을 가져옵니다.
        /// </summary>
        /// <param name="enumValue">Enum 값</param>
        /// <returns>Display ShortName 또는 Name 또는 Enum 값 이름</returns>
        public static string GetDisplayShortName(this Enum enumValue)
        {
            if (enumValue == null)
                return string.Empty;

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
                return enumValue.ToString();

            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(displayAttribute.ShortName))
                    return displayAttribute.ShortName;
                if (!string.IsNullOrWhiteSpace(displayAttribute.Name))
                    return displayAttribute.Name;
            }

            return enumValue.ToString();
        }

        /// <summary>
        /// Enum 값의 Display 특성에서 Order를 가져옵니다.
        /// </summary>
        /// <param name="enumValue">Enum 값</param>
        /// <returns>Display Order 또는 null</returns>
        public static int? GetDisplayOrder(this Enum enumValue)
        {
            if (enumValue == null)
                return null;

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
                return null;

            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.GetOrder();
        }

        /// <summary>
        /// Enum 타입의 모든 값을 Display Name과 함께 Dictionary로 반환합니다.
        /// </summary>
        /// <typeparam name="TEnum">Enum 타입</typeparam>
        /// <returns>Key: Enum 값, Value: Display Name</returns>
        public static Dictionary<TEnum, string> GetEnumDisplayDictionary<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToDictionary(e => e, e => e.GetDisplayName());
        }

        /// <summary>
        /// Enum 타입의 모든 값을 Display Order에 따라 정렬하여 반환합니다.
        /// </summary>
        /// <typeparam name="TEnum">Enum 타입</typeparam>
        /// <returns>정렬된 Enum 값 리스트</returns>
        public static List<TEnum> GetEnumValuesByDisplayOrder<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .OrderBy(e => e.GetDisplayOrder() ?? int.MaxValue)
                .ThenBy(e => e.GetDisplayName())
                .ToList();
        }

        /// <summary>
        /// Display Name으로 Enum 값을 찾습니다.
        /// </summary>
        /// <typeparam name="TEnum">Enum 타입</typeparam>
        /// <param name="displayName">찾을 Display Name</param>
        /// <param name="ignoreCase">대소문자 무시 여부</param>
        /// <returns>해당하는 Enum 값 또는 null</returns>
        public static TEnum? GetEnumFromDisplayName<TEnum>(string displayName, bool ignoreCase = true) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(displayName))
                return null;

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                if (string.Equals(value.GetDisplayName(), displayName, comparison))
                    return value;
            }

            return null;
        }

        /// <summary>
        /// Enum을 ComboBox/ListBox용 아이템 리스트로 변환합니다.
        /// </summary>
        /// <typeparam name="TEnum">Enum 타입</typeparam>
        /// <returns>Display Name과 Value를 가진 익명 타입 리스트</returns>
        public static List<object> GetEnumComboBoxItems<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .OrderBy(e => e.GetDisplayOrder() ?? int.MaxValue)
                .Select(e => new
                {
                    Text = e.GetDisplayName(),
                    Value = e
                })
                .Cast<object>()
                .ToList();
        }

        /// <summary>
        /// Enum 값이 특정 특성을 가지고 있는지 확인합니다.
        /// </summary>
        /// <typeparam name="TAttribute">확인할 특성 타입</typeparam>
        /// <param name="enumValue">Enum 값</param>
        /// <returns>특성 존재 여부</returns>
        public static bool HasAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            if (enumValue == null)
                return false;

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
                return false;

            return fieldInfo.GetCustomAttribute<TAttribute>() != null;
        }

        /// <summary>
        /// Enum 값의 특정 특성을 가져옵니다.
        /// </summary>
        /// <typeparam name="TAttribute">가져올 특성 타입</typeparam>
        /// <param name="enumValue">Enum 값</param>
        /// <returns>특성 인스턴스 또는 null</returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            if (enumValue == null)
                return null;

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
                return null;

            return fieldInfo.GetCustomAttribute<TAttribute>();
        }
    }
}
