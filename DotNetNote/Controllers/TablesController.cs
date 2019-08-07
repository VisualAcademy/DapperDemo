using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class TableViewModel
    {
        public int Id { get; set; }
        public string Note { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        // GET: api/Tables
        [HttpGet]
        public IEnumerable<TableViewModel> Get()
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=DapperDemo;Trusted_Connection=True;";

            SqlConnection db = new SqlConnection(connectionString); 

            string sql = "GetTables";

            return db.Query<TableViewModel>(sql, commandType: CommandType.StoredProcedure).ToList();
        }

        // GET: api/Tables/5
        [HttpGet("{id}", Name = "Get")]
        public TableViewModel Get(int id, string foo = "", string bar = "")
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=DapperDemo;Trusted_Connection=True;";

            SqlConnection db = new SqlConnection(connectionString);

            string sql = "GetTableById";

            if (foo != "")
            {
                id = id * 2; // 매개 변수를 받는 것 테스트
            }

            return db.Query<TableViewModel>(sql, new { Id = id }, commandType: CommandType.StoredProcedure).SingleOrDefault();
        }

        // POST: api/Tables
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Tables/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
