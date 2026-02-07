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
    }
}