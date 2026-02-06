using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using nU3.Models;
using nU3.Core.Repositories;
using nU3.Connectivity;

namespace nU3.Data.Repositories
{
    public class SQLiteProgramRepository : IProgramRepository
    {
        private readonly IDBAccessService _db;

        public SQLiteProgramRepository(IDBAccessService db)
        {
            _db = db;
        }

        public List<ProgramDto> GetAllPrograms()
        {
            var list = new List<ProgramDto>();
            string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, CLASS_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST ORDER BY PROG_NAME";
            
            using (var dt = _db.ExecuteDataTable(sql))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapRow(row));
                }
            }
            return list;
        }

        public List<ProgramDto> GetProgramsByModuleId(string moduleId)
        {
            var list = new List<ProgramDto>();
            string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, CLASS_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST WHERE MODULE_ID = @mid ORDER BY PROG_NAME";
            var parameters = new Dictionary<string, object> { { "@mid", moduleId } };

            using (var dt = _db.ExecuteDataTable(sql, parameters))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapRow(row));
                }
            }
            return list;
        }

        public ProgramDto? GetProgramByProgId(string progId)
        {
            string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, CLASS_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST WHERE PROG_ID = @pid"; // LIMIT 1 not needed for ID search usually but ok
            var parameters = new Dictionary<string, object> { { "@pid", progId } };

            using (var dt = _db.ExecuteDataTable(sql, parameters))
            {
                if (dt.Rows.Count == 0) return null;
                return MapRow(dt.Rows[0]);
            }
        }

        public void UpsertProgram(ProgramDto program)
        {
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

            var parameters = new Dictionary<string, object>
            {
                { "@pid", program.ProgId },
                { "@mid", program.ModuleId },
                { "@class", program.ClassName },
                { "@name", program.ProgName ?? (object)DBNull.Value },
                { "@auth", program.AuthLevel },
                { "@active", program.IsActive ?? "N" },
                { "@type", program.ProgType }
            };

            _db.ExecuteNonQuery(sql, parameters);
        }

        public void DeactivateMissingPrograms(string moduleId, IEnumerable<string> activeProgIds)
        {
            var activeList = activeProgIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (activeList.Count == 0)
            {
                string sql = "UPDATE SYS_PROG_MST SET IS_ACTIVE = 'N' WHERE MODULE_ID = @mid";
                _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@mid", moduleId } });
                return;
            }

            // Parameterized IN clause
            var parameters = new Dictionary<string, object> { { "@mid", moduleId } };
            var paramNames = new List<string>();
            for (int i = 0; i < activeList.Count; i++)
            {
                string pName = $"@p{i}";
                paramNames.Add(pName);
                parameters.Add(pName, activeList[i]);
            }

            string sqlUpdate = $@"
                UPDATE SYS_PROG_MST
                SET IS_ACTIVE = 'N'
                WHERE MODULE_ID = @mid AND PROG_ID NOT IN ({string.Join(", ", paramNames)})";

            _db.ExecuteNonQuery(sqlUpdate, parameters);
        }

        private ProgramDto MapRow(DataRow row)
        {
            return new ProgramDto
            {
                ProgId = row["PROG_ID"].ToString(),
                ProgName = row["PROG_NAME"] == DBNull.Value ? null : row["PROG_NAME"].ToString(),
                ModuleId = row["MODULE_ID"].ToString(),
                ClassName = row["CLASS_NAME"].ToString(),
                AuthLevel = Convert.ToInt32(row["AUTH_LEVEL"] == DBNull.Value ? 1 : row["AUTH_LEVEL"]),
                IsActive = row["IS_ACTIVE"] == DBNull.Value ? "N" : row["IS_ACTIVE"].ToString(),
                ProgType = Convert.ToInt32(row["PROG_TYPE"] == DBNull.Value ? 1 : row["PROG_TYPE"])
            };
        }
    }
}