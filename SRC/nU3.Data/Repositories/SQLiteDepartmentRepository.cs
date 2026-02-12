using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using nU3.Connectivity;
using nU3.Core.Interfaces;
using nU3.Core.Repositories;
using nU3.Models;

namespace nU3.Data.Repositories
{
    public class SQLiteDepartmentRepository : IDepartmentRepository
    {
        private readonly IDBAccessService _db;

        public SQLiteDepartmentRepository(IDBAccessService db)
        {
            _db = db;
        }

        public List<DepartmentDto> GetAllDepartments()
        {
            var list = new List<DepartmentDto>();
            string sql = "SELECT * FROM SYS_DEPT WHERE IS_ACTIVE = 'Y' ORDER BY DISPLAY_ORDER, DEPT_CODE";
            
            using (var dt = _db.ExecuteDataTable(sql))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapFromDataRow(row));
                }
            }
            
            return list;
        }

        public DepartmentDto? GetDepartmentByCode(string deptCode)
        {
            string sql = "SELECT * FROM SYS_DEPT WHERE DEPT_CODE = @code";
            var parameters = new Dictionary<string, object> { { "@code", deptCode } };
            
            using (var dt = _db.ExecuteDataTable(sql, parameters))
            {
                if (dt.Rows.Count == 0) return null;
                return MapFromDataRow(dt.Rows[0]);
            }
        }

        public async Task<List<DepartmentDto>> GetDepartmentsByUserIdAsync(string userId)
        {
            var list = new List<DepartmentDto>();
            string sql = @"
                SELECT d.* 
                FROM SYS_DEPT d
                INNER JOIN SYS_USER_DEPT ud ON d.DEPT_CODE = ud.DEPT_CODE
                WHERE ud.USER_ID = @userId AND d.IS_ACTIVE = 'Y'
                ORDER BY d.DISPLAY_ORDER, d.DEPT_CODE";
            
            var parameters = new Dictionary<string, object> { { "@userId", userId } };
            
            var dt = await _db.ExecuteDataTableAsync(sql, parameters);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapFromDataRow(row));
                }
            }
            
            return list;
        }

        public void UpsertDepartment(DepartmentDto department)
        {
            string sql = @"
                INSERT INTO SYS_DEPT (DEPT_CODE, DEPT_NAME, DEPT_NAME_ENG, DESCRIPTION, DISPLAY_ORDER, PARENT_DEPT, IS_ACTIVE, CREATED_DATE)
                VALUES (@code, @name, @nameEng, @desc, @order, @parent, @active, @created)
                ON CONFLICT(DEPT_CODE) DO UPDATE SET
                    DEPT_NAME = @name,
                    DEPT_NAME_ENG = @nameEng,
                    DESCRIPTION = @desc,
                    DISPLAY_ORDER = @order,
                    PARENT_DEPT = @parent,
                    IS_ACTIVE = @active,
                    MODIFIED_DATE = @modified";
            
            var parameters = new Dictionary<string, object>
            {
                { "@code", department.DeptCode },
                { "@name", department.DeptName },
                { "@nameEng", department.DeptNameEng ?? (object)DBNull.Value },
                { "@desc", department.Description ?? (object)DBNull.Value },
                { "@order", department.DisplayOrder },
                { "@parent", department.ParentDept ?? (object)DBNull.Value },
                { "@active", department.IsActive },
                { "@created", department.CreatedDate },
                { "@modified", DateTime.Now }
            };
            
            _db.ExecuteNonQuery(sql, parameters);
        }

        public void DeactivateDepartment(string deptCode)
        {
            string sql = "UPDATE SYS_DEPT SET IS_ACTIVE = 'N', MODIFIED_DATE = @modified WHERE DEPT_CODE = @code";
            var parameters = new Dictionary<string, object>
            {
                { "@code", deptCode },
                { "@modified", DateTime.Now }
            };
            
            _db.ExecuteNonQuery(sql, parameters);
        }

        private DepartmentDto MapFromDataRow(DataRow row)
        {
            return new DepartmentDto
            {
                DeptCode = row["DEPT_CODE"].ToString() ?? string.Empty,
                DeptName = row["DEPT_NAME"].ToString() ?? string.Empty,
                DeptNameEng = row["DEPT_NAME_ENG"] == DBNull.Value ? null : row["DEPT_NAME_ENG"].ToString(),
                Description = row["DESCRIPTION"] == DBNull.Value ? null : row["DESCRIPTION"].ToString(),
                DisplayOrder = Convert.ToInt32(row["DISPLAY_ORDER"]),
                ParentDept = row["PARENT_DEPT"] == DBNull.Value ? null : row["PARENT_DEPT"].ToString(),
                IsActive = row["IS_ACTIVE"].ToString() ?? "Y",
                CreatedDate = Convert.ToDateTime(row["CREATED_DATE"]),
                ModifiedDate = row["MODIFIED_DATE"] == DBNull.Value ? null : Convert.ToDateTime(row["MODIFIED_DATE"])
            };
        }
    }
}
