<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm_ByModle.aspx.cs" Inherits="kittingStatus.jabil.web.WebUI.WebForm_ByModle" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head  runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
 
    <link rel="shortcut icon" href="../images/PageLog.ico" type="image/x-icon">
    <title>ByModle</title>
    <link href="../Style/global.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-table.css" rel="stylesheet" type="text/css" />
    <link href="../Select2/css/select2.css" rel="stylesheet" type="text/css" />

    <script src="../Scripts/jquery-1.10.2.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Scripts/bootstrap-table.js"></script>
    <script src="../Scripts/bootstrap-table-export.js"></script>
    <script src="../Scripts/tableExport.js"></script>
    <script src="../Select2/js/select2.full.js" type="text/javascript"></script>

    <script>

        var Model_list=eval(<%=Model_list%>);
        $(document).ready(function () {
                                         $("#SelectModel").select2({    
                                                                    width: "200",                                       
                                                                    placeholder: "Select", 
                                                                    tags: false,                                         
                                                                    allowClear: true,
                                                                    multiple:"multiple",
                                                                    maximumSelectionLength: 1,  //最多能够选择的个数
                                                                    minimumResultsForSearch: Infinity,
                                                                    data: Model_list
                                                                 })
         
      
 

          $("#Searchbtn").click(function () { 
                            SearchData();
                        });

          initTable();
           
        })

         function initTable() {
                  
                  var url = "../Data/GetData_ByModle.ashx";
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
                          queryParams: queryParams,
                          minimumCountColumns: 2,
                          pageNumber: 1,                       //初始化加载第一页，默认第一页
                          pageSize: 25,
                          pageList: [25, 50, 100, 500],
                          uniqueId: "ToolID",                     //每一行的唯一标识，一般为主键列
                          showExport: true,
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
                  } //end initable


                     function queryParams(params) {
                      var param = {
                      Model: $("#SelectModel option:selected").text(),                                
                      limit: this.limit, // 页面大小
                      offset: this.offset, // 页码
                      pageindex: this.pageNumber,
                      pageSize: this.pageSize
                  }
                  return param;
              }


                     function operateFormatter(value, row, index) {

                      return [
                                  '<i class="fa fa-star fa-fw" style="width:20px;color:Green;">abcd</i>',                                                  
                                   ].join('');
//                               if(row['SendMail']=='0')
//                               {
//                                 return [
//                                  '<i class="fa fa-star fa-fw" style="width:20px;color:Green;"></i>',                                                  
//                                   ].join('');
//                               }
//                               else
//                               {
//                                return [
//                                  '<i class="fa fa-star-half-o fa-fw" style="width:20px;color:Red;"></i>',                                                  
//                                   ].join('');
//                               }
//                           
                             }

                   window.operateEvents = {
                      'click .Edit': function (e, value, row, index) {
//                            $('#tempPN').html(row["P/N"]);
//                            $('#Max_Times_temp').val(row["Max_Use"]);
//                            $('#Workcell_temp').val(row["Workcell"]);
//                            $('#Model_temp').val(row["Model"]);
//                            $('#Tool_temp').val(row["Tool"]);
//                            $('#Alarm_MinTimes_temp').val(row["AlarmMin_Use"]);
//                            $('#Email_Addr_temp').val(row["AlarmSendMail"]);
//                            $('#EditModal').modal('show')                        
                      }
                  };


        function SearchData()
        {           
           $('#tb').bootstrapTable('refreshOptions', { pageNumber: 1 });          
        }
    </script>
</head>
<body>
    <div id="Header">
        <hr id="topHR" />
        <div id="logoDiv">
            <a href="#" style="cursor: pointer; float: left">
                <img src="../images/jabil_log.jpg" style="width: 179px; height: 48px; border: none"
                    alt="JABIL" />
            </a><span id="appName"><font color="Blue">ByModle</font> <font size="3">v1.0</font></span>
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
                            <li><a href="WebForm_TaskReport.aspx">Report</a></li>   
                            <li ><a href="WebForm_Create.aspx">Create Request</a></li>          
                            <li class="active" class="dropdown">
                                <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                    Tool&Seting <b class="caret"></b>
                                </a>
                                <ul class="dropdown-menu">  
                                 <li><a href="WebForm_Log.aspx">Log Report</a></li>                 
                                  <%--  <li class="active"><a href="WebForm_ByModle.aspx">HP Report</a></li>
                                    <li><a href="WebForm_HistoryReport.aspx">HistoryReport</a></li>
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
    <div>
        Select Model:<select id="SelectModel"  style="margin-left: 10px; width:150px;" >   
         <input type="button" class="btn-info btn-sm" id="Searchbtn" value="查询" style="margin-left: 25px;" />            
        </select> 
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
