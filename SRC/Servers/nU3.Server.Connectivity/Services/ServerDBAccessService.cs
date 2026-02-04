using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace nU3.Server.Connectivity.Services
{
    /// <summary>
    /// 서버 측 데이터베이스 접근 서비스입니다.
    /// 
    /// 책임:
    /// - 데이터베이스 연결/트랜잭션 관리를 제공
    /// - SQL 실행 및 프로시저 호출을 동기/비동기 방식으로 지원
    /// 
    /// 사용법:
    /// - 생성 시 연결 문자열과 DbConnection 생성 팩토리를 전달하세요.
    /// - Oracle, SQLServer 등 특정 공급자에 맞는 DbConnection을 팩토리로 제공해야 합니다.
    /// </summary>
    public class ServerDBAccessService : IDisposable
    {
        private readonly string _connectionString;
        private DbConnection? _connection;
        private DbTransaction? _transaction;
        private readonly Func<DbConnection> _connectionFactory;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="connectionString">DB 연결 문자열</param>
        /// <param name="connectionFactory">DbConnection을 생성하는 팩토리 함수 (예: () => new OracleConnection())</param>
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
                // 콘솔에 간단히 로깅합니다. 필요 시 ILogger로 대체하세요.
                Console.WriteLine($"DB 연결 오류: {ex.Message}");
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
                Console.WriteLine($"DB 연결 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 트랜잭션 시작
        /// </summary>
        public void BeginTransaction()
        {
            if (_connection == null || _connection.State != ConnectionState.Open) Connect();
            _transaction = _connection!.BeginTransaction();
        }

        /// <summary>
        /// 트랜잭션 커밋
        /// </summary>
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        /// <summary>
        /// 트랜잭션 롤백
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
                    p.Value = kvp.Value ?? DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }

        /// <summary>
        /// 쿼리를 실행하여 DataTable을 반환합니다(동기).
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
        /// 쿼리를 비동기적으로 실행하여 DataTable을 반환합니다.
        /// </summary>
        public async Task<DataTable> ExecuteDataTableAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            using var reader = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader); // DataTable.Load는 비동기가 아니므로 주의
            return dt;
        }

        /// <summary>
        /// 쿼리를 실행하여 DataSet을 반환합니다(동기).
        /// </summary>
        public DataSet ExecuteDataSet(string commandText, Dictionary<string, object>? parameters = null)
        {
            var dt = ExecuteDataTable(commandText, parameters);
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// 쿼리를 비동기적으로 실행하여 DataSet을 반환합니다.
        /// </summary>
        public async Task<DataSet> ExecuteDataSetAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            var dt = await ExecuteDataTableAsync(commandText, parameters);
            var ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// 비쿼리를 실행합니다(INSERT/UPDATE/DELETE 등).
        /// </summary>
        public bool ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteNonQuery() >= 0; // 일부 공급자는 -1을 반환할 수 있음
        }

        /// <summary>
        /// 비쿼리를 비동기적으로 실행합니다.
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        /// <summary>
        /// 단일 스칼라 값을 반환합니다.
        /// </summary>
        public object ExecuteScalarValue(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// 스칼라 값을 비동기적으로 반환합니다.
        /// </summary>
        public async Task<object> ExecuteScalarValueAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(commandText, parameters);
            return await cmd.ExecuteScalarAsync();
        }

        /// <summary>
        /// 저장 프로시저를 실행하고 출력 파라미터를 갱신합니다.
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
        /// 저장 프로시저를 비동기적으로 실행하고 출력 파라미터를 갱신합니다.
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
        /// 연결/트랜잭션 관련 자원을 해제합니다.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }

    /// <summary>
    /// DbConnection 생성을 도와주는 팩토리 유틸리티입니다.
    /// - Reflection을 사용하여 런타임에 제공되는 ADO.NET 공급자 어셈블리에서 Connection 타입을 찾습니다.
    /// - Oracle.ManagedDataAccess.Core, MySqlConnector(MySqlConnector) 및 MySql.Data 지원
    /// </summary>
    public static class DbConnectionFactories
    {
        /// <summary>
        /// Oracle용 DbConnection 팩토리를 생성합니다. Oracle.ManagedDataAccess.Core 어셈블리가 설치되어 있어야 합니다.
        /// 사용 예: var factory = DbConnectionFactories.CreateOracleFactory(connStr); var service = new ServerDBAccessService(connStr, factory);
        /// </summary>
        public static Func<DbConnection> CreateOracleFactory(string connectionString)
        {
            return () =>
            {
                // 가능한 타입 이름들을 시도합니다.
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
                    throw new InvalidOperationException("Oracle 공급자를 찾을 수 없습니다. Oracle.ManagedDataAccess.Core 패키지를 설치하고 참조를 추가하세요.");
                }

                var conn = (DbConnection?)Activator.CreateInstance(connType);
                if (conn == null) throw new InvalidOperationException("OracleConnection 인스턴스 생성 실패");
                conn.ConnectionString = connectionString;
                return conn;
            };
        }

        /// <summary>
        /// MariaDB(MySQL)용 DbConnection 팩토리를 생성합니다. 우선 MySqlConnector를 시도하고, 없으면 MySql.Data를 시도합니다.
        /// 사용 예: var factory = DbConnectionFactories.CreateMariaDbFactory(connStr); var service = new ServerDBAccessService(connStr, factory);
        /// </summary>
        public static Func<DbConnection> CreateMariaDbFactory(string connectionString)
        {
            return () =>
            {
                // MySqlConnector
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
                    throw new InvalidOperationException("MariaDB/MySQL 공급자를 찾을 수 없습니다. MySqlConnector 또는 MySql.Data 패키지를 설치하고 참조를 추가하세요.");
                }

                var conn = (DbConnection?)Activator.CreateInstance(connType);
                if (conn == null) throw new InvalidOperationException("MySqlConnection 인스턴스 생성 실패");
                conn.ConnectionString = connectionString;
                return conn;
            };
        }

        /// <summary>
        /// SQLite용 DbConnection 팩토리를 생성합니다. Microsoft.Data.Sqlite 또는 System.Data.SQLite 어셈블리가 설치되어 있어야 합니다.
        /// 사용 예: var factory = DbConnectionFactories.CreateSqliteFactory(connStr); var service = new ServerDBAccessService(connStr, factory);
        /// </summary>
        public static Func<DbConnection> CreateSqliteFactory(string connectionString)
        {
            return () =>
            {
                // 가능한 타입 이름들을 시도합니다.
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
                    throw new InvalidOperationException("SQLite 공급자를 찾을 수 없습니다. Microsoft.Data.Sqlite 또는 System.Data.SQLite 패키지를 설치하고 참조를 추가하세요.");
                }

                var conn = (DbConnection?)Activator.CreateInstance(connType);
                if (conn == null) throw new InvalidOperationException("SqliteConnection 인스턴스 생성 실패");
                conn.ConnectionString = connectionString;
                return conn;
            };
        }
    }
}
