using System;

namespace nU3.Models
{
    public class DateRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsEmpty => !StartDate.HasValue && !EndDate.HasValue;

        public bool IsValid => StartDate.HasValue && EndDate.HasValue && StartDate <= EndDate;

        public int? Days
        {
            get
            {
                if (!StartDate.HasValue || !EndDate.HasValue) return null;
                return (EndDate.Value - StartDate.Value).Days;
            }
        }

        public override string ToString()
        {
            if (IsEmpty) return "";
            if (StartDate.HasValue && !EndDate.HasValue) return $"{StartDate:yyyy-MM-dd} ~";
            if (!StartDate.HasValue && EndDate.HasValue) return $"~ {EndDate:yyyy-MM-dd}";
            return $"{StartDate:yyyy-MM-dd} ~ {EndDate:yyyy-MM-dd}";
        }

        public static DateRange Today => new() { StartDate = DateTime.Today, EndDate = DateTime.Today };
        public static DateRange ThisWeek => GetWeekRange(DateTime.Today);
        public static DateRange ThisMonth => GetMonthRange(DateTime.Today);

        private static DateRange GetWeekRange(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var monday = date.AddDays(-(dayOfWeek == 0 ? 6 : dayOfWeek - 1));
            var sunday = monday.AddDays(6);
            return new DateRange { StartDate = monday, EndDate = sunday };
        }

        private static DateRange GetMonthRange(DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            return new DateRange { StartDate = firstDay, EndDate = lastDay };
        }
    }
}
