using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using nU3.Models;
using nU3.Core.Repositories;

namespace nU3.Data.Repositories
{
    public class SQLiteProgramRepository : IProgramRepository
    {
        private readonly LocalDatabaseManager _db;

        public SQLiteProgramRepository(LocalDatabaseManager db)
        {
            _db = db;
        }

        public List<ProgramDto> GetAllPrograms()
        {
            var list = new List<ProgramDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, CLASS_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST ORDER BY PROG_NAME";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProgramDto
                        {
                            ProgId = reader["PROG_ID"].ToString(),
                            ProgName = reader["PROG_NAME"]?.ToString(),
                            ModuleId = reader["MODULE_ID"].ToString(),
                            ClassName = reader["CLASS_NAME"].ToString(),
                            AuthLevel = Convert.ToInt32(reader["AUTH_LEVEL"] == DBNull.Value ? 1 : reader["AUTH_LEVEL"]),
                            IsActive = reader["IS_ACTIVE"] == DBNull.Value ? "N" : reader["IS_ACTIVE"].ToString(),
                            ProgType = Convert.ToInt32(reader["PROG_TYPE"] == DBNull.Value ? 1 : reader["PROG_TYPE"])
                        });
                    }
                }
            }
            return list;
        }

        public List<ProgramDto> GetProgramsByModuleId(string moduleId)
        {
            var list = new List<ProgramDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, CLASS_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST WHERE MODULE_ID = @mid ORDER BY PROG_NAME";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mid", moduleId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ProgramDto
                            {
                                ProgId = reader["PROG_ID"].ToString(),
                                ProgName = reader["PROG_NAME"]?.ToString(),
                                ModuleId = reader["MODULE_ID"].ToString(),
                                ClassName = reader["CLASS_NAME"].ToString(),
                                AuthLevel = Convert.ToInt32(reader["AUTH_LEVEL"] == DBNull.Value ? 1 : reader["AUTH_LEVEL"]),
                                IsActive = reader["IS_ACTIVE"] == DBNull.Value ? "N" : reader["IS_ACTIVE"].ToString(),
                                ProgType = Convert.ToInt32(reader["PROG_TYPE"] == DBNull.Value ? 1 : reader["PROG_TYPE"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public ProgramDto? GetProgramByProgId(string progId)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, CLASS_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST WHERE PROG_ID = @pid LIMIT 1";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", progId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        return new ProgramDto
                        {
                            ProgId = reader["PROG_ID"].ToString(),
                            ProgName = reader["PROG_NAME"]?.ToString(),
                            ModuleId = reader["MODULE_ID"].ToString(),
                            ClassName = reader["CLASS_NAME"].ToString(),
                            AuthLevel = Convert.ToInt32(reader["AUTH_LEVEL"] == DBNull.Value ? 1 : reader["AUTH_LEVEL"]),
                            IsActive = reader["IS_ACTIVE"] == DBNull.Value ? "N" : reader["IS_ACTIVE"].ToString(),
                            ProgType = Convert.ToInt32(reader["PROG_TYPE"] == DBNull.Value ? 1 : reader["PROG_TYPE"])
                        };
                    }
                }
            }
        }

        public void UpsertProgram(ProgramDto program)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO SYS_PROG_MST (PROG_ID, MODULE_ID, CLASS_NAME, PROG_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE)
                    VALUES (@pid, @mid, @class, @name, @auth, @active, @type)
                    ON CONFLICT(PROG_ID) DO UPDATE SET
                        MODULE_ID = @mid,
                        CLASS_NAME = @class,
                        PROG_NAME = @name,
                        AUTH_LEVEL = @auth,
                        IS_ACTIVE = @active,
                        PROG_TYPE = @type";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", program.ProgId);
                    cmd.Parameters.AddWithValue("@mid", program.ModuleId);
                    cmd.Parameters.AddWithValue("@class", program.ClassName);
                    cmd.Parameters.AddWithValue("@name", (object)program.ProgName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@auth", program.AuthLevel);
                    cmd.Parameters.AddWithValue("@active", (object)program.IsActive ?? "N");
                    cmd.Parameters.AddWithValue("@type", program.ProgType);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeactivateMissingPrograms(string moduleId, IEnumerable<string> activeProgIds)
        {
            var activeList = activeProgIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();

                if (activeList.Count == 0)
                {
                    using var cmd = new SQLiteCommand(
                        "UPDATE SYS_PROG_MST SET IS_ACTIVE = 'N' WHERE MODULE_ID = @mid", conn);
                    cmd.Parameters.AddWithValue("@mid", moduleId);
                    cmd.ExecuteNonQuery();
                    return;
                }

                var parameters = activeList.Select((_, i) => $"@p{i}").ToList();
                var sql = $@"
                    UPDATE SYS_PROG_MST
                    SET IS_ACTIVE = 'N'
                    WHERE MODULE_ID = @mid AND PROG_ID NOT IN ({string.Join(", ", parameters)})";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mid", moduleId);
                    for (var i = 0; i < parameters.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(parameters[i], activeList[i]);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
