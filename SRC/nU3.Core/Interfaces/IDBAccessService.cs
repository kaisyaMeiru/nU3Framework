using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 데이터베이스 접근 서비스 인터페이스 (오라클 중심).
    /// 동기 및 비동기 호출을 모두 지원합니다.
    /// </summary>
    public interface IDBAccessService
    {
        // ==========================
        // 연결 및 트랜잭션
        // ==========================
        bool Connect();
        Task<bool> ConnectAsync();

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        // ==========================
        // 데이터 조회 (DataTable)
        // ==========================
        DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null);
        Task<DataTable> ExecuteDataTableAsync(string commandText, Dictionary<string, object>? parameters = null);

        // ==========================
        // 데이터 조회 (DataSet)
        // ==========================
        DataSet ExecuteDataSet(string commandText, Dictionary<string, object>? parameters = null);
        Task<DataSet> ExecuteDataSetAsync(string commandText, Dictionary<string, object>? parameters = null);

        // ==========================
        // 실행 (NonQuery)
        // ==========================
        bool ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null);
        Task<bool> ExecuteNonQueryAsync(string commandText, Dictionary<string, object>? parameters = null);

        // ==========================
        // 스칼라 값
        // ==========================
        object ExecuteScalarValue(string commandText, Dictionary<string, object>? parameters = null);
        Task<object> ExecuteScalarValueAsync(string commandText, Dictionary<string, object>? parameters = null);

        // ==========================
        // 프로시저 호출
        // ==========================
        bool ExecuteProcedure(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams);
        Task<bool> ExecuteProcedureAsync(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams);
    }
}
