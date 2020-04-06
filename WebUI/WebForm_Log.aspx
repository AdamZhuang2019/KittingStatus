<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm_Log.aspx.cs" Inherits="kittingStatus.jabil.web.WebUI.WebForm_Log" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head  runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
 
    <link rel="shortcut icon" href="../images/PageLog.ico" type="image/x-icon">
    <title>lOG</title>
    <link href="../Style/global.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-table.css" rel="stylesheet" type="text/css" />
    <link href="../Select2/css/select2.css" rel="stylesheet" type="text/css" />

    <script src="../Scripts/jquery-1.10.2.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Scripts/bootstrap-table.js"></script>
    <script src="../Scripts/bootstrap-table-export.js"></script>
    <script src="../Scripts/tableExport.js"></script>


    <script>

    
        $(document).ready(function () {                                       

          initTable();
           
        })

         function initTable() {

                    var url = "../Data/GetData_Log.ashx";
                     $('#tb').bootstrapTable({
                           method: 'get',
                          dataType: 'json',
                          cache: false,
                          striped: true,                              //是否显示行间隔色
                          url: url,  
                          searchAlign:'left',  
                          search: false,      //是否显示表格搜索，此搜索是客户端搜索，不会进服务端                     
                         // showColumns: true,
                          pagination: true,                        
                          minimumCountColumns: 2,
                          pageNumber: 1,                       //初始化加载第一页，默认第一页
                          pageSize: 25,
                          pageList: [25, 50, 100, 500],
                          uniqueId: "ID",                     //每一行的唯一标识，一般为主键列
                          showExport: true,
                          exportDataType: 'all',
                          columns: [                                      
                                        {
                                            field: 'ID',
                                            title: 'ID',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                        {
                                            field: 'TitleName',
                                            title: 'TitleName',
                                            align: 'center',
                                            valign: 'middle',                                          
                                            sortable: true
                                        },
                                        {
                                            field: 'LogTime',
                                            title: 'LogTime',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },
                                         {
                                             field: 'Log',
                                             title: 'Log',
                                            align: 'center',
                                            valign: 'middle',
                                            sortable: true
                                        },                                        
                                        {
                                            field: 'SQL',
                                            title: 'SQL',
                                             align: 'center',
                                            valign: 'middle',                                        
                                            sortable: true
                                        }
                                        ]
                     });
                  } //end initable



    </script>
</head>
<body>
    <div id="Header">
        <hr id="topHR" />
        <div id="logoDiv">
            <a href="#" style="cursor: pointer; float: left">
                <img src="../images/jabil_log.jpg" style="width: 179px; height: 48px; border: none"
                    alt="JABIL" />
            </a><span id="appName"><font color="Blue"></font> <font size="3">v1.0</font></span>
            <div style="clear: both">
            </div>
        </div>
       
        <%-- 菜单--%>
        <div class="navbar navbar-default" role="navigation">
             <div class="container-fluid">
                  <div class="navbar-header">
                     <a class="navbar-brand" href="#">Home</a>
                  </div>

                   <div>
                      <ul class="nav navbar-nav">
                            <li  ><a href="WebForm_TaskReport.aspx">Report</a></li>   
                            <li ><a href="WebForm_Create.aspx">Create Request</a></li>          
                            <li class="active" class="dropdown">
                                <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                    Tool&Seting <b class="caret"></b>
                                </a>
                                <ul class="dropdown-menu">                   
                                    <li><a href="WebForm_ByModle.aspx">Report</a></li>
                                    <li><a href="WebForm_Log.aspx">Log Report</a></li>
                                   
                                   <%-- <li><a href="WebForm_HistoryReport.aspx">HistoryReport</a></li>
                                    <li class="divider"></li>
                                    <li><a href="#">..</a></li>
                                    <li class="divider"></li>
                                    <li><a href="#">...</a></li>--%>
                                </ul>
                            </li>
                      </ul>
                  </div>

                 
             </div>
        </div>
        <%-- 菜单 End--%>

    </div>
   
    <form id="form1" runat="server" class="form-horizontal" role="form">   
      <div class="form-group">
          <div class="col-sm-12">
              <table id="tb"></table>
          </div>
      </div>
    </form>
</body>
</html>
