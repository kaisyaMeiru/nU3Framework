using System;
using System.Collections.Generic;
using System.Data.SQLite;
using nU3.Models;
using nU3.Core.Repositories;

namespace nU3.Data.Repositories
{
    public class SQLiteMenuRepository : IMenuRepository
    {
        private readonly LocalDatabaseManager _db;

        public SQLiteMenuRepository(LocalDatabaseManager db)
        {
            _db = db;
        }

        public List<MenuDto> GetAllMenus()
        {
            var list = new List<MenuDto>();
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT * FROM SYS_MENU ORDER BY PARENT_ID, SORT_ORD";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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
            }
            return list;
        }

        public void DeleteAllMenus()
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM SYS_MENU", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddMenu(MenuDto menu)
        {
            using (var conn = new SQLiteConnection(_db.GetConnectionString()))
            {
                conn.Open();
                string sql = @"INSERT INTO SYS_MENU (MENU_ID, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL) 
                               VALUES (@id, @pid, @name, @prog, @sort, @auth)";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", menu.MenuId);
                    cmd.Parameters.AddWithValue("@pid", (object)menu.ParentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", menu.MenuName);
                    cmd.Parameters.AddWithValue("@prog", (object)menu.ProgId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sort", menu.SortOrd);
                    cmd.Parameters.AddWithValue("@auth", menu.AuthLevel);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
