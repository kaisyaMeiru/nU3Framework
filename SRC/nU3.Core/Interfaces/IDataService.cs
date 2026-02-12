using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 비즈니스 데이터 접근 서비스 인터페이스
    /// 백엔드(Java Spring 등)와의 통신을 추상화하여 제공합니다.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// 서비스 메서드 실행 (CUD - Create, Update, Delete)
        /// 트랜잭션은 백엔드 서비스에서 관리됩니다.
        /// </summary>
        /// <typeparam name="TRequest">요청 DTO 타입</typeparam>
        /// <typeparam name="TResponse">응답 DTO 타입</typeparam>
        /// <param name="serviceId">백엔드 서비스 Bean ID (예: "ot.opScheduleService")</param>
        /// <param name="method">메서드 명 (예: "saveSchedule")</param>
        /// <param name="request">요청 데이터</param>
        /// <returns>응답 데이터</returns>
        Task<TResponse> ExecuteAsync<TRequest, TResponse>(string serviceId, string method, TRequest request);

        /// <summary>
        /// 데이터 조회 (Read - List)
        /// </summary>
        /// <typeparam name="T">데이터 모델 타입 (DTO)</typeparam>
        /// <param name="serviceId">백엔드 서비스 Bean ID</param>
        /// <param name="method">메서드 명</param>
        /// <param name="parameters">조회 조건 (익명 객체 또는 DTO)</param>
        /// <returns>데이터 목록</returns>
        Task<List<T>> QueryAsync<T>(string serviceId, string method, object parameters = null);

        /// <summary>
        /// 단건 데이터 조회 (Read - Single)
        /// </summary>
        /// <typeparam name="T">데이터 모델 타입 (DTO)</typeparam>
        /// <param name="serviceId">백엔드 서비스 Bean ID</param>
        /// <param name="method">메서드 명</param>
        /// <param name="parameters">조회 조건</param>
        /// <returns>단일 데이터 객체</returns>
        Task<T> QuerySingleAsync<T>(string serviceId, string method, object parameters = null);
    }
}
