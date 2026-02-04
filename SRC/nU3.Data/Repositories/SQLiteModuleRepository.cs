using System;
using System.Collections.Generic;
using System.Data.SQLite;
using nU3.Models;
using nU3.Core.Repositories;

namespace nU3.Data.Repositories
{
    public class SQLiteModuleRepository : IModuleRepository
    {
        private readonly LocalDatabaseManager _db;

        public SQLiteModuleRepository(LocalDatabaseManager db)
        {
            _db = db;
        }

        public List<ModuleMstDto> GetAllModules()
        {
            var list = new List<ModuleMstDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT * FROM SYS_MODULE_MST ORDER BY REG_DATE DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ModuleMstDto
                        {
                            ModuleId = reader["MODULE_ID"].ToString(),
                            Category = reader["CATEGORY"]?.ToString(),
                            SubSystem = reader["SUBSYSTEM"]?.ToString(),
                            ModuleName = reader["MODULE_NAME"].ToString(),
                            FileName = reader["FILE_NAME"].ToString(),
                            RegDate = Convert.ToDateTime(reader["REG_DATE"])
                        });
                    }
                }
            }
            return list;
        }

        public ModuleMstDto GetModule(string moduleId)
        {
            // Simplified for now
            return null;
        }

        public void SaveModule(ModuleMstDto module)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO SYS_MODULE_MST (MODULE_ID, CATEGORY, SUBSYSTEM, MODULE_NAME, FILE_NAME)
                    VALUES (@id, @cat, @sub, @name, @file)
                    ON CONFLICT(MODULE_ID) DO UPDATE SET
                        CATEGORY = @cat,
                        SUBSYSTEM = @sub,
                        MODULE_NAME = @name,
                        FILE_NAME = @file";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", module.ModuleId);
                    cmd.Parameters.AddWithValue("@cat", module.Category);
                    cmd.Parameters.AddWithValue("@sub", (object)module.SubSystem ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", module.ModuleName);
                    cmd.Parameters.AddWithValue("@file", module.FileName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteModule(string moduleId)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmdPrograms = new SQLiteCommand("DELETE FROM SYS_PROG_MST WHERE MODULE_ID = @id", conn, tx))
                    {
                        cmdPrograms.Parameters.AddWithValue("@id", moduleId);
                        cmdPrograms.ExecuteNonQuery();
                    }

                    using (var cmdVersions = new SQLiteCommand("DELETE FROM SYS_MODULE_VER WHERE MODULE_ID = @id", conn, tx))
                    {
                        cmdVersions.Parameters.AddWithValue("@id", moduleId);
                        cmdVersions.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand("DELETE FROM SYS_MODULE_MST WHERE MODULE_ID = @id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", moduleId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }
        }

        public List<ModuleVerDto> GetActiveVersions()
        {
            var list = new List<ModuleVerDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT v.*, m.CATEGORY 
                    FROM SYS_MODULE_VER v
                    JOIN SYS_MODULE_MST m ON v.MODULE_ID = m.MODULE_ID
                    WHERE v.DEL_DATE IS NULL";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ModuleVerDto
                        {
                            ModuleId = reader["MODULE_ID"].ToString(),
                            Version = reader["VERSION"].ToString(),
                            FileHash = reader["FILE_HASH"].ToString(),
                            FileSize = Convert.ToInt64(reader["FILE_SIZE"]),
                            StoragePath = reader["STORAGE_PATH"].ToString(),
                            DeployDesc = reader["DEPLOY_DESC"].ToString(),
                            DelDate = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]),
                            Category = reader["CATEGORY"]?.ToString() ?? "ETC"
                        });
                    }
                }
            }
            return list;
        }

        public void DeactivateOldVersions(string moduleId)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE SYS_MODULE_VER SET DEL_DATE=CURRENT_TIMESTAMP WHERE MODULE_ID=@id AND DEL_DATE IS NULL", conn))
                {
                    cmd.Parameters.AddWithValue("@id", moduleId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddVersion(ModuleVerDto version)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO SYS_MODULE_VER (MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEL_DATE)
                    VALUES (@id, @ver, @hash, @size, @path, @desc, NULL)
                    ON CONFLICT(MODULE_ID, VERSION) DO UPDATE SET
                        FILE_HASH = @hash,
                        FILE_SIZE = @size,
                        STORAGE_PATH = @path,
                        DEPLOY_DESC = @desc,
                        DEL_DATE = NULL";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", version.ModuleId);
                    cmd.Parameters.AddWithValue("@ver", version.Version);
                    cmd.Parameters.AddWithValue("@hash", version.FileHash);
                    cmd.Parameters.AddWithValue("@size", version.FileSize);
                    cmd.Parameters.AddWithValue("@path", version.StoragePath);
                    cmd.Parameters.AddWithValue("@desc", version.DeployDesc);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
