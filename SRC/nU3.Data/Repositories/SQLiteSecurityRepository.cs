using System;
using System.Collections.Generic;
using System.Data;
using nU3.Core.Repositories;
using nU3.Connectivity;
using nU3.Core.Interfaces;

namespace nU3.Data.Repositories
{
    public class SQLiteSecurityRepository : ISecurityRepository
    {
        private readonly IDBAccessService _db;

        public SQLiteSecurityRepository(IDBAccessService db)
        {
            _db = db;
        }

        public List<SecurityUserDto> GetAllSecurityUsers()
        {
            var list = new List<SecurityUserDto>();
            string sql = @"SELECT u.USER_ID, u.USERNAME, u.EMAIL, u.ROLE_CODE, r.ROLE_NAME
                           FROM SYS_USER u
                           LEFT JOIN SYS_ROLE r ON u.ROLE_CODE = r.ROLE_CODE
                           WHERE u.IS_ACTIVE = 'Y'
                           ORDER BY u.USERNAME";

            using (var dt = _db.ExecuteDataTable(sql))
            {
                foreach (DataRow row in dt.Rows)
                {
                    var user = new SecurityUserDto
                    {
                        UserId = row["USER_ID"]?.ToString() ?? "",
                        Username = row["USERNAME"]?.ToString() ?? "",
                        Email = row["EMAIL"]?.ToString() ?? "",
                        RoleCode = row["ROLE_CODE"]?.ToString() ?? "",
                        RoleName = row["ROLE_NAME"]?.ToString() ?? ""
                    };
                    list.Add(user);
                }
            }

            foreach (var user in list)
            {
                string sqlDepts = @"
                    SELECT d.DEPT_NAME 
                    FROM SYS_USER_DEPT ud
                    JOIN SYS_DEPT d ON ud.DEPT_CODE = d.DEPT_CODE
                    WHERE ud.USER_ID = @uid";
                
                var dtDepts = _db.ExecuteDataTable(sqlDepts, new Dictionary<string, object> { { "@uid", user.UserId } });
                var depts = new List<string>();
                foreach (DataRow row in dtDepts.Rows) depts.Add(row["DEPT_NAME"].ToString());
                user.DeptNames = string.Join(", ", depts);
            }

            return list;
        }

        public void AddSecurityUser(SecurityUserDto user, string password, List<string> deptCodes)
        {
            _db.BeginTransaction();
            try
            {
                string sql = @"
                    INSERT INTO SYS_USER (USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE)
                    VALUES (@id, @name, @pwd, @email, @role, 'Y')";
                
                _db.ExecuteNonQuery(sql, new Dictionary<string, object> {
                    { "@id", user.UserId },
                    { "@name", user.Username },
                    { "@pwd", password },
                    { "@email", user.Email },
                    { "@role", user.RoleCode }
                });

                foreach (var deptCode in deptCodes)
                {
                    string sqlDept = "INSERT INTO SYS_USER_DEPT (USER_ID, DEPT_CODE) VALUES (@uid, @dcode)";
                    _db.ExecuteNonQuery(sqlDept, new Dictionary<string, object> { { "@uid", user.UserId }, { "@dcode", deptCode } });
                }
                _db.CommitTransaction();
            }
            catch
            {
                _db.RollbackTransaction();
                throw;
            }
        }

        public void UpdateSecurityUser(SecurityUserDto user, List<string> deptCodes)
        {
            _db.BeginTransaction();
            try
            {
                string sql = @"UPDATE SYS_USER SET EMAIL = @email, ROLE_CODE = @role WHERE USER_ID = @id";
                _db.ExecuteNonQuery(sql, new Dictionary<string, object> {
                    { "@email", user.Email },
                    { "@role", user.RoleCode },
                    { "@id", user.UserId }
                });

                string sqlDel = "DELETE FROM SYS_USER_DEPT WHERE USER_ID = @uid";
                _db.ExecuteNonQuery(sqlDel, new Dictionary<string, object> { { "@uid", user.UserId } });

                foreach (var deptCode in deptCodes)
                {
                    string sqlDept = "INSERT INTO SYS_USER_DEPT (USER_ID, DEPT_CODE) VALUES (@uid, @dcode)";
                    _db.ExecuteNonQuery(sqlDept, new Dictionary<string, object> { { "@uid", user.UserId }, { "@dcode", deptCode } });
                }
                _db.CommitTransaction();
            }
            catch
            {
                _db.RollbackTransaction();
                throw;
            }
        }

        public void DeleteSecurityUser(string userId)
        {
            string sql = "UPDATE SYS_USER SET IS_ACTIVE = 'N' WHERE USER_ID = @id";
            _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@id", userId } });
        }

        public bool IsUserIdExists(string userId)
        {
            object result = _db.ExecuteScalarValue("SELECT COUNT(*) FROM SYS_USER WHERE USER_ID = @id", new Dictionary<string, object> { { "@id", userId } });
            return Convert.ToInt32(result) > 0;
        }

        public List<string> GetUserDeptCodes(string userId)
        {
            var list = new List<string>();
            string sql = "SELECT DEPT_CODE FROM SYS_USER_DEPT WHERE USER_ID = @uid";
            using (var dt = _db.ExecuteDataTable(sql, new Dictionary<string, object> { { "@uid", userId } }))
            {
                foreach (DataRow row in dt.Rows) list.Add(row["DEPT_CODE"].ToString());
            }
            return list;
        }

        public List<SecurityRoleDto> GetAllRoles()
        {
            var list = new List<SecurityRoleDto>();
            string sql = "SELECT ROLE_CODE, ROLE_NAME, DESCRIPTION FROM SYS_ROLE ORDER BY ROLE_CODE";
            using (var dt = _db.ExecuteDataTable(sql))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new SecurityRoleDto
                    {
                        RoleCode = row["ROLE_CODE"]?.ToString() ?? "",
                        RoleName = row["ROLE_NAME"]?.ToString() ?? "",
                        Description = row["DESCRIPTION"]?.ToString() ?? ""
                    });
                }
            }
            return list;
        }

        public void SyncRoles(List<SecurityRoleDto> roles)
        {
            _db.BeginTransaction();
            try
            {
                foreach (var role in roles)
                {
                    string sql = @"INSERT OR REPLACE INTO SYS_ROLE (ROLE_CODE, ROLE_NAME, DESCRIPTION) VALUES (@c, @n, @d)";
                    _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@c", role.RoleCode }, { "@n", role.RoleName }, { "@d", role.Description } });
                }
                _db.CommitTransaction();
            }
            catch
            {
                _db.RollbackTransaction();
                throw;
            }
        }

        public List<SecurityDeptDto> GetAllDepartments()
        {
            var list = new List<SecurityDeptDto>();
            string sql = "SELECT DEPT_CODE, DEPT_NAME FROM SYS_DEPT ORDER BY DEPT_NAME";
            using (var dt = _db.ExecuteDataTable(sql))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new SecurityDeptDto
                    {
                        DeptCode = row["DEPT_CODE"]?.ToString() ?? "",
                        DeptName = row["DEPT_NAME"]?.ToString() ?? ""
                    });
                }
            }
            return list;
        }

        public void SyncDepartments(List<SecurityDeptDto> depts)
        {
            _db.BeginTransaction();
            try
            {
                foreach (var dept in depts)
                {
                    string sql = @"INSERT OR REPLACE INTO SYS_DEPT (DEPT_CODE, DEPT_NAME) VALUES (@c, @n)";
                    _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@c", dept.DeptCode }, { "@n", dept.DeptName } });
                }
                _db.CommitTransaction();
            }
            catch
            {
                _db.RollbackTransaction();
                throw;
            }
        }

        public SecurityPermissionDto GetPermission(string targetType, string targetId, string progId)
        {
            return GetPermissionInternal(targetType, targetId, progId);
        }

        private SecurityPermissionDto GetPermissionInternal(string targetType, string targetId, string progId)
        {
            string sql = @"
                SELECT CAN_READ, CAN_CREATE, CAN_UPDATE, CAN_DELETE, CAN_PRINT, CAN_EXPORT, CAN_APPROVE, CAN_CANCEL
                FROM SYS_PERMISSION
                WHERE TARGET_TYPE = @type AND TARGET_ID = @id AND PROG_ID = @prog";

            using (var dt = _db.ExecuteDataTable(sql, new Dictionary<string, object> {
                { "@type", targetType },
                { "@id", targetId },
                { "@prog", progId }
            }))
            {
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    return new SecurityPermissionDto
                    {
                        TargetType = targetType,
                        TargetId = targetId,
                        ProgId = progId,
                        CanRead = Convert.ToInt32(row["CAN_READ"]) == 1,
                        CanCreate = Convert.ToInt32(row["CAN_CREATE"]) == 1,
                        CanUpdate = Convert.ToInt32(row["CAN_UPDATE"]) == 1,
                        CanDelete = Convert.ToInt32(row["CAN_DELETE"]) == 1,
                        CanPrint = Convert.ToInt32(row["CAN_PRINT"]) == 1,
                        CanExport = Convert.ToInt32(row["CAN_EXPORT"]) == 1,
                        CanApprove = Convert.ToInt32(row["CAN_APPROVE"]) == 1,
                        CanCancel = Convert.ToInt32(row["CAN_CANCEL"]) == 1
                    };
                }
            }
            return null;
        }

        public SecurityPermissionDto GetEffectivePermission(string userId, string roleCode, string progId)
        {
            // 1. Check User Permission
            var userPerm = GetPermissionInternal("USER", userId, progId);
            if (userPerm != null) return userPerm;

            // 2. Check Role Permission
            var rolePerm = GetPermissionInternal("ROLE", roleCode, progId);
            if (rolePerm != null) return rolePerm;

            // 3. Default: All True
            return new SecurityPermissionDto
            {
                TargetType = "DEFAULT",
                TargetId = "ALL",
                ProgId = progId,
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true,
                CanPrint = true,
                CanExport = true,
                CanApprove = true,
                CanCancel = true
            };
        }

        public void SavePermission(SecurityPermissionDto p)
        {
            string sql = @"
                INSERT INTO SYS_PERMISSION (TARGET_TYPE, TARGET_ID, PROG_ID, CAN_READ, CAN_CREATE, CAN_UPDATE, CAN_DELETE, CAN_PRINT, CAN_EXPORT, CAN_APPROVE, CAN_CANCEL)
                VALUES (@type, @id, @prog, @r, @c, @u, @d, @p, @e, @a, @x)
                ON CONFLICT(TARGET_TYPE, TARGET_ID, PROG_ID) DO UPDATE SET
                    CAN_READ = @r,
                    CAN_CREATE = @c,
                    CAN_UPDATE = @u,
                    CAN_DELETE = @d,
                    CAN_PRINT = @p,
                    CAN_EXPORT = @e,
                    CAN_APPROVE = @a,
                    CAN_CANCEL = @x";

            _db.ExecuteNonQuery(sql, new Dictionary<string, object> {
                { "@type", p.TargetType },
                { "@id", p.TargetId },
                { "@prog", p.ProgId },
                { "@r", p.CanRead ? 1 : 0 },
                { "@c", p.CanCreate ? 1 : 0 },
                { "@u", p.CanUpdate ? 1 : 0 },
                { "@d", p.CanDelete ? 1 : 0 },
                { "@p", p.CanPrint ? 1 : 0 },
                { "@e", p.CanExport ? 1 : 0 },
                { "@a", p.CanApprove ? 1 : 0 },
                { "@x", p.CanCancel ? 1 : 0 }
            });
        }
    }
}
