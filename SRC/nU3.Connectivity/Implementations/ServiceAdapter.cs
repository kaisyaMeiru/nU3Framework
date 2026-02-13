using nU3.Connectivity.Models;
using nU3.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace nU3.Connectivity.Implementations
{
    /// <summary>
    /// Legacy Backend (Java Spring)와의 통신을 위한 어댑터
    /// IDataService를 구현하여 비즈니스 로직에 추상화된 데이터 접근을 제공합니다.
    /// </summary>
    public class ServiceAdapter : IDataService
    {
        private readonly HttpClient _httpClient;
        
        public ServiceAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// 공통 호출 메서드
        /// </summary>
        private async Task<T> InvokeAsync<T>(string serviceId, string method, object request, bool isList = false)
        {
            try
            {
                // Gateway URL 구성 (실제 환경에 맞게 조정 필요)
                // 예: /api/services/{serviceId}/{method} 또는 단일 엔드포인트
                //var url = $"/api/services/{serviceId}/{method}";

                var url = $"/api/services/{serviceId}/{method}";
                url = $"https://emr012edu.cmcnu.or.kr/cmcnu/.live?submit_id=TRZMP00101&business_id=zz&instcd=012";
                //https://emr012edu.cmcnu.or.kr/cmcnu/.live?

                // 요청 래핑 (ValueObjectAssembler 구조)
                // Java 측: ValueObjectAssembler pVOs 내에 "req" 키로 ValueObject를 기대함
                var assembler = new ValueObjectAssembler();
                
                // Request가 이미 ValueObject라면 그대로 사용, 아니면 변환 (여기서는 단순 할당)
                // System.Text.Json이 객체를 JSON Object로 직렬화하여 "req" 키에 할당됨
                assembler["req"] = request ?? new object();

                // POST 요청
                var response = await _httpClient.PostAsJsonAsync(url, assembler);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("세션이 만료되었습니다. 다시 로그인해주세요.");
                }
                
                response.EnsureSuccessStatusCode();

                // 응답 파싱
                var resultJson = await response.Content.ReadFromJsonAsync<JsonElement>();
                
                // 표준 응답 구조 확인 ("rtn" 키)
                if (resultJson.TryGetProperty("rtn", out var rtnElement))
                {
                    // 비즈니스 오류 체크 ("returnVal" = false)
                    if (rtnElement.TryGetProperty("returnVal", out var returnVal))
                    {
                        if (returnVal.ValueKind == JsonValueKind.False || (returnVal.ValueKind == JsonValueKind.String && returnVal.GetString() == "false"))
                        {
                             var msg = rtnElement.TryGetProperty("returnMsg", out var returnMsg) 
                                ? returnMsg.GetString() 
                                : "알 수 없는 서버 오류가 발생했습니다.";
                            throw new ApplicationException(msg); // 추후 BusinessLogicException으로 변경 권장
                        }
                    }

                    // 데이터 매핑
                    if (isList)
                    {
                        // 목록 조회 시 "list" 키 확인
                        if (rtnElement.TryGetProperty("list", out var listElement))
                        {
                            return JsonSerializer.Deserialize<T>(listElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        return default; // 리스트가 없는 경우
                    }
                    else
                    {
                        // 단건 조회 또는 실행 결과
                        // T가 단순 타입(bool, int 등)인지 확인하거나, 전체 rtn을 매핑할지 결정
                        // 여기서는 rtn 객체 자체를 매핑 시도 (단, "param"이나 "data" 같은 키가 있다면 조정 필요)
                        return JsonSerializer.Deserialize<T>(rtnElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }
                
                // "rtn" 키가 없는 비표준 응답 처리
                 return JsonSerializer.Deserialize<T>(resultJson.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"서버 통신 오류: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException($"데이터 처리 오류: {ex.Message}", ex);
            }
        }

        public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(string serviceId, string method, TRequest request)
        {
            return await InvokeAsync<TResponse>(serviceId, method, request);
        }

        public async Task<List<T>> QueryAsync<T>(string serviceId, string method, object parameters = null)
        {
            return await InvokeAsync<List<T>>(serviceId, method, parameters, isList: true) ?? new List<T>();
        }

        public async Task<T> QuerySingleAsync<T>(string serviceId, string method, object parameters = null)
        {
            return await InvokeAsync<T>(serviceId, method, parameters);
        }
    }
}
