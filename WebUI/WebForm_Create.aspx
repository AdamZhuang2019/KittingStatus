<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm_Create.aspx.cs" Inherits="kittingStatus.jabil.web.WebUI.WebForm_Create" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1"  runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
 
    <link rel="shortcut icon" href="../images/PageLog.ico" type="image/x-icon"/>
    <title>ME</title>
    <link href="../Style/global.css" rel="stylesheet" type="text/css" />
    <link href="../Style/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="../Select2/css/select2.css" rel="stylesheet" type="text/css" />
    <link href="../bootstrap-datetimepicker/bootstrap-datetimepicker.min.css" rel="stylesheet"  type="text/css" />

    <script src="../Scripts/jquery-1.10.2.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Select2/js/select2.full.js" type="text/javascript"></script>
    <script src="../bootstrap-datetimepicker/bootstrap-datetimepicker.min.js" type="text/javascript"></script>

    <script>

     var YorN=eval(<%=YorN%>); 
     var Workcell_list=eval(<%=Workcell_list%>);        

    
        $(document).ready(function () {
            //是否共享炉温检测
            $("#Share_ProfileBoard").select2({
                width: "400",
                placeholder: "Select",
                tags: false,
                //allowClear: true,
                multiple: "multiple",
                maximumSelectionLength: 1,  //最多能够选择的个数
                minimumResultsForSearch: Infinity,
                data: YorN
            })


            $("#Share_DEKPallet").select2({
                width: "400",
                placeholder: "Select",
                tags: false,
                //allowClear: true,
                multiple: "multiple",
                maximumSelectionLength: 1,  //最多能够选择的个数
                minimumResultsForSearch: Infinity,
                data: YorN
            })


            $("#ShareSqueegee").select2({
                width: "400",
                placeholder: "Select",
                tags: false,
                //allowClear: true,
                multiple: "multiple",
                maximumSelectionLength: 1,  //最多能够选择的个数
                minimumResultsForSearch: Infinity,
                data: YorN
            });


            $("#Generatebtn").click(function () {
                $('#ApproveModal').modal();
            });


            var dd = new Date();
            dd.setMinutes(dd.getMinutes() + 15);

            $('#ExpectedTime').datetimepicker({
                format: 'yyyy-mm-dd hh:ii',
                //language:  'fr',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                minView: 0,
                minuteStep: 5,
                forceParse: 0,
                startDate: dd
            });

            //填充workcell 下拉选择器
            $("#WorkCell").select2({
                width: "400",
                placeholder: "Select",
                tags: false,
                //allowClear: true,
                multiple: "multiple",
                maximumSelectionLength: 1,  //最多能够选择的个数
                minimumResultsForSearch: Infinity,
                data: Workcell_list
            });

            //workcell 选中事件
            $("#WorkCell").on("change", function (e) {
                var _workcell = $("#WorkCell option:selected").text();
                if (_workcell.length > 0)
                {
                    $.ajax({
                        url: "../Data/GetBAYlbyWorkcell.ashx?workcell=" + _workcell,
                        contentType: 'application/json; charset=utf-8',
                        timeout: 5000,
                        //dataType: 'json',                                                                                                
                        datatype: "html",
                        success: function (data)
                        {

                            var BayName_list = eval(data);
                            $("#BayName").html('');
                            $("#BayName").select2({
                                width: "400",
                                placeholder: "Select",
                                tags: false,
                                //allowClear: true,
                                multiple: "multiple",
                                maximumSelectionLength: 1,  //最多能够选择的个数
                                minimumResultsForSearch: Infinity,
                                data: BayName_list
                            });

                            // Bay选中事件
                            $("#BayName").on("change", function (e) {
                                var _workcell = $("#WorkCell option:selected").text();
                                var _bayName = $("#BayName option:selected").text();
                                $.ajax({
                                    url: "../Data/GetModelbyWorkcell.ashx?workcell=" + _workcell + "&bayName=" + _bayName,
                                    contentType: 'application/json; charset=utf-8',
                                    timeout: 5000,
                                    //dataType: 'json',                                                                                                
                                    datatype: "html",
                                    success: function (data) {
                                        var Model_list = eval(data);
                                        $("#Model").html('');
                                        $("#Model").select2({
                                            width: "400",
                                            placeholder: "Select",
                                            tags: false,
                                            //allowClear: true,
                                            multiple: "multiple",
                                            maximumSelectionLength: 1,  //最多能够选择的个数
                                            minimumResultsForSearch: Infinity,
                                            data: Model_list
                                        });

                                    },
                                    error: function (msg) {
                                        alert("error:" + msg);
                                    }
                                }); //end ajax  
                            });

                        },
                        error: function (msg) {
                            alert("error:" + msg);
                        }
                    }); //end ajax   

                }

            });
        });

    function SaveData()
    {
     
     var _Workcell=$("#WorkCell option:selected").text();
     var _BayName=$("#BayName option:selected").text();
     var _Model=$("#Model option:selected").text();

     var _Share_ProfileBoard = $("#Share_ProfileBoard option:selected").text();
     var _Share_DEKPallet=$("#Share_DEKPallet option:selected").text();
     var _ShareSqueegee=$("#ShareSqueegee option:selected").text();
     
      var _ExpectedTime=$("#ExpectedTimeValue").val();
     if(_Workcell=="")
     {
         alert("请填写Workcell");
        return ;
     }
     if(_BayName=="")
     {
         alert("请填写BayName");
        return ;
     }
     if(_Model=="")
     {
         alert("请填写Model");
        return ;
     }

     if (_Share_ProfileBoard == "") {
         alert("请填写ProfileBoard");
         return;
     }
      if(_ExpectedTime=="")
     {
         alert("请填写ExpectedTim");
        return ;
     }



      $.post("../Data/SaveTask.ashx", {
          "Workcell": _Workcell,
          "BayName": _BayName,
          "Model": _Model,        
          "ExpectedTime": _ExpectedTime,
          "Share_ProfileBoard": _Share_ProfileBoard,
          "Share_DEKPallet": _Share_DEKPallet,
          "ShareSqueegee": _ShareSqueegee
      },
                 function(data)
                 {                      
                 });


        
      $.ajax({
          url: "../Data/SaveTask.ashx",
          contentType: 'application/json; charset=utf-8',
          timeout: 5000,
          type: 'POST',
          data: {
              "Workcell": _Workcell,
              "BayName": _BayName,
              "Model": _Model,
              "ExpectedTime": _ExpectedTime,
              "Share_ProfileBoard": _Share_ProfileBoard,
              "Share_DEKPallet": _Share_DEKPallet,
              "ShareSqueegee": _ShareSqueegee
          }
      });
        window.location.reload();
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
            </a><span id="appName" style=" color:Blue"><font>SMT Material and Tools Pull System <font size="3">v1.0</font></font></span>
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


                  <div>
                      <ul class="nav navbar-nav">
                            <li  ><a href="WebForm_TaskReport.aspx">Report</a></li>   
                            <li  class="active"><a href="WebForm_Create.aspx">Create Request</a></li>          
                            <li class="dropdown">
                                <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                                    Tool&Seting <b class="caret"></b>
                                </a>
                                <ul class="dropdown-menu">                   
                                    <li><a href="WebForm_ByModle.aspx">Report</a></li>
                                    <li><a href="WebForm_Log.aspx">Log Report</a></li>
                                </ul>
                            </li>
                      </ul>
                  </div>


                  </div>
             </div>
        </div>
       <%-- 菜单 End--%>
    </div>

    <div class="row">
       <div class="center-block" style="width:650px;background-color:#a8ce76; padding:25px; box-shadow: 3px 3px 3px;  ">
           <table>
               <tr style="height: 50px;">
                   <td style=" text-align:right; width:150px;">
                       WorkCell:
                   </td>
                   <td>
                       <select id="WorkCell" style="width: 400px;">
                       </select>
                   </td>
               </tr>
               <tr style="height: 50px;">
                   <td style=" text-align:right">
                       BayName:
                   </td>
                   <td>
                       <select id="BayName" style="width: 400px;">
                       </select>
                   </td>
               </tr>


               <tr style="height: 50px;">
                   <td style=" text-align:right">
                    To  Model:
                   </td>
                   <td>
                       <select id="Model" style="width: 400px;">
                       </select>
                   </td>
               </tr>

               <tr style="height: 50px;">
                   <td style=" text-align:right">
                       Expected Time:
                   </td>
                   <td>
                       <div id="ExpectedTime" name="ExpectedTime" class="input-group date form_date" data-date=""
                           data-date-format="dd MM yyyy" data-link-field="dtp_input2" data-link-format="yyyy-mm-dd"
                           style="width: 400px;">
                          
                          
                           <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span>
                           </span>
                            <input id="ExpectedTimeValue" class="form-control" size="16" type="text" value=""/>
                       </div>
                       <input type="hidden" id="dtp_input2" value="" /><br />
                   </td>
               </tr>
               
               
               <tr style="height: 50px;">
                   <td style=" text-align:right">
                     Share Profile Board:
                   </td>
                   <td>
                       <select id="Share_ProfileBoard" style="width: 400px;">
                       </select>
                   </td>
               </tr>

               <tr style="height: 50px;">
                   <td style=" text-align:right">
                     Share DEK Pallet:
                   </td>
                   <td>
                       <select id="Share_DEKPallet" style="width: 400px;">
                       </select>
                   </td>
               </tr>


               <tr style="height: 50px;">
                   <td style=" text-align:right">
                    Share Squeegee:
                   </td>
                   <td>
                       <select id="ShareSqueegee" style="width: 400px;">
                       </select>
                   </td>
               </tr>

                

               <tr style="height: 50px;">
                   <td>
                    </td>
                   <td>
                     <input type="button" class="btn btn-success" id="Generatebtn" value="Generate"  />         
                   </td>
                   </tr>
           </table>           
       </div>
   </div>  

<div class="modal fade" id="ApproveModal">  
  <div class="modal-dialog">  
    <div class="modal-content message_align">  
      <div class="modal-header">  
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>  
        <h4 class="modal-title">保存确认对话框</h4>  
      </div>  
      <div class="modal-body">         
       <div>
         是否要保存?          
      </div>
      </div>
      <div class="modal-footer">  
         <input type="hidden" id="Hidden5"/>  
         <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>  
         <a  onclick="SaveData()" class="btn btn-success" data-dismiss="modal">确定</a>  
      </div>  
    </div>
  </div>
</div>

</body>
</html>
