namespace nU3.Modules.EMR.Common.Constants
{
    /// <summary>
    /// EMR 도메인 공통 상수
    /// </summary>
    public static class EmrConstants
    {
        public static class Departments
        {
            public const string InternalMedicine = "IM";
            public const string Surgery = "GS";
            public const string Orthopedics = "OS";
            public const string Pediatrics = "PED";
            public const string Emergency = "ER";
        }

        public static class Status
        {
            public const string Admitted = "ADMITTED";
            public const string Discharged = "DISCHARGED";
            public const string Outpatient = "OUTPATIENT";
            public const string Emergency = "EMERGENCY";
        }

        public static class Colors
        {
            // EMR 전용 색상 테마
            public const string MainColor = "#0078D7";
            public const string AlertColor = "#FF5722";
            public const string InfoColor = "#2196F3";
        }
    }
}
