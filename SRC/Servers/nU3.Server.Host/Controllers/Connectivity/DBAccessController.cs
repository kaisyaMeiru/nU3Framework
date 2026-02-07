using Microsoft.AspNetCore.Mvc;
using nU3.Connectivity;
using nU3.Server.Connectivity.Services;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace nU3.Server.Host.Controllers.Connectivity
{
    [ApiController]
    [Route("api/v1/db")]
    public class DBAccessController : ControllerBase
    {
        // 데이터베이스 접근 및 쿼리 실행을 담당하는 서비스
        private readonly ServerDBAccessService _dbService;

        // 생성자: DI로 ServerDBAccessService를 주입받음
        public DBAccessController(ServerDBAccessService dbService)
        {
            _dbService = dbService;
        }

        // 데이터베이스 연결 테스트 API
        [HttpPost("connect")]
        public async Task<IActionResult> Connect()
        {
            return Ok(await _dbService.ConnectAsync());
        }

        // 트랜잭션 시작
        [HttpPost("transaction/begin")]
        public IActionResult BeginTransaction()
        {
            _dbService.BeginTransaction();
            return Ok(true);
        }

        // 트랜잭션 커밋
        [HttpPost("transaction/commit")]
        public IActionResult CommitTransaction()
        {
            _dbService.CommitTransaction();
            return Ok(true);
        }

        // 트랜잭션 롤백
        [HttpPost("transaction/rollback")]
        public IActionResult RollbackTransaction()
        {
            _dbService.RollbackTransaction();
            return Ok(true);
        }

        // ============================
        // 쿼리 실행 관련 메서드
        // ============================

        // DataTable 형태의 결과를 반환하는 쿼리 실행
        [HttpPost("query/table")]
        public async Task<IActionResult> ExecuteDataTable([FromBody] QueryRequestDto request)
        {
            try
            {
                var dt = await _dbService.ExecuteDataTableAsync(request.CommandText, request.Parameters);
                return Ok(ConvertDataTableToList(dt));
            }
            catch (Exception ex)
            {
                // 예외 발생 시 BadRequest로 메시지 반환
                return BadRequest(ex.Message);
            }
        }

        // INSERT/UPDATE/DELETE 등의 쿼리 실행(영향받은 행 수 반환)
        [HttpPost("query/nonquery")]
        public async Task<IActionResult> ExecuteNonQuery([FromBody] QueryRequestDto request)
        {
            try
            {
                var result = await _dbService.ExecuteNonQueryAsync(request.CommandText, request.Parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 단일 값(Scalar) 쿼리 실행
        [HttpPost("query/scalar")]
        public async Task<IActionResult> ExecuteScalar([FromBody] QueryRequestDto request)
        {
            try
            {
                var result = await _dbService.ExecuteScalarValueAsync(request.CommandText, request.Parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 저장 프로시저 실행 (입력/출력 파라미터 처리)
        [HttpPost("procedure")]
        public async Task<IActionResult> ExecuteProcedure([FromBody] ProcedureRequestDto request)
        {
            try
            {
                var result = await _dbService.ExecuteProcedureAsync(request.SpName, request.InputParams, request.OutputParams);
                return Ok(new { Success = result, OutputParams = request.OutputParams });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DataTable을 List<Dictionary<string, object>>로 변환하여 JSON 직렬화에 적합한 형태로 반환
        private List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null! : row[col];
                }
                list.Add(dict);
            }
            return list;
        }
    }

    // 쿼리 요청 DTO: 실행할 SQL과 선택적 파라미터 포함
    public class QueryRequestDto
    {
        [Required]
        public string CommandText { get; set; } = string.Empty;
        
        public Dictionary<string, object>? Parameters { get; set; }
    }

    // 저장 프로시저 호출용 DTO: 프로시저명, 입력/출력 파라미터
    public class ProcedureRequestDto
    {
        [Required]
        public string SpName { get; set; } = string.Empty;
        
        [Required]
        public Dictionary<string, object> InputParams { get; set; } = new();
        
        [Required]
        public Dictionary<string, object> OutputParams { get; set; } = new();
    }
}
