using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using nU3.Connectivity;
using nU3.Core.Interfaces;

namespace nU3.Data
{
    public class LocalDbService : IDBAccessService, IDisposable
    {
        private readonly string _dbPath;
        private readonly string _connectionString;
        private SQLiteConnection? _connection;
        private SQLiteTransaction? _transaction;

        public LocalDbService(string dbFileName = "nU3_Local.db")
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "nU3.Framework", "Database");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            _dbPath = Path.Combine(folder, dbFileName);
            _connectionString = $"Data Source={_dbPath};Version=3;";
            
            // Client might not need to init schema if Server does, but for safety/standalone:
            // We can keep it or rely on Server. Let's assume standalone capability for Deployer.
            // InitializeSchema(); 
        }

        public string GetConnectionString() => _connectionString;

        public bool Connect()
        {
            try
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(_connectionString);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(_connectionString);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void BeginTransaction()
        {
            Connect();
            _transaction = _connection!.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
        }

        private SQLiteCommand CreateCommand(string commandText, Dictionary<string, object>? parameters)
        {
            Connect();
            var cmd = _connection!.CreateCommand();
            cmd.CommandText = commandText;
            cmd.Transaction = _transaction;

            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                }
            }
            return cmd;
        }

        public DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            using var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public async Task<DataTable> ExecuteDataTableAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            using var reader = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public DataSet ExecuteDataSet(string commandText, Dictionary<string, object>? parameters = null)
        {
            var dt = ExecuteDataTable(commandText, parameters);
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public async Task<DataSet> ExecuteDataSetAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            var dt = await ExecuteDataTableAsync(commandText, parameters);
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public bool ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteNonQuery() >= 0;
        }

        public async Task<bool> ExecuteNonQueryAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public object ExecuteScalarValue(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteScalar();
        }

        public async Task<object> ExecuteScalarValueAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return await cmd.ExecuteScalarAsync();
        }

        public bool ExecuteProcedure(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams)
        {
            // SQLite does not support Stored Procedures efficiently. 
            // Mapping to simple Query or Throwing.
            throw new NotSupportedException("SQLite does not support Stored Procedures.");
        }

        public Task<bool> ExecuteProcedureAsync(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams)
        {
            throw new NotSupportedException("SQLite does not support Stored Procedures.");
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }

    // Wrapper expected by other projects
    public class LocalDatabaseManager : IDBAccessService
    {
        private readonly LocalDbService _service;
        private readonly string _dbFilePath;
        private readonly string _connectionString;

        public LocalDatabaseManager(string dbFileName = "nU3_local.db")
        {
            _service = new LocalDbService(dbFileName);
            _connectionString = _service.GetConnectionString();

            // expose path for other components
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "nU3.Framework");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            _dbFilePath = Path.Combine(dir, dbFileName);
        }

        // Keep compatibility method used across codebase
        public string GetConnectionString() => _connectionString;

        public void InitializeSchema()
        {
            // Delegate to LocalDbService implementation to create tables
            // We'll open a direct connection here to create schema (same SQL as before)
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            using var tx = conn.BeginTransaction();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_ROLE (ROLE_CODE INTEGER PRIMARY KEY, ROLE_NAME TEXT, DESCRIPTION TEXT);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_DEPT (DEPT_CODE INTEGER PRIMARY KEY, DEPT_NAME TEXT, DEPT_NAME_ENG TEXT, DESCRIPTION TEXT, DISPLAY_ORDER INTEGER DEFAULT 0, PARENT_DEPT INTEGER, IS_ACTIVE TEXT DEFAULT 'Y', CREATED_DATE TEXT, MODIFIED_DATE TEXT);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SYS_USER (
                        USER_ID TEXT PRIMARY KEY,
                        USERNAME TEXT,
                        PASSWORD TEXT,
                        EMAIL TEXT,
                        ROLE_CODE INTEGER,
                        IS_ACTIVE TEXT DEFAULT 'Y',
                        REG_DATE TEXT DEFAULT (datetime('now')),
                        MOD_DATE TEXT,
                        FOREIGN KEY(ROLE_CODE) REFERENCES SYS_ROLE(ROLE_CODE)
                    );";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_USER_DEPT (USER_ID TEXT, DEPT_CODE INTEGER, IS_PRIMARY TEXT DEFAULT 'N', CREATED_DATE TEXT, PRIMARY KEY(USER_ID, DEPT_CODE));";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_PERMISSION (TARGET_TYPE TEXT, TARGET_ID TEXT, PROG_ID TEXT, CAN_READ INTEGER DEFAULT 0, CAN_CREATE INTEGER DEFAULT 0, CAN_UPDATE INTEGER DEFAULT 0, CAN_DELETE INTEGER DEFAULT 0, CAN_PRINT INTEGER DEFAULT 0, CAN_EXPORT INTEGER DEFAULT 0, CAN_APPROVE INTEGER DEFAULT 0, CAN_CANCEL INTEGER DEFAULT 0, PRIMARY KEY(TARGET_TYPE, TARGET_ID, PROG_ID));";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_PROG_MST (PROG_ID TEXT PRIMARY KEY, PROG_NAME TEXT, MODULE_ID TEXT, IS_ACTIVE TEXT DEFAULT 'Y');";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_MODULE_MST (MODULE_ID TEXT PRIMARY KEY, MODULE_NAME TEXT, FILE_NAME TEXT, IS_ACTIVE TEXT DEFAULT 'Y');";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS SYS_MENU (MENU_ID TEXT PRIMARY KEY, PARENT_ID TEXT, MENU_NAME TEXT, PROG_ID TEXT, SORT_ORD INTEGER DEFAULT 0, AUTH_LEVEL INTEGER DEFAULT 1);";
                cmd.ExecuteNonQuery();

                tx.Commit();
            }

            conn.Close();
        }

        // IDBAccessService compatibility - delegate to LocalDbService
        public bool Connect() => _service.Connect();
        public Task<bool> ConnectAsync() => _service.ConnectAsync();
        public void BeginTransaction() => _service.BeginTransaction();
        public void CommitTransaction() => _service.CommitTransaction();
        public void RollbackTransaction() => _service.RollbackTransaction();
        public System.Data.DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteDataTable(commandText, parameters);
        public Task<System.Data.DataTable> ExecuteDataTableAsync(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteDataTableAsync(commandText, parameters);
        public System.Data.DataSet ExecuteDataSet(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteDataSet(commandText, parameters);
        public Task<System.Data.DataSet> ExecuteDataSetAsync(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteDataSetAsync(commandText, parameters);
        public bool ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteNonQuery(commandText, parameters);
        public Task<bool> ExecuteNonQueryAsync(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteNonQueryAsync(commandText, parameters);
        public object ExecuteScalarValue(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteScalarValue(commandText, parameters);
        public Task<object> ExecuteScalarValueAsync(string commandText, Dictionary<string, object>? parameters = null) => _service.ExecuteScalarValueAsync(commandText, parameters);
        public bool ExecuteProcedure(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams) => _service.ExecuteProcedure(spName, inputParams, outputParams);
        public Task<bool> ExecuteProcedureAsync(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams) => _service.ExecuteProcedureAsync(spName, inputParams, outputParams);
    }
}