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
    /// SubTables 테이블과 일대일로 매핑되는 모델 클래스: XXX, XXXModel, XXXViewModel
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
        /// <summary>
        /// [0] 기본: DB 컨텍스트 개체를 바로 포함해서 사용
        /// </summary>
        List<TableViewModel> GetTables();

        /// <summary>
        /// [1] 입력 패턴
        /// </summary>
        TableViewModel Add(TableViewModel model);

        /// <summary>
        /// [2] 출력 패턴(전체): GetAll(), GetTables()
        /// </summary>
        List<TableViewModel> GetAll();

        /// <summary>
        /// [3] 상세 패턴
        /// </summary>
        TableViewModel GetById(int id);

        /// <summary>
        /// [4] 수정 패턴
        /// </summary>
        TableViewModel Update(TableViewModel model);

        /// <summary>
        /// [6] 삭제 패턴
        /// </summary>
        void Remove(int id);

        /// <summary>
        /// [7] 검색 패턴
        /// </summary>
        List<TableViewModel> SearchTablesByNote(string note);

        /* More... */

        /// <summary>
        /// 입력: 입력 전용
        /// </summary>
        void AddOnly(TableViewModel model);

        /// <summary>
        /// 벌크 인서트
        /// </summary>
        int BulkInsertRecords(List<TableViewModel> records);

        /// <summary>
        /// 다중 레코드 검색
        /// </summary>
        List<TableViewModel> GetByIds(params int[] ids);

        /// <summary>
        /// 선택 삭제: 코드를 보충으로 추가
        /// </summary>
        void DeleteByIds(params int[] ids);

        /// <summary>
        /// Dynamic 출력
        /// </summary>
        List<dynamic> GetDynamicAll();

        /// <summary>
        /// 저장 프로시저 사용
        /// </summary>
        List<TableViewModel> GetAllWithSp();

        /// <summary>
        /// 매개변수가 있는 저장 프로시저 사용
        /// </summary>
        List<TableViewModel> GetByIdWithSp(int id);

        /// <summary>
        /// 저장 프로시저의 매개변수로 DynamicParameters 사용
        /// </summary>
        List<TableViewModel> GetByIdWithSpWithDynamicParamter(int id);

        /// <summary>
        /// 다중 테이블에서 데이터 가져오기
        /// </summary>
        TableViewModel GetMultiData(int id);

        /// <summary>
        /// 트랜잭션 처리: 다중 삭제 또는 다중 업데이트
        /// </summary>
        void RemoveWith(int id);

        /// <summary>
        /// Tables 테이블의 총 레코드 수 반환
        /// </summary>
        int GetTotalCount(); // 총 레코드 수

        /// <summary>
        /// 페이징 처리 후의 데이터 리스트
        /// </summary>
        List<TableViewModel> GetAllWithPaging(int pageIndex, int pageSize);

        /// <summary>
        /// 페이징 처리된 리스트: 인라인 SQL 사용
        /// </summary>
        List<TableViewModel> GetAllWithPagingInline(int pageIndex, int pageSize = 10);

        /// <summary>
        /// 저장 프로시저의 OUTPUT 매개변수 사용
        /// </summary>
        string GetTablesNoteByIdWithOutput(int id);

        /// <summary>
        /// 날짜 검색
        /// </summary>
        List<TableViewModel> SearchTablesByDate(string start, string end);

        /// <summary>
        /// 특정 Id에 해당하는 레코드의 Note 컬럼의 값이 있는지 없는지(NULL) 체크
        /// </summary>
        bool IsExistNoteById(int id);
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
        /// 기본: DB 컨텍스트 개체를 바로 포함해서 사용
        /// </summary>
        public List<TableViewModel> GetTables()
        {
            using (IDbConnection ctx = new SqlConnection(
                ConfigurationManager.ConnectionStrings[
                    "ConnectionString"].ConnectionString))
            {
                if (ctx.State == ConnectionState.Closed)
                {
                    ctx.Open();
                }

                string sql = "Select * From Tables";
                return ctx.Query<TableViewModel>(sql).ToList();
            }
        }

        /// <summary>
        /// 입력: 입력 전용
        /// </summary>
        public void AddOnly(TableViewModel model)
        {
            var sql = "Insert Into Tables (Note) Values (@Note); ";

            db.Execute(sql, model);
        }

        /// <summary>
        /// 입력: 입력 후 Identity 값 반환
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
        /// 상세: 매개 변수 전달 방식 종류 확인(모델, 익명 형식, 개체 이니셜라이저, ...)
        /// </summary>
        public TableViewModel GetById(int id)
        {
            string sql = "Select * From Tables Where Id = @Id";
            return db.Query<TableViewModel>(sql, new { id }).SingleOrDefault();
        }

        /// <summary>
        /// 상세 패턴: 저장 프로시저 사용
        /// </summary>
        /// <param name="uid">Id</param>
        /// <returns>T</returns>
        //public UserModel GetUserInfo(int uid)
        //{
        //     저장 프로시저 이름 또는 인라인 SQL 문(Ad HOC 쿼리)
        //    string sql = "GetUsers";

        //     파라미터 추가
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@UID", value: uid, 
        //        dbType: DbType.Int32, direction: ParameterDirection.Input);

        //     저장 프로시저 실행
        //    return db.Query<UserModel>(sql, parameters, 
        //        commandType: CommandType.StoredProcedure).SingleOrDefault();
        //}

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
        /// 검색 조건 처리시 Like 절 처리 : Note Like N'%홍길동10%'
        /// </summary>
        public List<TableViewModel> SearchTablesByNote(string note)
        {
            string sql =
                "Select * From Tables Where Note Like N'%' + @Note + '%' ";
            return db.Query<TableViewModel>(sql, new { Note = note }).ToList();
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
        /// 선택 삭제: 코드를 보충으로 추가
        /// </summary>
        public void DeleteByIds(params int[] ids)
        {
            string sql = "Delete Tables Where Id In @Ids";
            db.Execute(sql, new { Ids = ids });
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
        /// 저장 프로시저의 매개변수로 DynamicParameters 사용
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

        /// <summary>
        /// 페이징 처리 후의 데이터 리스트
        /// </summary>
        /// <param name="pageIndex">페이지 인덱스(페이지 번호 - 1)</param>
        /// <param name="pageSize">한 페이지에서 보여줄 레코드 수</param>
        public List<TableViewModel> GetAllWithPaging(
            int pageIndex, int pageSize = 10)
        {
            // 인라인 SQL(Ad Hoc 쿼리) 또는 저장 프로시저 지정
            string sql = "GetTablesWithPaging"; // 페이징 저장 프로시저

            // 파라미터 추가
            var parameters = new DynamicParameters();
            parameters.Add("@PageIndex",
                value: pageIndex,
                dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            parameters.Add("@PageSize",
                value: pageSize,
                dbType: DbType.Int32,
                direction: ParameterDirection.Input);

            // 실행
            return db.Query<TableViewModel>(sql, parameters,
                commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// 페이징 처리된 리스트: 인라인 SQL 사용
        /// SELECT * FROM Tables ORDER BY Id 
        /// OFFSET @PageSize * (@PageIndex - 1) ROWS FETCH NEXT @PageSize ROWS ONLY; 
        /// </summary>
        public List<TableViewModel> GetAllWithPagingInline(
            int pageIndex, int pageSize = 10)
        {
            string sql = @"
                Select Id, Note
                From 
                    (
                        Select Row_Number() Over (Order By Id Desc) As RowNumbers, 
                        Id, Note From Tables
                    ) As TempRowTables
                Where 
                    RowNumbers
                        Between
                            (@PageIndex * @PageSize + 1)
                        And
                            (@PageIndex + 1) * @PageSize
            ";
            return db.Query<TableViewModel>(
                sql, new { PageIndex = pageIndex, PageSize = pageSize }).ToList();
        }

        /// <summary>
        /// Tables 테이블의 총 레코드 수 반환
        /// </summary>
        public int GetTotalCount()
        {
            var sql = "Select Count(*) From Tables";

            return db.Query<int>(sql).Single();
        }

        /// <summary>
        /// 저장 프로시저의 OUTPUT 매개변수 사용
        /// </summary>
        public string GetTablesNoteByIdWithOutput(int id)
        {
            string sql = "GetTablesNoteByIdWithOutput";

            var parameters = new DynamicParameters();

            parameters.Add("@Id",
                value: id,
                dbType: DbType.Int32,
                direction: ParameterDirection.Input);
            parameters.Add("@Note",
                value: "",
                dbType: DbType.String,
                direction: ParameterDirection.InputOutput);

            db.Execute(sql, parameters,
               commandType: CommandType.StoredProcedure);

            return parameters.Get<string>("@Note"); // OUTPUT 매개변수
        }

        /// <summary>
        /// 날짜 검색
        /// </summary>
        public List<TableViewModel> SearchTablesByDate(
            string start, string end)
        {
            string sql = @"
                Select * From Tables 
                Where TimeStamp Between @StartDate And @EndDate";
            return db.Query<TableViewModel>(sql, new
            {
                StartDate = start,
                EndDate = end
            }).ToList();
        }

        // 날짜 차이
        //string sql = @"
        //        Select Top 10 UID 
        //        From Users 
        //        Where 
        //            LastLoginDate Is Not Null 
        //            And 
        //            DATEDIFF(day, LastLoginDate, GetDate()) > 365 
        //    ";

        /// <summary>
        /// 특정 Id에 해당하는 레코드의 Note 컬럼의 값이 있는지 없는지(NULL) 체크
        /// </summary>
        public bool IsExistNoteById(int id)
        {
            var sql = "Select Note From Tables Where Id = @Id";

            // Single() : null 값이면 예외 발생(에러가 발생합니다.)
            // SingleOrDefault(): 값이 없으면 null 값을 반환합니다. 
            var result = db.Query<string>(sql, new { Id = id }).SingleOrDefault();

            if (result == null)
            {
                return false;
            }

            return true;
        }
    }
}
