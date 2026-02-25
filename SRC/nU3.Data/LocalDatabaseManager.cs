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
    
}