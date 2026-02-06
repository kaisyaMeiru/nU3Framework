using System;
using System.Collections.Generic;
using System.Data;
using nU3.Models;
using nU3.Core.Repositories;
using nU3.Connectivity;

namespace nU3.Data.Repositories
{
    public class SQLiteModuleRepository : IModuleRepository
    {
        private readonly IDBAccessService _db;

        public SQLiteModuleRepository(IDBAccessService db)
        {
            _db = db;
        }

        public List<ModuleMstDto> GetAllModules()
        {
            var list = new List<ModuleMstDto>();
            string sql = "SELECT * FROM SYS_MODULE_MST ORDER BY REG_DATE DESC";
            
            using (var dt = _db.ExecuteDataTable(sql))
            {
                foreach (DataRow row in dt.Rows)
                {
                                            list.Add(new ModuleMstDto
                                        {
                                            ModuleId = row["MODULE_ID"].ToString(),
                                            Category = row["CATEGORY"] == DBNull.Value ? null : row["CATEGORY"].ToString(),
                                            SubSystem = row["SUBSYSTEM"] == DBNull.Value ? null : row["SUBSYSTEM"].ToString(),
                                            ModuleName = row["MODULE_NAME"].ToString(),
                                            FileName = row["FILE_NAME"].ToString(),
                                            RegDate = ParseDate(row["REG_DATE"]) ?? DateTime.Now
                                        });
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
                                string sql = @"
                                    INSERT INTO SYS_MODULE_MST (MODULE_ID, CATEGORY, SUBSYSTEM, MODULE_NAME, FILE_NAME)
                                    VALUES (@id, @cat, @sub, @name, @file)
                                    ON CONFLICT(MODULE_ID) DO UPDATE SET
                                        CATEGORY = @cat,
                                        SUBSYSTEM = @sub,
                                        MODULE_NAME = @name,
                                        FILE_NAME = @file";
                    
                                var parameters = new Dictionary<string, object>
                                {
                                    { "@id", module.ModuleId },
                                    { "@cat", (object)module.Category ?? DBNull.Value },
                                    { "@sub", (object)module.SubSystem ?? DBNull.Value },
                                    { "@name", module.ModuleName },
                                    { "@file", module.FileName }
                                };
                                _db.ExecuteNonQuery(sql, parameters);
                            }
                    
                            public void DeleteModule(string moduleId)
                            {
                                _db.BeginTransaction();
                                try
                                {
                                    _db.ExecuteNonQuery("DELETE FROM SYS_PROG_MST WHERE MODULE_ID = @id", new Dictionary<string, object> { { "@id", moduleId } });
                                    _db.ExecuteNonQuery("DELETE FROM SYS_MODULE_VER WHERE MODULE_ID = @id", new Dictionary<string, object> { { "@id", moduleId } });
                                    _db.ExecuteNonQuery("DELETE FROM SYS_MODULE_MST WHERE MODULE_ID = @id", new Dictionary<string, object> { { "@id", moduleId } });
                                    _db.CommitTransaction();
                                }
                                catch
                                {
                                    _db.RollbackTransaction();
                                    throw;
                                }
                            }
                    
                            public List<ModuleVerDto> GetActiveVersions()
                            {
                                var list = new List<ModuleVerDto>();
                                string sql = @"
                                    SELECT v.*, m.CATEGORY 
                                    FROM SYS_MODULE_VER v
                                    JOIN SYS_MODULE_MST m ON v.MODULE_ID = m.MODULE_ID
                                    WHERE v.DEL_DATE IS NULL";
                    
                                using (var dt = _db.ExecuteDataTable(sql))
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        list.Add(new ModuleVerDto
                                        {
                                            ModuleId = row["MODULE_ID"].ToString(),
                                            Version = row["VERSION"].ToString(),
                                            FileHash = row["FILE_HASH"].ToString(),
                                            FileSize = Convert.ToInt64(row["FILE_SIZE"]),
                                            StoragePath = row["STORAGE_PATH"].ToString(),
                                            DeployDesc = row["DEPLOY_DESC"] == DBNull.Value ? "" : row["DEPLOY_DESC"].ToString(),
                                            DelDate = ParseDate(row["DEL_DATE"]),
                                            Category = row["CATEGORY"] == DBNull.Value ? "ETC" : row["CATEGORY"].ToString()
                                        });
                                    }
                                }
                                return list;
                            }
                    
                            public void DeactivateOldVersions(string moduleId)
                            {
                                string sql = "UPDATE SYS_MODULE_VER SET DEL_DATE=CURRENT_TIMESTAMP WHERE MODULE_ID=@id AND DEL_DATE IS NULL";
                                _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@id", moduleId } });
                            }
                    
                            public void AddVersion(ModuleVerDto version)
                            {
                                string sql = @"
                                    INSERT INTO SYS_MODULE_VER (MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEL_DATE)
                                    VALUES (@id, @ver, @hash, @size, @path, @desc, NULL)
                                    ON CONFLICT(MODULE_ID, VERSION) DO UPDATE SET
                                        FILE_HASH = @hash,
                                        FILE_SIZE = @size,
                                        STORAGE_PATH = @path,
                                        DEPLOY_DESC = @desc,
                                        DEL_DATE = NULL";
                    
                                var parameters = new Dictionary<string, object>
                                {
                                    { "@id", version.ModuleId },
                                    { "@ver", version.Version },
                                    { "@hash", version.FileHash },
                                    { "@size", version.FileSize },
                                    { "@path", version.StoragePath },
                                    { "@desc", (object)version.DeployDesc ?? DBNull.Value }
                                };
                                _db.ExecuteNonQuery(sql, parameters);
                            }
                    
                            private static DateTime? ParseDate(object value)
                            {
                                if (value == null || value == DBNull.Value) return null;
                                string str = value.ToString();
                                if (string.IsNullOrWhiteSpace(str) || str == "{}") return null;
                                if (DateTime.TryParse(str, out var dt)) return dt;
                                return null;
                            }
                        }
                    }