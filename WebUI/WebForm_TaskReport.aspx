<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm_TaskReport.aspx.cs" Inherits="kittingStatus.jabil.web.WebUI.WebForm_TaskReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link rel="shortcut icon" href="../images/PageLog.ico" type="image/x-icon">
    <title>KittingReport</title>
    <link href="../Style/global.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-table.css" rel="stylesheet" type="text/css" />
    <link href="../bootstrap-datetimepicker/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-editable.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap-table-group-by.css" rel="stylesheet" type="text/css" />

    <link href="../Style/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
         .editable-click, a.editable-click, a.editable-click:hover {
            text-decoration: none;
            border-bottom: 0;
        }

            a.editable-click:hover {
                text-decoration: none;
                border-bottom: dashed 1px #0088cc;
            }
    </style>
  

    <script src="../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap.min.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-table.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-table-export.js" type="text/javascript"></script>
    <script src="../Scripts/tableExport.js" type="text/javascript"></script>
     <script src="../bootstrap-datetimepicker/bootstrap-datetimepicker.min.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-editable.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-table-group-by.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-table-editable.js" type="text/javascript"></script>


    <!--加载功能JS-->
    <script src="TaskReport_8.js" type="text/javascript"></script>
    <script type="text/javascript">

        var int_clock = -1;
        $(document).ready(function () {
            initTable();
            int_clock = self.setInterval("RefreshGetdata()", 60000);

        })

        function ShowGrid_DEK_Pallet(id) {
            var frameSrc = "WebForm_ShowGrid.aspx?ID=" + id + "&Tpye=DEK_Pallet";
            $("#iframe_DEK_Pallet").attr("src", frameSrc);
            $('#GridModal_DEK_Pallet').modal();
         }

         function ShowGrid_Squeegee(id) {
             var frameSrc = "WebForm_ShowGrid.aspx?ID=" + id + "&Tpye=Squeegee";
             $("#iframe_Squeegee").attr("src", frameSrc);
             $('#GridModal_Squeegee').modal();
         }

         function ShowGrid_Stencil(id) {
             var frameSrc = "WebForm_ShowGrid.aspx?ID=" + id + "&Tpye=Stencil";
             $("#iframe_Stencil").attr("src", frameSrc);
             $('#GridModal_Stencil').modal();
         }

         function ShowGrid_ProfileBoard(id) {
             var frameSrc = "WebForm_ShowGrid.aspx?ID=" + id + "&Tpye=ProfileBoard";
             $("#iframe_ProfileBoard").attr("src", frameSrc);
             $('#GridModal_ProfileBoard').modal();
         }
         function RefreshGetdata() {
             $('#tb').bootstrapTable('refresh');
         }
    </script>
</head>
<body>
    <div id="Header">
        <hr id="topHR" />
        <div id="logoDiv">
            <a href="#" style="cursor: pointer; float: left">
                <img src="../images/jabil_log.jpg" style="width: 179px; height: 48px; border: none;"
                    alt="JABIL" />
            </a><span id="appName"><font color="Blue">SMT Material and Tools Pull System</font> <font size="3">v1.0</font></span>
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
                            <li class="active" ><a href="WebForm_TaskReport.aspx">Report</a></li>   
                            <li ><a href="WebForm_Create.aspx">Create Request</a></li>          
                            <li class="dropdown">
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


    <!--编辑框 start-->
   <div class="modal fade" id="EditAction">  
    <div class="modal-dialog">  
        <div class="modal-content">  
            <div class="modal-header">              
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" id="HEditAction">Update Stencil Slots Info</h4>  
            </div>  
            <div class="modal-body">  
               <div class="container-fluid">
											<form class="form-horizontal" >
												<div class="form-group ">
													<label for="Max_Times_temp" class="col-xs-4 control-label">Stencil Slots:</label>
													<div class="col-xs-8 ">
                                                         <select id="StencilSlotsNo" style="width: 300px;height: 28px;">
                                                             <option value="OK" title="OK">OK</option>
                                                             <option value="NO" title="No">NO</option>
                                                         </select>
													</div>
												</div>
                                                <input  type="hidden" id="StencilTaskID"/>
											</form>
										</div>
            </div>  
            <div class="modal-footer"> 
            
              <a  class="btn btn-info" data-dismiss="modal">取 消</a>  
                 <a  onclick="UpdataData()" class="btn btn-success" data-dismiss="modal">保 存</a>  
        </div>  
    </div>  
</div>  
</div>  
<!--编辑框 end-->
<!--扫描框 start-->
<div class="modal fade" id="ScanAction">  
    <div class="modal-dialog">  
        <div class="modal-content">  
            <div class="modal-header">              
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" id="HScanAciton">EditAction</h4>  
            </div>  
            <div class="modal-body">  
               <div class="container-fluid">
											<form class="form-horizontal">
												<div class="form-group ">
													<label for="Max_Times_temp" class="col-xs-2 control-label">FeederCarNo:</label>
													<div class="col-xs-10 ">
														<input type="text" class="form-control input-sm duiqi" id="FeederCar" placeholder="">
                                                        <span style="color: red;">请扫描配料车</span>
													</div>
                                                    <input type="hidden" id="FeederCar_TaskID" />
												</div>
											</form>
										</div>
            </div>  
            <div class="modal-footer"> 
            
              <a  class="btn btn-info" data-dismiss="modal">取 消</a>  
                 <a  onclick="UpdataFeederCard()" class="btn btn-success" data-dismiss="modal">保 存</a>  
        </div>  
    </div>  
</div>  
</div>  
    <!--扫描框 end-->
<div class="modal fade" id="GridModal_DEK_Pallet">  
    <div class="modal-dialog"  style="width:800px; ">  
        <div class="modal-content">  
            <div class="modal-header">              
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" id="H2">DEK_Pallet</h4>  
            </div>  
            <div class="modal-body">  
                <iframe id="iframe_DEK_Pallet" width="100%" style="height:300px;" frameborder="0"></iframe>  
            </div>  
           <%-- <div class="modal-footer"> 
                <button class="btn btn-default"  type="button"  data-dismiss="modal">关  闭</button>  
            </div>  --%>
        </div>  
    </div>  
</div> 



   <div class="modal fade" id="GridModal_Squeegee">  
    <div class="modal-dialog"  style="width:800px; ">  
        <div class="modal-content">  
            <div class="modal-header">              
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" id="H3">Squeegee</h4>  
            </div>  
            <div class="modal-body">  
                <iframe id="iframe_Squeegee" width="100%" style="height:300px;" frameborder="0"></iframe>  
            </div>  
           <%-- <div class="modal-footer"> 
                <button class="btn btn-default"  type="button"  data-dismiss="modal">关  闭</button>  
            </div>  --%>
        </div>  
    </div>  
</div> 



   <div class="modal fade" id="GridModal_Stencil">  
    <div class="modal-dialog"  style="width:800px; ">  
        <div class="modal-content">  
            <div class="modal-header">              
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" id="titleLabel">Stencil</h4>  
            </div>  
            <div class="modal-body">  
                <iframe id="iframe_Stencil" width="100%" style="height:300px;" frameborder="0"></iframe>  
            </div>  
           <%-- <div class="modal-footer"> 
                <button class="btn btn-default"  type="button"  data-dismiss="modal">关  闭</button>  
            </div>  --%>
        </div>  
    </div>  
</div>  


   <div class="modal fade" id="GridModal_ProfileBoard">  
    <div class="modal-dialog" style="width:800px;">  
        <div class="modal-content">  
            <div class="modal-header">              
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title" ">ProfileBoard</h4>  
            </div>  
            <div class="modal-body">  
                <iframe id="iframe_ProfileBoard" width="100%"  style="height:300px;" frameborder="0"></iframe>  
            </div>  
           <%-- <div class="modal-footer"> 
                <button class="btn btn-default"  type="button"  data-dismiss="modal">关  闭</button>  
            </div>  --%>
        </div>  
    </div>  
</div>
</body>
</html>
