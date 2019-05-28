using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace DapperDemo.Models
{
    /// <summary>
    /// Tables 테이블과 일대일로 매핑되는 모델 클래스
    /// </summary>
    public class TableViewModel
    {
        public TableViewModel()
        {
            SubTableViewModel = new List<SubTableViewModel>();
        }

        /// <summary>
        /// 일련번호
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 비고
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 하위 테이블에 대한 참조
        /// </summary>
        public List<SubTableViewModel> SubTableViewModel { get; set; }
    }

    /// <summary>
    /// Tables 테이블과 일대일로 매핑되는 모델 클래스
    /// </summary>
    public class SubTableViewModel
    {
        /// <summary>
        /// 일련번호
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 부모 테이블의 기본키
        /// </summary>
        public int TableId { get; set; }
        /// <summary>
        /// 비고
        /// </summary>
        public string Note { get; set; }
    }

    public interface ITableRepository
    {
        TableViewModel Add(TableViewModel model);       // 입력
        List<TableViewModel> GetAll();                  // 출력
        TableViewModel GetById(int id);                 // 상세
        TableViewModel Update(TableViewModel model);    // 수정
        void Remove(int id);                            // 삭제

        // More...
        int BulkInsertRecords(List<TableViewModel> records);
        List<TableViewModel> GetByIds(params int[] ids);
        List<dynamic> GetDynamicAll();
        List<TableViewModel> GetAllWithSp();
        List<TableViewModel> GetByIdWithSp(int id);
        List<TableViewModel> GetByIdWithSpWithDynamicParamter(int id);
        TableViewModel GetMultiData(int id);
        void RemoveWith(int id);
    }

    public class TableRepository : ITableRepository
    {
        private IDbConnection db;

        public TableRepository()
        {
            db = new SqlConnection(
                ConfigurationManager.ConnectionStrings[
                    "ConnectionString"].ConnectionString);
        }

        /// <summary>
        /// 입력
        /// </summary>
        public TableViewModel Add(TableViewModel model)
        {
            var sql =
                "Insert Into Tables (Note) Values (@Note); " +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            var id = db.Query<int>(sql, model).Single();

            model.Id = id;
            return model;
        }

        /// <summary>
        /// 출력
        /// </summary>
        public List<TableViewModel> GetAll()
        {
            string sql = "Select * From Tables";
            return db.Query<TableViewModel>(sql).ToList();
        }

        /// <summary>
        /// 상세
        /// </summary>
        public TableViewModel GetById(int id)
        {
            string sql = "Select * From Tables Where Id = @Id";
            return db.Query<TableViewModel>(sql, new { id }).SingleOrDefault();
        }

        /// <summary>
        /// 수정
        /// </summary>
        public TableViewModel Update(TableViewModel model)
        {
            var sql =
                "Update Tables                  " +
                "Set                            " +
                "    Note       =       @Note   " +
                "Where Id = @Id                 ";
            db.Execute(sql, model);
            return model;
        }

        /// <summary>
        /// 삭제
        /// </summary>
        public void Remove(int id)
        {
            string sql = "Delete From Tables Where Id = @Id";
            db.Execute(sql, new { Id = id });
        }

        /// <summary>
        /// 벌크 인서트
        /// </summary>
        public int BulkInsertRecords(List<TableViewModel> records)
        {
            if (db.State != ConnectionState.Open)
            {
                db.Open();
            }

            var sql =
                "Insert Into Tables (Note) Values (@Note); " +
                "Select Cast(SCOPE_IDENTITY() As Int);";

            return db.Execute(sql, records);
        }

        /// <summary>
        /// 다중 레코드 검색
        /// </summary>
        public List<TableViewModel> GetByIds(params int[] ids)
        {
            string sql = "Select * From Tables Where Id In @Ids";
            return db.Query<TableViewModel>(
                sql, new { Ids = ids }).ToList();
        }

        /// <summary>
        /// Dynamic 출력
        /// </summary>
        public List<dynamic> GetDynamicAll()
        {
            string sql = "Select * From Tables";
            return db.Query(sql).ToList();
        }

        /// <summary>
        /// 저장 프로시저 사용
        /// </summary>
        public List<TableViewModel> GetAllWithSp()
        {
            string sql = "GetTables";
            return db.Query<TableViewModel>(
                sql, commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// 매개변수가 있는 저장 프로시저 사용
        /// </summary>
        public List<TableViewModel> GetByIdWithSp(int id)
        {
            string sql = "GetTableById";
            return db.Query<TableViewModel>(sql, new { Id = id },
                commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// DynamicParameters 사용
        /// </summary>
        public List<TableViewModel> GetByIdWithSpWithDynamicParamter(int id)
        {
            string sql = "GetTableById";

            var parameters = new DynamicParameters();

            parameters.Add("@Id", 
                value: id, 
                dbType: DbType.Int32, 
                direction: ParameterDirection.Input);
            //parameters.Add("@Note", "노트");

            return db.Query<TableViewModel>(sql, parameters,
                commandType: CommandType.StoredProcedure).ToList();

            //parameters.Get<int>("@Id");
        }

        /// <summary>
        /// 다중 테이블에서 데이터 가져오기
        /// </summary>
        public TableViewModel GetMultiData(int id)
        {
            var sql =
                "Select * From Tables Where Id = @Id; " +
                "Select * From SubTables Where TableId = @Id ";

            using (var multiRecords = db.QueryMultiple(sql, new { Id = id }))
            {
                var table = 
                    multiRecords.Read<TableViewModel>().SingleOrDefault();
                var subTable =
                    multiRecords.Read<SubTableViewModel>().ToList();
                if (table != null && subTable != null)
                {
                    table.SubTableViewModel.AddRange(subTable); 
                }

                return table;
            }
        }

        /// <summary>
        /// 트랜잭션 처리: 다중 삭제 또는 다중 업데이트
        /// </summary>
        public void RemoveWith(int id)
        {
            using (var tran = new TransactionScope())
            {
                var sqlTables = "Delete Tables Where Id = @Id";
                db.Execute(sqlTables, new { Id = id });

                var sqlSubTables = "Delete SubTables Where TableId = @Id";
                db.Execute(sqlSubTables, new { Id = id });

                tran.Complete(); 
            }
        }
    }
}
