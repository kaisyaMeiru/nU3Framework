using nU3.Models;

namespace nU3.Tools.Deployer.Models
{
    public class ModuleCompareRow
    {
        public ModuleMstDto Master { get; set; } = new ModuleMstDto();
        public ModuleVerDto? ActiveVersion { get; set; }
        public ModuleFileItem? ScannedFile { get; set; }

        public string ModuleId => Master.ModuleId;
        public string Category => Master.Category;
        public string SubSystem => Master.SubSystem;
        public string ModuleName => Master.ModuleName;
        public string FileName => Master.FileName;

        public string DbActiveVersion => ActiveVersion?.Version ?? string.Empty;
        public string ScannedVersion => ScannedFile?.Version ?? string.Empty;

        public bool IsDifferentVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DbActiveVersion) || string.IsNullOrWhiteSpace(ScannedVersion)) return false;
                return !string.Equals(DbActiveVersion, ScannedVersion, System.StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
