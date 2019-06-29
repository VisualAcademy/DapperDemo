using DapperDemo.Models;

namespace DapperDemo
{
    class DapperDemo
    {
        static void Main(string[] args)
        {
            //ITableRepository repository = new TableRepository();

            ////repository.RemoveWith(4);

            //var tables = repository.SearchTablesByNote("홍길동123");
            //foreach (var t in tables)
            //{
            //    System.Console.WriteLine($"Id : {t.Id}, Note : {t.Note}");
            //}

            //var ts =
            //    repository.GetMultiData(4);
            //if (ts != null)
            //{
            //    // 부모 출력
            //    System.Console.WriteLine($"Id : {ts.Id}, Note : {ts.Note}");
            //    // 자식 출력
            //    foreach (var subs in ts.SubTableViewModel)
            //    {
            //        System.Console.WriteLine($"\tId : {subs.Id}, " +
            //            $"Note : {subs.Note}, TableId : {subs.TableId} ");
            //    } 
            //}


            ITableRepository repository = new TableRepository();

            var tables = repository.GetAllWithPaging(17 - 1, 10);

            foreach (var t in tables)
            {
                System.Console.WriteLine($"Id : {t.Id}, Note : {t.Note}");
            }

            System.Console.WriteLine(
                $"총 레코드 수 {repository.GetTotalCount()}");

        }
    }
}
