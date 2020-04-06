<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm_ShowGrid.aspx.cs" Inherits="kittingStatus.jabil.web.WebUI.WebForm_ShowGrid" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1"  runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
 
    <link rel="shortcut icon" href="../images/PageLog.ico" type="image/x-icon">
    <title></title>
    <link href="../Style/global.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-table.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-editable.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-table-group-by.css" rel="stylesheet" type="text/css" />
    <link href="../Style/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />

    <script src="../Scripts/jquery-1.10.2.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Scripts/bootstrap-table.js"></script>
    <script src="../Scripts/bootstrap-table-export.js"></script>
    <script src="../Scripts/tableExport.js"></script>    


    <script src="../Scripts/bootstrap-editable.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-table-group-by.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-table-editable.js" type="text/javascript"></script>

    <script>

        var _ID = '<%=ID%>';
        var _Tpye = '<%=Tpye%>';


        $(document).ready(function () {
           
            initTable();
            
        })
      
        function initTable() {
            var url = "../Data/GetData_ByID.ashx";   //?ID=" + ID + "&Tpye=" + Tpye           
            $('#tb').bootstrapTable({
                method: 'get',
                dataType: 'json',
                cache: false,
                striped: true,                              //是否显示行间隔色
                url: url,
                searchAlign: 'left',
                search: false,      //是否显示表格搜索，此搜索是客户端搜索，不会进服务端                     
                // showColumns: true,
                pagination: true,
                queryParams: queryParams,
                minimumCountColumns: 2,
                pageNumber: 1,                       //初始化加载第一页，默认第一页
                pageSize: 4,
                pageList: [4, 20, 100],
                uniqueId: "ToolID",                     //每一行的唯一标识，一般为主键列
                showExport: false,
                exportDataType: 'all',
                columns: [
                                        {
                                            field: 'ToolID',
                                            title: 'ToolID',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                        {
                                            field: 'ToolTypeName',
                                            title: 'ToolTypeName',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                        {
                                            field: 'ToolSN',
                                            title: 'ToolSN',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                         {
                                             field: 'Status',
                                             title: 'Status',
                                             align: 'center',
                                             valign: 'middle',
                                             sortable: true
                                         },
                                        {
                                            field: 'Model',
                                            title: 'Model',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                        {
                                            field: 'Slot No',
                                            title: 'Slot No',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                        {
                                            field: 'Tool Side',
                                            title: 'Tool Side',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                        {
                                            field: 'Customer',
                                            title: 'Customer',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                        {
                                            field: 'LastUpdatedDate',
                                            title: 'LastUpdatedDate',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        }
                                        ]
                                    });

                                    $('#tb').bootstrapTable('hideColumn', 'ToolID');
                                    $('#tb').bootstrapTable('hideColumn', 'ToolTypeName');
                                    $('#tb').bootstrapTable('hideColumn', 'Status');
                                    $('#tb').bootstrapTable('hideColumn', 'Customer');

                                    
                                } //end initable


                                function queryParams(params) {
                                    var param = {
                                        ID: _ID,
                                        Tpye: _Tpye,
                                        limit: this.limit, // 页面大小
                                        offset: this.offset, // 页码
                                        pageindex: this.pageNumber,
                                        pageSize: this.pageSize
                                    }
                                    return param;
                                }

    </script>
</head>
<body>

    <form id="form1" runat="server" class="form-horizontal" role="form">  
  
      <div class="form-group">
          <div class="col-sm-12">
               <table id="tb"></table>  
          </div>
      </div>
    </form>

</body>
</html>
