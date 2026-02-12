using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using nU3.Connectivity;
using nU3.Core.Repositories;
using nU3.Models;

namespace nU3.Data.Repositories
{
    /// <summary>
    /// SQLite 기반의 사용자 정보 저장소 구현체입니다.
    /// IDBAccessService를 통해 서버 API와 통신합니다.
    /// </summary>
    public class SQLiteUserRepository : IUserRepository
    {
        private readonly IDBAccessService _db;

        public SQLiteUserRepository(IDBAccessService db)
        {
            _db = db;
        }

        public async Task<List<UserInfoDto>> GetAllUsersAsync()
        {
            string sql = "SELECT USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE FROM SYS_USER WHERE IS_ACTIVE = 'Y' ORDER BY USERNAME";
            var dt = await _db.ExecuteDataTableAsync(sql);
            var users = new List<UserInfoDto>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    users.Add(new UserInfoDto
                    {
                        UserId = row["USER_ID"].ToString() ?? string.Empty,
                        UserName = row["USERNAME"].ToString() ?? string.Empty,
                        Password = row["PASSWORD"].ToString() ?? string.Empty,
                        Email = row["EMAIL"].ToString() ?? string.Empty,
                        IsActive = (row["IS_ACTIVE"].ToString() ?? "N") == "Y",
                        RoleCode = row["ROLE_CODE"].ToString() ?? "",
                        Remarks = row["ROLE_CODE"].ToString() ?? "1"  // 기본값: 사용자(Tech)
                    });
                }
            }

            return users;
        }

        public async Task<UserInfoDto?> GetUserByIdAsync(string userId)
        {
            string sql = "SELECT USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE FROM SYS_USER WHERE USER_ID = @id AND IS_ACTIVE = 'Y'";
            var parameters = new Dictionary<string, object> { { "@id", userId } };

            var dt = await _db.ExecuteDataTableAsync(sql, parameters);
            if (dt == null || dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new UserInfoDto
            {
                UserId = row["USER_ID"].ToString() ?? string.Empty,
                UserName = row["USERNAME"].ToString() ?? string.Empty,
                Password = row["PASSWORD"].ToString() ?? string.Empty,
                Email = row["EMAIL"].ToString() ?? string.Empty,
                IsActive = (row["IS_ACTIVE"].ToString() ?? "N") == "Y",
                RoleCode = row["ROLE_CODE"].ToString() ?? "",
                Remarks = row["ROLE_CODE"].ToString() ?? "1"  // 기본값: 사용자(Tech)
            };
        }

        public UserInfoDto? GetUserById(string userId)
        {
            string sql = "SELECT USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE FROM SYS_USER WHERE USER_ID = @id AND IS_ACTIVE = 'Y'";
            var parameters = new Dictionary<string, object> { { "@id", userId } };

            var dt = _db.ExecuteDataTable(sql, parameters);
            if (dt == null || dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new UserInfoDto
            {
                UserId = row["USER_ID"].ToString() ?? string.Empty,
                UserName = row["USERNAME"].ToString() ?? string.Empty,
                Password = row["PASSWORD"].ToString() ?? string.Empty,
                Email = row["EMAIL"].ToString() ?? string.Empty,
                IsActive = (row["IS_ACTIVE"].ToString() ?? "N") == "Y",
                RoleCode = row["ROLE_CODE"].ToString() ?? "",
                Remarks = row["ROLE_CODE"].ToString() ?? "1"
            };
        }

        public async Task<List<string>> GetUserDepartmentCodesAsync(string userId)
        {
            string sql = "SELECT DEPT_CODE FROM SYS_USER_DEPT WHERE USER_ID = @id";
            var parameters = new Dictionary<string, object> { { "@id", userId } };

            var dt = await _db.ExecuteDataTableAsync(sql, parameters);
            var list = new List<string>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(row["DEPT_CODE"].ToString() ?? string.Empty);
                }
            }

            return list;
        }

        public async Task SaveUserAsync(UserInfoDto user)
        {
            string sql = @"
                INSERT INTO SYS_USER (USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE)
                VALUES (@id, @name, @pwd, @email, @role, @active)
                ON CONFLICT(USER_ID) DO UPDATE SET
                    USERNAME = @name,
                    PASSWORD = @pwd,
                    EMAIL = @email,
                    ROLE_CODE = @role,
                    IS_ACTIVE = @active";

            int roleCode = int.TryParse(user.Remarks, out var code) ? code : 1;  // 기본값: Tech(1)

            var parameters = new Dictionary<string, object>
            {
                { "@id", user.UserId },
                { "@name", user.UserName },
                { "@pwd", user.Password },
                { "@email", user.Email },
                { "@role", roleCode },
                { "@active", user.IsActive ? "Y" : "N" }
            };

            await _db.ExecuteNonQueryAsync(sql, parameters);
        }

        public async Task UpdateUserDepartmentsAsync(string userId, List<string> deptCodes)
        {
            // 트랜잭션 처리가 필요할 수 있으나 IDBAccessService의 트랜잭션 기능을 활용
            _db.BeginTransaction();
            try
            {
                // 기존 맵핑 삭제
                await _db.ExecuteNonQueryAsync("DELETE FROM SYS_USER_DEPT WHERE USER_ID = @id", new Dictionary<string, object> { { "@id", userId } });

                // 신규 맵핑 추가
                foreach (var code in deptCodes)
                {
                    await _db.ExecuteNonQueryAsync("INSERT INTO SYS_USER_DEPT (USER_ID, DEPT_CODE) VALUES (@id, @dept)", 
                        new Dictionary<string, object> { { "@id", userId }, { "@dept", code } });
                }

                _db.CommitTransaction();
            }
            catch
            {
                _db.RollbackTransaction();
                throw;
            }
        }
    }
}
