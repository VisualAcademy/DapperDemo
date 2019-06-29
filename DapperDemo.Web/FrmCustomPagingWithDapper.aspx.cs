using DapperDemo.Models;
using System;
using System.Web.UI;

namespace DapperDemo.Web
{
    public partial class FrmCustomPagingWithDapper : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // 처음 로드할 때에는 0번째 인덱스의 페이지부터 표시
                DisplayData(0, ctlLists.PageSize);
            }
        }

        private void DisplayData(int pageIndex, int pageSize)
        {
            ITableRepository repository = new TableRepository();

            // 페이징 처리를 한 후의 필요한 데이터만 가져오기
            var tables = repository.GetAllWithPaging(pageIndex, pageSize);
            
            ctlLists.PageSize = pageSize;
            ctlLists.VirtualItemCount = repository.GetTotalCount();
            ctlLists.PageIndex = pageIndex;

            ctlLists.DataSource = tables;
            ctlLists.DataBind(); 
        }

        protected void ctlLists_PageIndexChanging(
            object sender, 
            System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            DisplayData(e.NewPageIndex, ctlLists.PageSize);
        }
    }
}
