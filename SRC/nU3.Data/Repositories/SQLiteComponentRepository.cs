using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using nU3.Models;
using nU3.Core.Repositories;
using nU3.Connectivity;
using nU3.Core.Interfaces;

namespace nU3.Data.Repositories
{
    /// <summary>
    /// SQLite implementation of IComponentRepository
    /// </summary>
    public class SQLiteComponentRepository : IComponentRepository
    {
        private readonly IDBAccessService _db;

        public SQLiteComponentRepository(IDBAccessService db)
        {
            _db = db;
        }

        #region Component Master

        public List<ComponentMstDto> GetAllComponents()
        {
            string sql = @"
                SELECT * FROM SYS_COMPONENT_MST 
                WHERE IS_ACTIVE = 'Y'
                ORDER BY PRIORITY, GROUP_NAME, COMPONENT_NAME";
            return GetMstList(sql);
        }

        public List<ComponentMstDto> GetComponentsByType(ComponentType type)
        {
            string sql = @"
                SELECT * FROM SYS_COMPONENT_MST 
                WHERE COMPONENT_TYPE = @type AND IS_ACTIVE = 'Y'
                ORDER BY PRIORITY, COMPONENT_NAME";
            return GetMstList(sql, new Dictionary<string, object> { { "@type", (int)type } });
        }

        public List<ComponentMstDto> GetComponentsByGroup(string groupName)
        {
            string sql = @"
                SELECT * FROM SYS_COMPONENT_MST 
                WHERE GROUP_NAME = @group AND IS_ACTIVE = 'Y'
                ORDER BY PRIORITY, COMPONENT_NAME";
            return GetMstList(sql, new Dictionary<string, object> { { "@group", groupName } });
        }

        public List<ComponentMstDto> GetRequiredComponents()
        {
            string sql = @"
                SELECT * FROM SYS_COMPONENT_MST 
                WHERE IS_REQUIRED = 1 AND IS_ACTIVE = 'Y'
                ORDER BY PRIORITY, COMPONENT_NAME";
            return GetMstList(sql);
        }

        public ComponentMstDto GetComponent(string componentId)
        {
            string sql = "SELECT * FROM SYS_COMPONENT_MST WHERE COMPONENT_ID = @id";
            var list = GetMstList(sql, new Dictionary<string, object> { { "@id", componentId } });
            return list.FirstOrDefault();
        }

        public void SaveComponent(ComponentMstDto component)
        {
            string sql = @"
                INSERT INTO SYS_COMPONENT_MST (
                    COMPONENT_ID, COMPONENT_TYPE, COMPONENT_NAME, FILE_NAME,
                    INSTALL_PATH, GROUP_NAME, IS_REQUIRED, AUTO_UPDATE,
                    DESCRIPTION, PRIORITY, DEPENDENCIES, IS_ACTIVE
                ) VALUES (
                    @id, @type, @name, @file,
                    @path, @group, @required, @auto,
                    @desc, @priority, @deps, @active
                )
                ON CONFLICT(COMPONENT_ID) DO UPDATE SET
                    COMPONENT_TYPE = @type,
                    COMPONENT_NAME = @name,
                    FILE_NAME = @file,
                    INSTALL_PATH = @path,
                    GROUP_NAME = @group,
                    IS_REQUIRED = @required,
                    AUTO_UPDATE = @auto,
                    DESCRIPTION = @desc,
                    PRIORITY = @priority,
                    DEPENDENCIES = @deps,
                    IS_ACTIVE = @active,
                    MOD_DATE = CURRENT_TIMESTAMP";

            var parameters = new Dictionary<string, object>
            {
                { "@id", component.ComponentId },
                { "@type", (int)component.ComponentType },
                { "@name", component.ComponentName },
                { "@file", component.FileName },
                { "@path", (object)component.InstallPath ?? DBNull.Value },
                { "@group", (object)component.GroupName ?? DBNull.Value },
                { "@required", component.IsRequired ? 1 : 0 },
                { "@auto", component.AutoUpdate ? 1 : 0 },
                { "@desc", (object)component.Description ?? DBNull.Value },
                { "@priority", component.Priority },
                { "@deps", (object)component.Dependencies ?? DBNull.Value },
                { "@active", component.IsActive ?? "Y" }
            };

            _db.ExecuteNonQuery(sql, parameters);
        }

        public void DeleteComponent(string componentId)
        {
            string sql = "UPDATE SYS_COMPONENT_MST SET IS_ACTIVE = 'N', MOD_DATE = CURRENT_TIMESTAMP WHERE COMPONENT_ID = @id";
            _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@id", componentId } });
        }

        #endregion

        #region Component Version

        public List<ComponentVerDto> GetActiveVersions()
        {
            string sql = @"
                SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                FROM SYS_COMPONENT_VER v
                JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                WHERE v.IS_ACTIVE = 'Y' AND v.DEL_DATE IS NULL AND m.IS_ACTIVE = 'Y'
                ORDER BY m.PRIORITY, m.COMPONENT_NAME";
            return GetVerList(sql);
        }

        public List<ComponentVerDto> GetActiveVersionsByType(ComponentType type)
        {
            string sql = @"
                SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                FROM SYS_COMPONENT_VER v
                JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                WHERE v.IS_ACTIVE = 'Y' AND v.DEL_DATE IS NULL 
                  AND m.IS_ACTIVE = 'Y' AND m.COMPONENT_TYPE = @type
                ORDER BY m.PRIORITY, m.COMPONENT_NAME";
            return GetVerList(sql, new Dictionary<string, object> { { "@type", (int)type } });
        }

        public ComponentVerDto GetActiveVersion(string componentId)
        {
            string sql = @"
                SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                FROM SYS_COMPONENT_VER v
                JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                WHERE v.COMPONENT_ID = @id AND v.IS_ACTIVE = 'Y' AND v.DEL_DATE IS NULL";
            return GetVerList(sql, new Dictionary<string, object> { { "@id", componentId } }).FirstOrDefault();
        }

        public List<ComponentVerDto> GetVersionHistory(string componentId)
        {
            string sql = @"
                SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                FROM SYS_COMPONENT_VER v
                JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                WHERE v.COMPONENT_ID = @id
                ORDER BY v.REG_DATE DESC";
            return GetVerList(sql, new Dictionary<string, object> { { "@id", componentId } });
        }

        public void AddVersion(ComponentVerDto version)
        {
            string sql = @"
                INSERT INTO SYS_COMPONENT_VER (
                    COMPONENT_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH,
                    MIN_FRAMEWORK_VER, MAX_FRAMEWORK_VER, DEPLOY_DESC, RELEASE_NOTE_URL, IS_ACTIVE
                ) VALUES (
                    @id, @ver, @hash, @size, @path,
                    @minVer, @maxVer, @desc, @note, @active
                )
                ON CONFLICT(COMPONENT_ID, VERSION) DO UPDATE SET
                    FILE_HASH = @hash,
                    FILE_SIZE = @size,
                    STORAGE_PATH = @path,
                    MIN_FRAMEWORK_VER = @minVer,
                    MAX_FRAMEWORK_VER = @maxVer,
                    DEPLOY_DESC = @desc,
                    RELEASE_NOTE_URL = @note,
                    IS_ACTIVE = @active,
                    DEL_DATE = NULL";

            var parameters = new Dictionary<string, object>
            {
                { "@id", version.ComponentId },
                { "@ver", version.Version },
                { "@hash", version.FileHash },
                { "@size", version.FileSize },
                { "@path", version.StoragePath },
                { "@minVer", (object)version.MinFrameworkVersion ?? DBNull.Value },
                { "@maxVer", (object)version.MaxFrameworkVersion ?? DBNull.Value },
                { "@desc", (object)version.DeployDesc ?? DBNull.Value },
                { "@note", (object)version.ReleaseNoteUrl ?? DBNull.Value },
                { "@active", version.IsActive ?? "Y" }
            };

            _db.ExecuteNonQuery(sql, parameters);
        }

        public void DeactivateOldVersions(string componentId)
        {
            string sql = @"UPDATE SYS_COMPONENT_VER 
                          SET IS_ACTIVE = 'N', DEL_DATE = CURRENT_TIMESTAMP 
                          WHERE COMPONENT_ID = @id AND IS_ACTIVE = 'Y'";
            _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@id", componentId } });
        }

        public void SetActiveVersion(string componentId, string version)
        {
            _db.BeginTransaction();
            try
            {
                // Deactivate all versions
                _db.ExecuteNonQuery("UPDATE SYS_COMPONENT_VER SET IS_ACTIVE = 'N' WHERE COMPONENT_ID = @id", 
                    new Dictionary<string, object> { { "@id", componentId } });

                // Activate specific version
                _db.ExecuteNonQuery(
                    @"UPDATE SYS_COMPONENT_VER 
                      SET IS_ACTIVE = 'Y', DEL_DATE = NULL 
                      WHERE COMPONENT_ID = @id AND VERSION = @ver",
                    new Dictionary<string, object> { { "@id", componentId }, { "@ver", version } });

                _db.CommitTransaction();
            }
            catch
            {
                _db.RollbackTransaction();
                throw;
            }
        }

        #endregion

        #region Update Check

        public List<ComponentVerDto> CheckForUpdates(List<ClientComponentDto> clientComponents)
        {
            var serverVersions = GetActiveVersions();
            var updates = new List<ComponentVerDto>();

            foreach (var serverVer in serverVersions)
            {
                var clientVer = clientComponents.FirstOrDefault(c => 
                    c.ComponentId == serverVer.ComponentId);

                if (clientVer == null)
                {
                    updates.Add(serverVer);
                }
                else if (CompareVersions(serverVer.Version, clientVer.InstalledVersion) > 0)
                {
                    updates.Add(serverVer);
                }
                else if (serverVer.FileHash != clientVer.FileHash)
                {
                    updates.Add(serverVer);
                }
            }

            return updates;
        }

        public List<ComponentVerDto> GetMissingComponents(List<ClientComponentDto> clientComponents)
        {
            var serverVersions = GetActiveVersions();
            var clientIds = clientComponents.Select(c => c.ComponentId).ToHashSet();

            return serverVersions
                .Where(v => !clientIds.Contains(v.ComponentId))
                .ToList();
        }

        #endregion

        #region Private Helpers

        private List<ComponentMstDto> GetMstList(string sql, Dictionary<string, object> parameters = null)
        {
            var list = new List<ComponentMstDto>();
            using (var dt = _db.ExecuteDataTable(sql, parameters))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapToComponentMst(row));
                }
            }
            return list;
        }

        private List<ComponentVerDto> GetVerList(string sql, Dictionary<string, object> parameters = null)
        {
            var list = new List<ComponentVerDto>();
            using (var dt = _db.ExecuteDataTable(sql, parameters))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapToComponentVer(row));
                }
            }
            return list;
        }

        private static ComponentMstDto MapToComponentMst(DataRow row)
        {
            return new ComponentMstDto
            {
                ComponentId = row["COMPONENT_ID"].ToString(),
                ComponentType = (ComponentType)Convert.ToInt32(row["COMPONENT_TYPE"]),
                ComponentName = row["COMPONENT_NAME"].ToString(),
                FileName = row["FILE_NAME"].ToString(),
                InstallPath = row["INSTALL_PATH"] == DBNull.Value ? null : row["INSTALL_PATH"].ToString(),
                GroupName = row["GROUP_NAME"] == DBNull.Value ? null : row["GROUP_NAME"].ToString(),
                IsRequired = Convert.ToInt32(row["IS_REQUIRED"]) == 1,
                AutoUpdate = Convert.ToInt32(row["AUTO_UPDATE"]) == 1,
                Description = row["DESCRIPTION"] == DBNull.Value ? null : row["DESCRIPTION"].ToString(),
                Priority = Convert.ToInt32(row["PRIORITY"]),
                Dependencies = row["DEPENDENCIES"] == DBNull.Value ? null : row["DEPENDENCIES"].ToString(),
                RegDate = ParseDate(row["REG_DATE"]) ?? DateTime.Now,
                ModDate = ParseDate(row["MOD_DATE"]),
                IsActive = row["IS_ACTIVE"] == DBNull.Value ? "Y" : row["IS_ACTIVE"].ToString()
            };
        }

        private static ComponentVerDto MapToComponentVer(DataRow row)
        {
            return new ComponentVerDto
            {
                ComponentId = row["COMPONENT_ID"].ToString(),
                Version = row["VERSION"].ToString(),
                FileHash = row["FILE_HASH"].ToString(),
                FileSize = Convert.ToInt64(row["FILE_SIZE"]),
                StoragePath = row["STORAGE_PATH"].ToString(),
                MinFrameworkVersion = row["MIN_FRAMEWORK_VER"] == DBNull.Value ? null : row["MIN_FRAMEWORK_VER"].ToString(),
                MaxFrameworkVersion = row["MAX_FRAMEWORK_VER"] == DBNull.Value ? null : row["MAX_FRAMEWORK_VER"].ToString(),
                DeployDesc = row["DEPLOY_DESC"] == DBNull.Value ? null : row["DEPLOY_DESC"].ToString(),
                ReleaseNoteUrl = row["RELEASE_NOTE_URL"] == DBNull.Value ? null : row["RELEASE_NOTE_URL"].ToString(),
                RegDate = ParseDate(row["REG_DATE"]) ?? DateTime.Now,
                DelDate = ParseDate(row["DEL_DATE"]),
                IsActive = row["IS_ACTIVE"] == DBNull.Value ? "Y" : row["IS_ACTIVE"].ToString(),
                // Joined fields
                ComponentType = row.Table.Columns.Contains("COMPONENT_TYPE") ? (ComponentType)Convert.ToInt32(row["COMPONENT_TYPE"]) : ComponentType.Other,
                ComponentName = row.Table.Columns.Contains("COMPONENT_NAME") ? row["COMPONENT_NAME"].ToString() : null,
                InstallPath = row.Table.Columns.Contains("INSTALL_PATH") ? row["INSTALL_PATH"] == DBNull.Value ? null : row["INSTALL_PATH"].ToString() : null,
                GroupName = row.Table.Columns.Contains("GROUP_NAME") ? row["GROUP_NAME"] == DBNull.Value ? null : row["GROUP_NAME"].ToString() : null
            };
        }

        private static DateTime? ParseDate(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            string str = value.ToString();
            if (string.IsNullOrWhiteSpace(str) || str == "{}") return null;
            if (DateTime.TryParse(str, out var dt)) return dt;
            return null;
        }

        private static int CompareVersions(string v1, string v2)
        {
            try
            {
                var ver1 = new Version(v1);
                var ver2 = new Version(v2);
                return ver1.CompareTo(ver2);
            }
            catch
            {
                return string.Compare(v1, v2, StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion
    }
}