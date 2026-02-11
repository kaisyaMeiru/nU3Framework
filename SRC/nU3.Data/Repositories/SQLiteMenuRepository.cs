using System;
using System.Collections.Generic;
using System.Data;
using nU3.Models;
using nU3.Core.Repositories;
using nU3.Connectivity;

namespace nU3.Data.Repositories
{
    public class SQLiteMenuRepository : IMenuRepository
    {
        private readonly IDBAccessService _db;

        public SQLiteMenuRepository(IDBAccessService db)
        {
            _db = db;
        }

        public List<MenuDto> GetAllMenus()
        {
            var list = new List<MenuDto>();
            string sql = "SELECT * FROM SYS_MENU ORDER BY PARENT_ID, SORT_ORD";
            using (var dt = _db.ExecuteDataTable(sql))
            {
                foreach (DataRow reader in dt.Rows)
                {
                    list.Add(new MenuDto
                    {
                        MenuId = reader["MENU_ID"].ToString(),
                        ParentId = reader["PARENT_ID"] == DBNull.Value ? null : reader["PARENT_ID"].ToString(),
                        MenuName = reader["MENU_NAME"].ToString(),
                        ProgId = reader["PROG_ID"] == DBNull.Value ? null : reader["PROG_ID"].ToString(),
                        SortOrd = Convert.ToInt32(reader["SORT_ORD"]),
                        AuthLevel = Convert.ToInt32(reader["AUTH_LEVEL"] == DBNull.Value ? 1 : reader["AUTH_LEVEL"])
                    });
                }
            }
            return list;
        }

        public void DeleteAllMenus()
        {
            _db.ExecuteNonQuery("DELETE FROM SYS_MENU");
        }

        public void AddMenu(MenuDto menu)
        {
            string sql = @"INSERT INTO SYS_MENU (MENU_ID, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL) 
                           VALUES (@id, @pid, @name, @prog, @sort, @auth)";
            
            var parameters = new Dictionary<string, object>
            {
                { "@id", menu.MenuId },
                { "@pid", string.IsNullOrEmpty(menu.ParentId) ? (object)DBNull.Value : menu.ParentId },
                { "@name", menu.MenuName },
                { "@prog", string.IsNullOrEmpty(menu.ProgId) ? (object)DBNull.Value : menu.ProgId },
                { "@sort", menu.SortOrd },
                { "@auth", menu.AuthLevel }
            };
            _db.ExecuteNonQuery(sql, parameters);
        }

        public List<MenuDto> GetMenusByDeptCode(string deptCode)
        {
            var list = new List<MenuDto>();
            string sql = "SELECT * FROM SYS_DEPT_MENU WHERE DEPT_CODE = @deptCode ORDER BY PARENT_ID, SORT_ORD";
            var parameters = new Dictionary<string, object> { { "@deptCode", deptCode } };
            
            using (var dt = _db.ExecuteDataTable(sql, parameters))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapMenuFromDataRow(row));
                }
            }
            
            return list;
        }

        public List<MenuDto> GetMenusByUserAndDept(string userId, string deptCode)
        {
            var list = new List<MenuDto>();
            string sql = "SELECT * FROM SYS_USER_MENU WHERE USER_ID = @userId AND DEPT_CODE = @deptCode ORDER BY PARENT_ID, SORT_ORD";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId },
                { "@deptCode", deptCode }
            };
            
            using (var dt = _db.ExecuteDataTable(sql, parameters))
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(MapMenuFromDataRow(row));
                }
            }
            
            return list;
        }

        public void DeleteMenusByDeptCode(string deptCode)
        {
            string sql = "DELETE FROM SYS_DEPT_MENU WHERE DEPT_CODE = @deptCode";
            var parameters = new Dictionary<string, object> { { "@deptCode", deptCode } };
            _db.ExecuteNonQuery(sql, parameters);
        }

        public void DeleteMenusByUserAndDept(string userId, string deptCode)
        {
            string sql = "DELETE FROM SYS_USER_MENU WHERE USER_ID = @userId AND DEPT_CODE = @deptCode";
            var parameters = new Dictionary<string, object>
            {
                { "@userId", userId },
                { "@deptCode", deptCode }
            };
            _db.ExecuteNonQuery(sql, parameters);
        }

        public void AddMenuForDept(string deptCode, MenuDto menu)
        {
            string sql = @"INSERT INTO SYS_DEPT_MENU (MENU_ID, DEPT_CODE, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL) 
                           VALUES (@id, @deptCode, @pid, @name, @prog, @sort, @auth)";
            
            var parameters = new Dictionary<string, object>
            {
                { "@id", menu.MenuId },
                { "@deptCode", deptCode },
                { "@pid", string.IsNullOrEmpty(menu.ParentId) ? (object)DBNull.Value : menu.ParentId },
                { "@name", menu.MenuName },
                { "@prog", string.IsNullOrEmpty(menu.ProgId) ? (object)DBNull.Value : menu.ProgId },
                { "@sort", menu.SortOrd },
                { "@auth", menu.AuthLevel }
            };
            _db.ExecuteNonQuery(sql, parameters);
        }

        public void AddMenuForUser(string userId, string deptCode, MenuDto menu)
        {
            string sql = @"INSERT INTO SYS_USER_MENU (MENU_ID, USER_ID, DEPT_CODE, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL) 
                           VALUES (@id, @userId, @deptCode, @pid, @name, @prog, @sort, @auth)";
            
            var parameters = new Dictionary<string, object>
            {
                { "@id", menu.MenuId },
                { "@userId", userId },
                { "@deptCode", deptCode },
                { "@pid", string.IsNullOrEmpty(menu.ParentId) ? (object)DBNull.Value : menu.ParentId },
                { "@name", menu.MenuName },
                { "@prog", string.IsNullOrEmpty(menu.ProgId) ? (object)DBNull.Value : menu.ProgId },
                { "@sort", menu.SortOrd },
                { "@auth", menu.AuthLevel }
            };
            _db.ExecuteNonQuery(sql, parameters);
        }

        private MenuDto MapMenuFromDataRow(DataRow row)
        {
            return new MenuDto
            {
                MenuId = row["MENU_ID"].ToString(),
                ParentId = row["PARENT_ID"] == DBNull.Value ? null : row["PARENT_ID"].ToString(),
                MenuName = row["MENU_NAME"].ToString(),
                ProgId = row["PROG_ID"] == DBNull.Value ? null : row["PROG_ID"].ToString(),
                SortOrd = Convert.ToInt32(row["SORT_ORD"]),
                AuthLevel = Convert.ToInt32(row["AUTH_LEVEL"] == DBNull.Value ? 1 : row["AUTH_LEVEL"])
            };
        }
    }
}