<%@ Page Language="C#" AutoEventWireup="true" 
    CodeBehind="FrmCustomPagingWithDapper.aspx.cs" 
    Inherits="DapperDemo.Web.FrmCustomPagingWithDapper" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>사용자 정의 페이징</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="ctlLists" runat="server"
            AllowPaging="true" 
            PageSize="10"
            OnPageIndexChanging="ctlLists_PageIndexChanging"
            AllowCustomPaging="true"
            >
        </asp:GridView>
    </div>
    </form>
</body>
</html>
