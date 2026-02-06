using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Text.Json;
using nU3.Connectivity;

namespace nU3.Server.Connectivity.Services
{
    /// <summary>
    /// 서버 측 데이터베이스 접근 서비스를 제공합니다.
    /// </summary>
    public class ServerDBAccessService : IDBAccessService, IDisposable
    {
        private readonly string _connectionString;
        private DbConnection? _connection;
        private DbTransaction? _transaction;
        private readonly Func<DbConnection> _connectionFactory;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="connectionString">DB 연결 문자열</param>
        /// <param name="connectionFactory">DbConnection 인스턴스를 생성하는 팩토리 함수(예: () => new OracleConnection())</param>
        public ServerDBAccessService(string connectionString, Func<DbConnection> connectionFactory)
        {
            _connectionString = connectionString;
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// 동기적으로 DB에 연결합니다.
        /// </summary>
        /// <returns>연결 성공 여부</returns>
        public bool Connect()
        {
            try
            {
                if (_connection == null)
                {
                    _connection = _connectionFactory();
                    _connection.ConnectionString = _connectionString;
                }

                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                // TODO: ILogger로 로깅하도록 변경 권장
                Console.WriteLine($"DB 연결 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 비동기적으로 DB에 연결합니다.
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
             try
            {
                if (_connection == null)
                {
                    _connection = _connectionFactory();
                    _connection.ConnectionString = _connectionString;
                }

                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB 연결 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 트랜잭션을 시작합니다.
        /// </summary>
        public void BeginTransaction()
        {
            if (_connection == null || _connection.State != ConnectionState.Open) Connect();
            _transaction = _connection!.BeginTransaction();
        }

        /// <summary>
        /// 트랜잭션을 커밋합니다.
        /// </summary>
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        /// <summary>
        /// 트랜잭션을 롤백합니다.
        /// </summary>
        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
        }

        /// <summary>
        /// DbCommand를 생성하고 파라미터를 추가합니다.
        /// </summary>
        private DbCommand CreateCommand(string commandText, Dictionary<string, object>? parameters)
        {
            if (_connection == null) Connect();
            var cmd = _connection!.CreateCommand();
            cmd.CommandText = commandText;
            cmd.Transaction = _transaction;

            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = kvp.Key;
                    
                    // JsonElement 대응 변환 로직 적용
                    p.Value = ConvertJsonValue(kvp.Value);
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }

        /// <summary>
        /// JsonElement 값을 .NET 표준 타입으로 변환합니다.
        /// </summary>
        private object ConvertJsonValue(object? value)
        {
            if (value is JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.String:
                        return element.GetString() ?? (object)DBNull.Value;
                    case JsonValueKind.Number:
                        if (element.TryGetInt32(out int i)) return i;
                        if (element.TryGetInt64(out long l)) return l;
                        return element.GetDouble();
                    case JsonValueKind.True:
                        return true;
                    case JsonValueKind.False:
                        return false;
                    case JsonValueKind.Null:
                        return DBNull.Value;
                    default:
                        // 객체나 배열은 원문 JSON 문자열로 변환하여 처리 시도
                        return element.GetRawText();
                }
            }
            return value ?? DBNull.Value;
        }

        /// <summary>
        /// 지정한 SQL을 실행하여 DataTable을 반환합니다(동기).
        /// </summary>
        public DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            using var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        /// <summary>
        /// 지정한 SQL을 비동기적으로 실행하여 DataTable을 반환합니다.
        /// </summary>
        public async Task<DataTable> ExecuteDataTableAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            using var reader = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader); // DataTable.Load는 동기 API지만 여기서는 reader가 비동기적으로 준비된 뒤 로드합니다.
            return dt;
        }

        /// <summary>
        /// 지정한 SQL을 실행하여 DataSet을 반환합니다(동기).
        /// </summary>
        public DataSet ExecuteDataSet(string commandText, Dictionary<string, object>? parameters = null)
        {
            var dt = ExecuteDataTable(commandText, parameters);
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// 지정한 SQL을 비동기적으로 실행하여 DataSet을 반환합니다.
        /// </summary>
        public async Task<DataSet> ExecuteDataSetAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            var dt = await ExecuteDataTableAsync(commandText, parameters);
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// INSERT/UPDATE/DELETE 명령을 실행합니다.
        /// </summary>
        public bool ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteNonQuery() >= 0; // 성공 시 영향을 받은 행 수(>=0)를 반환
        }

        /// <summary>
        /// INSERT/UPDATE/DELETE 명령을 비동기적으로 실행합니다.
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        /// <summary>
        /// 단일 값(스칼라)을 반환합니다.
        /// </summary>
        public object ExecuteScalarValue(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// 단일 값(스칼라)을 비동기적으로 반환합니다.
        /// </summary>
        public async Task<object> ExecuteScalarValueAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return await cmd.ExecuteScalarAsync();
        }

        /// <summary>
        /// 저장 프로시저 호출을 동기적으로 실행합니다.
        /// </summary>
        public bool ExecuteProcedure(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams)
        {
            using var cmd = CreateCommand(spName, inputParams);
            cmd.CommandType = CommandType.StoredProcedure;

            if (outputParams != null)
            {
                foreach (var kvp in outputParams)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = kvp.Key;
                    p.Direction = ParameterDirection.Output;
                    p.Size = 4000;
                    cmd.Parameters.Add(p);
                }
            }

            cmd.ExecuteNonQuery();

            if (outputParams != null)
            {
                foreach (DbParameter p in cmd.Parameters)
                {
                    if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
                    {
                        if (outputParams.ContainsKey(p.ParameterName))
                        {
                            outputParams[p.ParameterName] = p.Value;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 저장 프로시저 호출을 비동기적으로 실행합니다.
        /// </summary>
        public async Task<bool> ExecuteProcedureAsync(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams)
        {
            using var cmd = CreateCommand(spName, inputParams);
            cmd.CommandType = CommandType.StoredProcedure;

            if (outputParams != null)
            {
                foreach (var kvp in outputParams)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = kvp.Key;
                    p.Direction = ParameterDirection.Output;
                    p.Size = 4000;
                    cmd.Parameters.Add(p);
                }
            }

            await cmd.ExecuteNonQueryAsync();

            if (outputParams != null)
            {
                foreach (DbParameter p in cmd.Parameters)
                {
                    if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
                    {
                        if (outputParams.ContainsKey(p.ParameterName))
                        {
                            outputParams[p.ParameterName] = p.Value;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 리소스를 해제합니다.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }

    /// <summary>
    /// DbConnection 생성 팩토리 유틸리티입니다.
    /// - Reflection을 사용해 런타임에 적절한 ADO.NET 제공자를 찾아 DbConnection 인스턴스를 생성합니다.
    /// - 필요한 어셈블리가 참조되어 있지 않으면 예외를 발생시킵니다.
    /// </summary>
    public static class DbConnectionFactories
    {
        /// <summary>
        /// Oracle용 DbConnection 팩토리를 생성합니다. Oracle.ManagedDataAccess.Core 어셈블리가 필요합니다.
        /// 사용 예: var factory = DbConnectionFactories.CreateOracleFactory(connStr); var service = new ServerDBAccessService(connStr, factory);
        /// </summary>
        public static Func<DbConnection> CreateOracleFactory(string connectionString)
        {
            return () =>
            {
                // 가능한 타입 이름 목록
                var typeNames = new[]
                {
                    "Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess.Core",
                    "Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess"
                };

                Type? connType = null;
                foreach (var tn in typeNames)
                {
                    connType = Type.GetType(tn, throwOnError: false, ignoreCase: true);
                    if (connType != null) break;
                }

                if (connType == null)
                {
                    throw new InvalidOperationException("Oracle 공급자를 찾을 수 없습니다. Oracle.ManagedDataAccess.Core 패키지를 설치하세요.");
                }

                var conn = (DbConnection?)Activator.CreateInstance(connType);
                if (conn == null) throw new InvalidOperationException("OracleConnection 인스턴스 생성 실패");
                conn.ConnectionString = connectionString;
                return conn;
            };
        }

        /// <summary>
        /// MariaDB(MySQL)용 DbConnection 팩토리를 생성합니다. MySqlConnector 또는 MySql.Data가 필요합니다.
        /// </summary>
        public static Func<DbConnection> CreateMariaDbFactory(string connectionString)
        {
            return () =>
            {
                var typeNames = new[]
                {
                    "MySqlConnector.MySqlConnection, MySqlConnector",
                    "MySql.Data.MySqlClient.MySqlConnection, MySql.Data"
                };

                Type? connType = null;
                foreach (var tn in typeNames)
                {
                    connType = Type.GetType(tn, throwOnError: false, ignoreCase: true);
                    if (connType != null) break;
                }

                if (connType == null)
                {
                    throw new InvalidOperationException("MariaDB/MySQL 공급자를 찾을 수 없습니다. MySqlConnector 또는 MySql.Data 패키지를 설치하세요.");
                }

                var conn = (DbConnection?)Activator.CreateInstance(connType);
                if (conn == null) throw new InvalidOperationException("MySqlConnection 인스턴스 생성 실패");
                conn.ConnectionString = connectionString;
                return conn;
            };
        }

        /// <summary>
        /// SQLite용 DbConnection 팩토리를 생성합니다. Microsoft.Data.Sqlite 또는 System.Data.SQLite가 필요합니다.
        /// </summary>
        public static Func<DbConnection> CreateSqliteFactory(string connectionString)
        {
            return () =>
            {
                var typeNames = new[]
                {
                    "Microsoft.Data.Sqlite.SqliteConnection, Microsoft.Data.Sqlite",
                    "System.Data.SQLite.SQLiteConnection, System.Data.SQLite"
                };

                Type? connType = null;
                foreach (var tn in typeNames)
                {
                    connType = Type.GetType(tn, throwOnError: false, ignoreCase: true);
                    if (connType != null) break;
                }

                if (connType == null)
                {
                    throw new InvalidOperationException("SQLite 공급자를 찾을 수 없습니다. Microsoft.Data.Sqlite 또는 System.Data.SQLite 패키지를 설치하세요.");
                }

                var conn = (DbConnection?)Activator.CreateInstance(connType);
                if (conn == null) throw new InvalidOperationException("SqliteConnection 인스턴스 생성 실패");
                conn.ConnectionString = connectionString;
                return conn;
            };
        }
    }
}
