using System;
using nU3.Core.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace nU3.Connectivity.Implementations
{
    /// <summary>
    /// 데이터베이스 접근 서비스를 HTTP/REST로 구현한 클라이언트입니다.
    /// 
    /// 요약:
    /// - DB 관련 동작(쿼리, 비쿼리, 스칼라, 프로시저 호출 등)을 REST API로 변환하여 서버에 요청합니다.
    /// - 서버 응답은 JSON으로 수신하며, 필요 시 DataTable/DataSet으로 변환합니다.
    /// - 호출자는 DBAccessClientBase의 IDBAccessService 인터페이스를 통해 이 클라이언트를 사용합니다.
    /// 
    /// 설계 고려사항:
    /// - 원격 호출의 실패는 호출자에게 InvalidOperationException으로 래핑하여 전달합니다.
    /// - 대량 데이터 처리를 위해 Post 요청의 타임아웃을 넉넉히 설정할 수 있도록 구성되어야 합니다.
    /// - 프로시저 호출의 출력 파라미터는 서버에서 JSON으로 반환된 OutputParams로 채워집니다.
    /// </summary>
    public class HttpDBAccessClient : DBAccessClientBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// 기본 생성자: 내부적으로 HttpClient를 생성하여 사용합니다.
        /// 주로 간단한 사용에서 기본 HttpClient 사용을 원할 때 호출합니다.
        /// </summary>
        /// <param name="baseUrl">서버 기본 URL (예: "https://localhost:64229") - 끝의 '/'는 내부에서 제거됩니다.</param>
        public HttpDBAccessClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                // 대용량 쿼리/응답을 고려해 비교적 긴 타임아웃을 설정
                Timeout = TimeSpan.FromMinutes(5)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// 외부에서 생성한 HttpClient를 주입받아 사용하는 생성자입니다.
        /// DI(Dependency Injection) 환경이나 단위 테스트에서 HttpClient를 제어해야 할 때 사용합니다.
        /// </summary>
        /// <param name="httpClient">주입받을 HttpClient 인스턴스</param>
        /// <param name="baseUrl">서버 기본 URL</param>
        public HttpDBAccessClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// DB 접근 작업을 원격으로 실행하는 핵심 메서드입니다.
        /// - method: DBAccessClientBase의 메서드 이름 (예: ExecuteDataTable, ExecuteProcedure 등)
        /// - args: 메서드에 전달된 인수들(예: SQL 텍스트, 파라미터 딕셔너리 등)
        /// 
        /// 동작:
        /// 1. 메서드 이름을 기준으로 REST 엔드포인트를 결정(MapMethodToEndpoint)
        /// 2. 필요하면 요청 바디를 생성(CreateRequestData)하여 POST로 전송
        /// 3. 응답을 수신한 후 제네릭 타입 T에 맞게 처리(예: DataTable으로 변환 등)
        /// </summary>
        protected override async Task<T> RemoteExecuteAsync<T>(string method, object[]? args)
        {
            try
            {
                // 메서드 이름을 API 엔드포인트로 매핑
                var endpoint = MapMethodToEndpoint(method);
                
                HttpResponseMessage response;

                if (RequiresBody(method))
                {
                    // 본문을 포함한 POST 요청 (쿼리/파라미터가 있는 경우)
                    var requestData = CreateRequestData(method, args);
                    response = await _httpClient.PostAsJsonAsync(endpoint, requestData).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new InvalidOperationException($"HTTP {(int)response.StatusCode} {response.ReasonPhrase} from {response.RequestMessage?.RequestUri}: {body}");
                    }
                }
                else
                {
                    // 예: Connect, 트랜잭션 시작/종료 등 단순 POST 호출
                    response = await _httpClient.PostAsync(endpoint, null).ConfigureAwait(false);
                }

                // HTTP 상태 코드가 성공인 경우 이어서 결과 처리
                response.EnsureSuccessStatusCode();

                // 반환 타입(T)에 따른 결과 처리
                if (method == nameof(ExecuteProcedure) || method == nameof(ExecuteProcedureAsync))
                {
                    // 프로시저 호출은 성공 여부와 OutputParams 딕셔너리를 반환하는 구조를 가정
                    var resultObj = await response.Content.ReadFromJsonAsync<ProcedureResultDto>(_jsonOptions).ConfigureAwait(false);
                    
                    if (resultObj != null && args != null && args.Length > 2)
                    {
                        // 클라이언트에서 전달한 outputParams 딕셔너리를 서버 결과로 갱신
                        if (args[2] is Dictionary<string, object> clientOutputParams && resultObj.OutputParams != null)
                        {
                            foreach (var kvp in resultObj.OutputParams)
                            {
                                if (clientOutputParams.ContainsKey(kvp.Key))
                                {
                                    // Json으로 전달된 값은 object로 들어오므로 타입 변환이 필요한 경우 호출자가 처리해야 함
                                    clientOutputParams[kvp.Key] = kvp.Value;
                                }
                            }
                        }
                        // 프로시저 성공 여부 반환
                        return (T)(object)resultObj.Success;
                    }
                    return (T)(object)(resultObj?.Success ?? false);
                }
                else if (typeof(T) == typeof(bool))
                {
                    // 단순 성공/실패를 bool로 반환하는 API
                    var result = await response.Content.ReadFromJsonAsync<bool>(_jsonOptions).ConfigureAwait(false);
                    return (T)(object)result!;
                }
                else if (typeof(T) == typeof(DataTable))
                {
                    // 서버는 테이블 데이터를 List<Dictionary<string, object>>로 반환한다고 가정
                    var data = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>(_jsonOptions).ConfigureAwait(false);
                    return (T)(object)ConvertToDataTable(data);
                }
                else if (typeof(T) == typeof(DataSet))
                {
                    // 단일 테이블을 DataSet으로 감싸서 반환
                    var data = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>(_jsonOptions).ConfigureAwait(false);
                    var dt = ConvertToDataTable(data);
                    var ds = new DataSet();
                    ds.Tables.Add(dt);
                    return (T)(object)ds;
                }
                else if (typeof(T) == typeof(object))
                {
                    var result = await response.Content.ReadFromJsonAsync<object>(_jsonOptions).ConfigureAwait(false);
                    return (T)result!;
                }
                else
                {
                    // 제네릭 타입으로 JSON 역직렬화
                    var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions).ConfigureAwait(false);
                    return result!;
                }
            }
            catch (HttpRequestException ex)
            {
                // 네트워크 관련 예외를 호출자에게 명확하게 전달
                throw new InvalidOperationException($"원격 호출 중 HTTP 요청 실패 (메서드: '{method}'): {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // 기타 예외는 호출자에게 전달
                throw new InvalidOperationException($"원격 실행 실패 (메서드: '{method}'): {ex.Message}", ex);
            }
        }

        // 프로시저 결과 DTO (서버가 이 구조로 반환한다고 가정)
        private class ProcedureResultDto
        {
            /// <summary>
            /// 프로시저의 성공 여부
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// 출력 파라미터 딕셔너리 (키: 파라미터명, 값: 반환값)
            /// </summary>
            public Dictionary<string, object>? OutputParams { get; set; }
        }

        /// <summary>
        /// 메서드 이름을 REST 엔드포인트 URL로 매핑합니다.
        /// 외부 API의 경로가 변경되면 이 매핑을 수정해야 합니다.
        /// </summary>
        private string MapMethodToEndpoint(string method)
        {
            return method switch
            {
                nameof(Connect) or nameof(ConnectAsync) => "/api/v1/db/connect",
                nameof(BeginTransaction) => "/api/v1/db/transaction/begin",
                nameof(CommitTransaction) => "/api/v1/db/transaction/commit",
                nameof(RollbackTransaction) => "/api/v1/db/transaction/rollback",
                nameof(ExecuteDataTable) or nameof(ExecuteDataTableAsync) => "/api/v1/db/query/table",
                nameof(ExecuteDataSet) or nameof(ExecuteDataSetAsync) => "/api/v1/db/query/table",
                nameof(ExecuteNonQuery) or nameof(ExecuteNonQueryAsync) => "/api/v1/db/query/nonquery",
                nameof(ExecuteScalarValue) or nameof(ExecuteScalarValueAsync) => "/api/v1/db/query/scalar",
                nameof(ExecuteProcedure) or nameof(ExecuteProcedureAsync) => "/api/v1/db/procedure",
                _ => throw new NotSupportedException($"메서드 '{method}'는 지원되지 않습니다")
            };
        }

        /// <summary>
        /// 해당 메서드가 요청 본문을 필요로 하는지 여부를 판단합니다.
        /// 일부 간단한 엔드포인트는 본문 없이 호출될 수 있습니다.
        /// </summary>
        private bool RequiresBody(string method)
        {
            return method switch
            {
                nameof(Connect) or nameof(ConnectAsync) => false,
                nameof(BeginTransaction) => false,
                nameof(CommitTransaction) => false,
                nameof(RollbackTransaction) => false,
                _ => true
            };
        }

        /// <summary>
        /// 메서드와 인자에 따라 요청에 사용할 데이터를 생성합니다.
        /// 반환되는 객체는 JSON으로 직렬화되어 POST 본문으로 전송됩니다.
        /// </summary>
        private object CreateRequestData(string method, object[]? args)
        {
            if (args == null || args.Length == 0)
                return new { };

            return method switch
            {
                // SQL 쿼리/파라미터 기반 호출
                nameof(ExecuteDataTable) or nameof(ExecuteDataTableAsync) or
                nameof(ExecuteDataSet) or nameof(ExecuteDataSetAsync) or
                nameof(ExecuteNonQuery) or nameof(ExecuteNonQueryAsync) or
                nameof(ExecuteScalarValue) or nameof(ExecuteScalarValueAsync) =>
                    new
                    {
                        CommandText = args[0]?.ToString() ?? string.Empty,
                        //Parameters = args.Length > 1 ? args[1] as Dictionary<string, object> ?? new Dictionary<string, object>() : new Dictionary<string, object>()
                        Parameters = args.Length > 1 ? args[1] as Dictionary<string, object> : null
                    },

                // 프로시저 호출: 이름 + 입력/출력 파라미터
                nameof(ExecuteProcedure) or nameof(ExecuteProcedureAsync) =>
                    new
                    {
                        SpName = args[0]?.ToString() ?? string.Empty,
                        InputParams = args.Length > 1 ? args[1] as Dictionary<string, object> : new Dictionary<string, object>(),
                        OutputParams = args.Length > 2 ? args[2] as Dictionary<string, object> : new Dictionary<string, object>()
                    },

                _ => new { }
            };
        }

        /// <summary>
        /// 서버에서 반환한 List<Dictionary<string, object>> 형식을 DataTable로 변환합니다.
        /// - 첫 행의 키들을 컬럼으로 사용
        /// - 각 값은 System.Text.Json.JsonElement인 경우 적절히 변환
        /// </summary>
        private DataTable ConvertToDataTable(List<Dictionary<string, object>>? data)
        {
            var dt = new DataTable();

            if (data == null || data.Count == 0)
                return dt;

            // 첫 행의 키를 기반으로 컬럼 생성
            foreach (var key in data[0].Keys)
            {
                dt.Columns.Add(key);
            }

            // 각 행을 DataRow로 변환
            foreach (var row in data)
            {
                var dataRow = dt.NewRow();
                foreach (var kvp in row)
                {
                    if (kvp.Value is JsonElement element)
                    {
                        // JsonElement의 ValueKind에 따라 적절한 .NET 타입으로 변환
                        switch (element.ValueKind)
                        {
                            case JsonValueKind.String:
                                dataRow[kvp.Key] = element.GetString();
                                break;
                            case JsonValueKind.Number:
                                // 정수/실수 타입을 우선적으로 시도
                                if (element.TryGetInt64(out long l)) dataRow[kvp.Key] = l;
                                else if (element.TryGetDouble(out double d)) dataRow[kvp.Key] = d;
                                else dataRow[kvp.Key] = element.ToString();
                                break;
                            case JsonValueKind.True:
                                dataRow[kvp.Key] = true;
                                break;
                            case JsonValueKind.False:
                                dataRow[kvp.Key] = false;
                                break;
                            case JsonValueKind.Null:
                                dataRow[kvp.Key] = DBNull.Value;
                                break;
                            default:
                                // 객체/배열 등은 ToString으로 저장(필요시 확장 고려)
                                dataRow[kvp.Key] = element.ToString();
                                break;
                        }
                    }
                    else
                    {
                        // 일반 CLR 타입인 경우 그대로 할당, null은 DBNull로 변환
                        dataRow[kvp.Key] = kvp.Value ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(dataRow);
            }

            return dt;
        }
    }
}
