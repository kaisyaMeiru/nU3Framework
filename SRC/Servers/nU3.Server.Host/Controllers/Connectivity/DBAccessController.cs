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
        private readonly ServerDBAccessService _dbService;

        public DBAccessController(ServerDBAccessService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("connect")]
        public async Task<IActionResult> Connect()
        {
            return Ok(await _dbService.ConnectAsync());
        }

        [HttpPost("transaction/begin")]
        public IActionResult BeginTransaction()
        {
            _dbService.BeginTransaction();
            return Ok();
        }

        [HttpPost("transaction/commit")]
        public IActionResult CommitTransaction()
        {
            _dbService.CommitTransaction();
            return Ok();
        }

        [HttpPost("transaction/rollback")]
        public IActionResult RollbackTransaction()
        {
            _dbService.RollbackTransaction();
            return Ok();
        }

        // ============================
        // 실행 관련 메서드
        // ============================

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
                return BadRequest(ex.Message);
            }
        }

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

        // DataTable을 List<Dictionary<string, object>>로 변환하는 헬퍼 메서드
        private List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }
            return list;
        }
    }

    public class QueryRequestDto
    {
        [Required]
        public string CommandText { get; set; } = string.Empty;
        
        public Dictionary<string, object>? Parameters { get; set; }
    }

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
