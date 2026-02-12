using System;
using nU3.Core.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace nU3.Connectivity.Implementations
{
    /// <summary>
    /// 데이터베이스 접근 서비스를 위한 클라이언트 기본 구현(추상).
    /// 
    /// 책임:
    /// - IDBAccessService 인터페이스에 정의된 데이터베이스 작업을 원격 호출로 위임
    /// - 동기 API는 내부적으로 비동기 호출을 블록킹 방식으로 호출(주의: UI 스레드 차단 가능)
    /// - 실제 원격 호출 로직은 하위 클래스가 RemoteExecuteAsync<T>를 구현하여 처리
    /// 
    /// 동작 모델:
    /// - 각 메서드는 메서드 이름과 인자를 RemoteExecuteAsync로 전달
    /// - RemoteExecuteAsync는 HTTP/gRPC 등 원하는 전송 방식을 사용하여 서버와 통신하고 결과를 역직렬화하여 반환
    /// 
    /// 예외 및 오류 처리:
    /// - 네트워크/직렬화 오류는 RemoteExecuteAsync의 구현에 의해 발생하며 이 클래스에서는 그대로 호출자에게 전달
    /// - 동기 메서드 호출 시 비동기 작업 대기에서 발생한 예외는 호출자에게 던져집니다.
    /// </summary>
    public abstract class DBAccessClientBase : IDBAccessService
    {
        /// <summary>
        /// 실제 원격 호출을 수행할 추상 메서드입니다.
        /// 하위 클래스는 이 메서드를 구현하여 지정된 메서드 이름과 인자에 해당하는 원격 엔드포인트를 호출하고,
        /// 결과를 제네릭 타입 T로 역직렬화하여 반환해야 합니다.
        /// 
        /// 주의사항:
        /// - method: 호출하려는 IDBAccessService 메서드 이름(예: ExecuteDataTable, ExecuteProcedure 등)
        /// - args: 메서드에 전달된 인자 배열(없으면 null)
        /// - 반환 타입 T는 호출자가 기대하는 타입이어야 하며, 하위 구현은 이를 충족하도록 직렬화/역직렬화를 수행해야 합니다.
        /// </summary>
        protected abstract Task<T> RemoteExecuteAsync<T>(string method, object[] args);

        /// <summary>
        /// 동기 연결 시도. 내부적으로 비동기 ConnectAsync를 블록킹 방식으로 호출합니다.
        /// UI 스레드에서 직접 호출하면 애플리케이션이 일시 중지될 수 있으니 주의하세요.
        /// </summary>
        public bool Connect() => ConnectAsync().GetAwaiter().GetResult();

        /// <summary>
        /// 원격 연결을 비동기적으로 시도합니다.
        /// 내부적으로 RemoteExecuteAsync에 nameof(Connect)를 전달하여 원격 연결 엔드포인트를 호출합니다.
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            return await RemoteExecuteAsync<bool>(nameof(Connect), null).ConfigureAwait(false);
        }

        /// <summary>
        /// 동기 방식으로 트랜잭션을 시작합니다. 내부적으로 원격 호출을 수행합니다.
        /// </summary>
        public void BeginTransaction() => RemoteExecuteAsync<object>(nameof(BeginTransaction), null).GetAwaiter().GetResult();

        /// <summary>
        /// 트랜잭션 커밋을 동기적으로 수행합니다.
        /// </summary>
        public void CommitTransaction() => RemoteExecuteAsync<object>(nameof(CommitTransaction), null).GetAwaiter().GetResult();

        /// <summary>
        /// 트랜잭션 롤백을 동기적으로 수행합니다.
        /// </summary>
        public void RollbackTransaction() => RemoteExecuteAsync<object>(nameof(RollbackTransaction), null).GetAwaiter().GetResult();

        /// <summary>
        /// SQL 쿼리 결과를 DataTable로 반환합니다 (동기). 내부적으로 비동기 호출을 대기합니다.
        /// </summary>
        public DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null) => ExecuteDataTableAsync(commandText, parameters).GetAwaiter().GetResult();

        /// <summary>
        /// SQL 쿼리 결과를 DataTable로 비동기 반환합니다.
        /// - request: { CommandText, Parameters }
        /// - 서버는 표 형식 결과를 JSON 배열(행: 딕셔너리) 등으로 반환하고 클라이언트가 이를 DataTable로 변환합니다.
        /// </summary>
        public async Task<DataTable> ExecuteDataTableAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            return await RemoteExecuteAsync<DataTable>(nameof(ExecuteDataTable), new object[] { commandText, parameters }).ConfigureAwait(false);
        }

        /// <summary>
        /// SQL 쿼리 결과를 DataSet으로 반환합니다 (동기).
        /// </summary>
        public DataSet ExecuteDataSet(string commandText, Dictionary<string, object>? parameters = null) => ExecuteDataSetAsync(commandText, parameters).GetAwaiter().GetResult();

        /// <summary>
        /// SQL 쿼리 결과를 DataSet으로 비동기 반환합니다.
        /// 서버가 단일 테이블을 반환하는 경우 DataSet.Tables[0]에 결과가 들어있을 것입니다.
        /// </summary>
        public async Task<DataSet> ExecuteDataSetAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            return await RemoteExecuteAsync<DataSet>(nameof(ExecuteDataSet), new object[] { commandText, parameters }).ConfigureAwait(false);
        }

        /// <summary>
        /// 비쿼리(INSERT/UPDATE/DELETE 등)를 실행하고 성공 여부를 반환합니다 (동기).
        /// </summary>
        public bool ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null) => ExecuteNonQueryAsync(commandText, parameters).GetAwaiter().GetResult();

        /// <summary>
        /// 비쿼리를 비동기적으로 실행합니다.
        /// 반환 타입은 bool(성공 여부)을 기대합니다.
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
             return await RemoteExecuteAsync<bool>(nameof(ExecuteNonQuery), new object[] { commandText, parameters }).ConfigureAwait(false);
        }

        /// <summary>
        /// 단일 스칼라 값을 반환하는 쿼리를 동기적으로 실행합니다.
        /// </summary>
        public object ExecuteScalarValue(string commandText, Dictionary<string, object>? parameters = null) => ExecuteScalarValueAsync(commandText, parameters).GetAwaiter().GetResult();

        /// <summary>
        /// 스칼라 값을 비동기적으로 조회합니다.
        /// 반환 타입은 object이며, 실제 타입은 호출자가 캐스팅하여 사용해야 합니다.
        /// </summary>
        public async Task<object> ExecuteScalarValueAsync(string commandText, Dictionary<string, object>? parameters = null)
        {
            return await RemoteExecuteAsync<object>(nameof(ExecuteScalarValue), new object[] { commandText, parameters }).ConfigureAwait(false);
        }

        /// <summary>
        /// 저장 프로시저를 실행하고 출력 파라미터를 갱신합니다 (동기).
        /// </summary>
        public bool ExecuteProcedure(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams) => ExecuteProcedureAsync(spName, inputParams, outputParams).GetAwaiter().GetResult();

        /// <summary>
        /// 저장 프로시저를 비동기적으로 실행합니다.
        /// - 입력: spName, inputParams, outputParams(빈 딕셔너리 전달 가능)
        /// - 서버는 출력 파라미터를 포함한 구조(예: { success: true, outputParams: {...} })로 응답하고,
        ///   RemoteExecuteAsync의 구현은 outputParams를 적절히 갱신해야 합니다.
        /// 
        /// 참고: 원격 호출의 특성상 outputParams 처리는 호출 규약에 따라 설계되어야 하며,
        /// 이 구현은 단순히 서버에 args를 전달하는 형태입니다. 필요 시 리턴값으로 출력 파라미터를 담아 받도록 프로토콜을 개선하세요.
        /// </summary>
        public async Task<bool> ExecuteProcedureAsync(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams)
        {
            // 원격 호출에서 Output 파라미터를 다루려면 반환값으로 Output을 포함한 객체를 사용해야 할 수 있습니다.
            // 현재 구현에서는 간단히 원격 호출을 수행하도록 처리합니다.
            return await RemoteExecuteAsync<bool>(nameof(ExecuteProcedure), new object[] { spName, inputParams, outputParams }).ConfigureAwait(false);
        }
    }
}
