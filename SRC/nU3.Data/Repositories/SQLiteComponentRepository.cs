using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using nU3.Models;
using nU3.Core.Repositories;

namespace nU3.Data.Repositories
{
    /// <summary>
    /// SQLite implementation of IComponentRepository
    /// </summary>
    public class SQLiteComponentRepository : IComponentRepository
    {
        private readonly LocalDatabaseManager _db;

        public SQLiteComponentRepository(LocalDatabaseManager db)
        {
            _db = db;
        }

        #region Component Master

        public List<ComponentMstDto> GetAllComponents()
        {
            var list = new List<ComponentMstDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT * FROM SYS_COMPONENT_MST 
                    WHERE IS_ACTIVE = 'Y'
                    ORDER BY PRIORITY, GROUP_NAME, COMPONENT_NAME";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapToComponentMst(reader));
                    }
                }
            }
            return list;
        }

        public List<ComponentMstDto> GetComponentsByType(ComponentType type)
        {
            var list = new List<ComponentMstDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT * FROM SYS_COMPONENT_MST 
                    WHERE COMPONENT_TYPE = @type AND IS_ACTIVE = 'Y'
                    ORDER BY PRIORITY, COMPONENT_NAME";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@type", (int)type);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapToComponentMst(reader));
                        }
                    }
                }
            }
            return list;
        }

        public List<ComponentMstDto> GetComponentsByGroup(string groupName)
        {
            var list = new List<ComponentMstDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT * FROM SYS_COMPONENT_MST 
                    WHERE GROUP_NAME = @group AND IS_ACTIVE = 'Y'
                    ORDER BY PRIORITY, COMPONENT_NAME";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@group", groupName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapToComponentMst(reader));
                        }
                    }
                }
            }
            return list;
        }

        public List<ComponentMstDto> GetRequiredComponents()
        {
            var list = new List<ComponentMstDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT * FROM SYS_COMPONENT_MST 
                    WHERE IS_REQUIRED = 1 AND IS_ACTIVE = 'Y'
                    ORDER BY PRIORITY, COMPONENT_NAME";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapToComponentMst(reader));
                    }
                }
            }
            return list;
        }

        public ComponentMstDto GetComponent(string componentId)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT * FROM SYS_COMPONENT_MST WHERE COMPONENT_ID = @id";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapToComponentMst(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void SaveComponent(ComponentMstDto component)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
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

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", component.ComponentId);
                    cmd.Parameters.AddWithValue("@type", (int)component.ComponentType);
                    cmd.Parameters.AddWithValue("@name", component.ComponentName);
                    cmd.Parameters.AddWithValue("@file", component.FileName);
                    cmd.Parameters.AddWithValue("@path", (object)component.InstallPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@group", (object)component.GroupName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@required", component.IsRequired ? 1 : 0);
                    cmd.Parameters.AddWithValue("@auto", component.AutoUpdate ? 1 : 0);
                    cmd.Parameters.AddWithValue("@desc", (object)component.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@priority", component.Priority);
                    cmd.Parameters.AddWithValue("@deps", (object)component.Dependencies ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@active", component.IsActive ?? "Y");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteComponent(string componentId)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                // Soft delete
                using (var cmd = new SQLiteCommand(
                    "UPDATE SYS_COMPONENT_MST SET IS_ACTIVE = 'N', MOD_DATE = CURRENT_TIMESTAMP WHERE COMPONENT_ID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Component Version

        public List<ComponentVerDto> GetActiveVersions()
        {
            var list = new List<ComponentVerDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                    FROM SYS_COMPONENT_VER v
                    JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                    WHERE v.IS_ACTIVE = 'Y' AND v.DEL_DATE IS NULL AND m.IS_ACTIVE = 'Y'
                    ORDER BY m.PRIORITY, m.COMPONENT_NAME";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapToComponentVer(reader));
                    }
                }
            }
            return list;
        }

        public List<ComponentVerDto> GetActiveVersionsByType(ComponentType type)
        {
            var list = new List<ComponentVerDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                    FROM SYS_COMPONENT_VER v
                    JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                    WHERE v.IS_ACTIVE = 'Y' AND v.DEL_DATE IS NULL 
                      AND m.IS_ACTIVE = 'Y' AND m.COMPONENT_TYPE = @type
                    ORDER BY m.PRIORITY, m.COMPONENT_NAME";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@type", (int)type);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapToComponentVer(reader));
                        }
                    }
                }
            }
            return list;
        }

        public ComponentVerDto GetActiveVersion(string componentId)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                    FROM SYS_COMPONENT_VER v
                    JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                    WHERE v.COMPONENT_ID = @id AND v.IS_ACTIVE = 'Y' AND v.DEL_DATE IS NULL";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapToComponentVer(reader);
                        }
                    }
                }
            }
            return null;
        }

        public List<ComponentVerDto> GetVersionHistory(string componentId)
        {
            var list = new List<ComponentVerDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT v.*, m.COMPONENT_TYPE, m.COMPONENT_NAME, m.INSTALL_PATH, m.GROUP_NAME
                    FROM SYS_COMPONENT_VER v
                    JOIN SYS_COMPONENT_MST m ON v.COMPONENT_ID = m.COMPONENT_ID
                    WHERE v.COMPONENT_ID = @id
                    ORDER BY v.REG_DATE DESC";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapToComponentVer(reader));
                        }
                    }
                }
            }
            return list;
        }

        public void AddVersion(ComponentVerDto version)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
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

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", version.ComponentId);
                    cmd.Parameters.AddWithValue("@ver", version.Version);
                    cmd.Parameters.AddWithValue("@hash", version.FileHash);
                    cmd.Parameters.AddWithValue("@size", version.FileSize);
                    cmd.Parameters.AddWithValue("@path", version.StoragePath);
                    cmd.Parameters.AddWithValue("@minVer", (object)version.MinFrameworkVersion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@maxVer", (object)version.MaxFrameworkVersion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@desc", (object)version.DeployDesc ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@note", (object)version.ReleaseNoteUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@active", version.IsActive ?? "Y");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeactivateOldVersions(string componentId)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"UPDATE SYS_COMPONENT_VER 
                      SET IS_ACTIVE = 'N', DEL_DATE = CURRENT_TIMESTAMP 
                      WHERE COMPONENT_ID = @id AND IS_ACTIVE = 'Y'", conn))
                {
                    cmd.Parameters.AddWithValue("@id", componentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SetActiveVersion(string componentId, string version)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Deactivate all versions
                        using (var cmd = new SQLiteCommand(
                            "UPDATE SYS_COMPONENT_VER SET IS_ACTIVE = 'N' WHERE COMPONENT_ID = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", componentId);
                            cmd.ExecuteNonQuery();
                        }

                        // Activate specific version
                        using (var cmd = new SQLiteCommand(
                            @"UPDATE SYS_COMPONENT_VER 
                              SET IS_ACTIVE = 'Y', DEL_DATE = NULL 
                              WHERE COMPONENT_ID = @id AND VERSION = @ver", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", componentId);
                            cmd.Parameters.AddWithValue("@ver", version);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
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
                    // Component not installed - needs install
                    updates.Add(serverVer);
                }
                else if (CompareVersions(serverVer.Version, clientVer.InstalledVersion) > 0)
                {
                    // Server version is newer
                    updates.Add(serverVer);
                }
                else if (serverVer.FileHash != clientVer.FileHash)
                {
                    // Hash mismatch - file may be corrupted
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

        private static ComponentMstDto MapToComponentMst(SQLiteDataReader reader)
        {
            return new ComponentMstDto
            {
                ComponentId = reader["COMPONENT_ID"].ToString(),
                ComponentType = (ComponentType)Convert.ToInt32(reader["COMPONENT_TYPE"]),
                ComponentName = reader["COMPONENT_NAME"].ToString(),
                FileName = reader["FILE_NAME"].ToString(),
                InstallPath = reader["INSTALL_PATH"]?.ToString(),
                GroupName = reader["GROUP_NAME"]?.ToString(),
                IsRequired = Convert.ToInt32(reader["IS_REQUIRED"]) == 1,
                AutoUpdate = Convert.ToInt32(reader["AUTO_UPDATE"]) == 1,
                Description = reader["DESCRIPTION"]?.ToString(),
                Priority = Convert.ToInt32(reader["PRIORITY"]),
                Dependencies = reader["DEPENDENCIES"]?.ToString(),
                RegDate = Convert.ToDateTime(reader["REG_DATE"]),
                ModDate = reader["MOD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["MOD_DATE"]),
                IsActive = reader["IS_ACTIVE"]?.ToString() ?? "Y"
            };
        }

        private static ComponentVerDto MapToComponentVer(SQLiteDataReader reader)
        {
            return new ComponentVerDto
            {
                ComponentId = reader["COMPONENT_ID"].ToString(),
                Version = reader["VERSION"].ToString(),
                FileHash = reader["FILE_HASH"].ToString(),
                FileSize = Convert.ToInt64(reader["FILE_SIZE"]),
                StoragePath = reader["STORAGE_PATH"].ToString(),
                MinFrameworkVersion = reader["MIN_FRAMEWORK_VER"]?.ToString(),
                MaxFrameworkVersion = reader["MAX_FRAMEWORK_VER"]?.ToString(),
                DeployDesc = reader["DEPLOY_DESC"]?.ToString(),
                ReleaseNoteUrl = reader["RELEASE_NOTE_URL"]?.ToString(),
                RegDate = Convert.ToDateTime(reader["REG_DATE"]),
                DelDate = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]),
                IsActive = reader["IS_ACTIVE"]?.ToString() ?? "Y",
                // Joined fields
                ComponentType = (ComponentType)Convert.ToInt32(reader["COMPONENT_TYPE"]),
                ComponentName = reader["COMPONENT_NAME"]?.ToString(),
                InstallPath = reader["INSTALL_PATH"]?.ToString(),
                GroupName = reader["GROUP_NAME"]?.ToString()
            };
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
