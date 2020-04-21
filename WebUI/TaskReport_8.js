function check(date) {
    return (new Date(date).getDate() == date.substring(date.length - 2));
}

function strDateTime(str) {
    var reg = /^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2})$/;
    var r = str.match(reg);
    if (r == null) return false;
    var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6]);
    var aa = (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6]);
    if (aa == false) {
        return false;
    }   
    if (d <= new Date()) {      
        return false;
    }
    return true;
}

function initTable() {
                 
var url = "../Data/GetTaskReport.ashx";
$('#tb').bootstrapTable({
    method: 'get',
    dataType: 'json',
    cache: false,
    striped: true,                              //是否显示行间隔色
    url: url,
    searchAlign: 'left',
    showRefresh: true,
    search: true,      //是否显示表格搜索，此搜索是客户端搜索，不会进服务端                     
    // showColumns: true,
    pagination: true,
    minimumCountColumns: 2,
    pageNumber: 1,                       //初始化加载第一页，默认第一页
    pageSize: 25,
    pageList: [25, 50, 100, 500],
    uniqueId: "ID",                     //每一行的唯一标识，一般为主键列
    showExport: false,
    clickToSelect: true,
    detailView: false,   //父子表 true
    exportDataType: 'all',
    columns: [
                    {
                        field: 'ID',
                        title: 'ID',
                        align: 'center',
                        valign: 'middle',
                        width: '1%',
                        sortable: true,
                        width:40
                    },
                    {
                        field: 'Workcell',
                        title: 'Workcell',
                        align: 'center',
                        valign: 'middle',
                        sortable: true
                    },
                    {
                        field: 'BayName',
                        title: 'BayName',
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
                        field: 'Tool Side',
                        title: 'Tool Side',
                        align: 'center',
                        valign: 'middle',
                        sortable: true
                    },
                    {
                        field: 'CreatedTime',
                        title: 'CreatedTime',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            var date = new Date(value);
                            var seperator1 = "-";
                            var seperator2 = ":";
                            var month = date.getMonth() + 1;
                            var strDate = date.getDate();
                            if (month >= 1 && month <= 9) {
                                month = "0" + month;
                            }
                            if (strDate >= 0 && strDate <= 9) {
                                strDate = "0" + strDate;
                            }
                            var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                                + " " + date.getHours() + seperator2 + date.getMinutes();

                            return currentdate;

                        }
                    },
                    {
                        field: 'ExpectedTime',
                        title: 'ExpectedTime',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            var date = new Date(value);
                            var seperator1 = "-";
                            var seperator2 = ":";
                            var month = date.getMonth() + 1;
                            var strDate = date.getDate();
                            if (month >= 1 && month <= 9) {
                                month = "0" + month;
                            }
                            if (strDate >= 0 && strDate <= 9) {
                                strDate = "0" + strDate;
                            }
                            var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                                + " " + date.getHours() + seperator2 + date.getMinutes();
                            return currentdate;
                        },
                        editable: {
                            type: 'text',
                            title: 'ExpectedTime',
                            validate: function (v) {

                                if (strDateTime(v) == false) {
                                    return '无效日期时间(日期格式错误或小于了当前时间)';
                                }
                                if (!v) return 'ExpectedTime 不能为空';
                            }
                        }
                    },
                    {
                        field: 'Feeder',
                        title: 'Feeder',
                        align: 'center',
                        valign: 'middle',
                        formatter: Formatter_Feeder,
                        sortable: true
                    },
                    {
                        field: 'FeederCar',
                        title: 'FeederCar',
                        align: 'center',
                        valign: 'middle',
                        formatter: Formatter_FeederCar,
                        sortable: true
                    },

                    {
                        field: 'Stencil',
                        title: 'Stencil',
                        align: 'center',
                        valign: 'middle',
                        formatter: Formatter_Stencil,
                        sortable: true
                    },
                   
                    {
                        field: 'Profile Board',
                        title: 'Profile Board',
                        align: 'center',
                        valign: 'middle',
                        formatter: Formatter_ProfileBoard,
                        sortable: true
                    },
                     {
                         field: 'DEK_Pallet',
                         title: 'DEK_Pallet',
                         align: 'center',
                         valign: 'middle',
                         formatter: Formatter_DEK_Pallet,
                         sortable: true
                     },
                        {
                            field: 'Squeegee',
                            title: 'Squeegee',
                            align: 'center',
                            valign: 'middle',
                            formatter: Formatter_Squeegee,
                            sortable: true
                        },
                    {
                        field: 'StencilCount',
                        title: 'StencilCount',
                        align: 'center',
                        valign: 'middle',
                        sortable: true
                    },
                    {
                        field: 'DEK_PalletCount',
                        title: 'DEK_PalletCount',
                        align: 'center',
                        valign: 'middle',
                        sortable: true
                    },
                    {
                        field: 'Profile BoardCount',
                        title: 'Profile BoardCount',
                        align: 'center',
                        valign: 'middle',
                        sortable: true
                    },
                        {
                            field: 'SqueegeeCount',
                            title: 'SqueegeeCount',
                            align: 'center',
                            valign: 'middle',
                            sortable: true
                        },
                    {
                        field: 'Status',
                        title: 'Status',
                        align: 'center',
                        valign: 'middle',
                        formatter: Formatter_Status,
                        sortable: true
                    },
                    {
                        field: 'Action',
                        title: 'Action',
                        align: 'center',
                        valign: 'middle',
                        formatter: Formatter_Action,                   
                        sortable: true
                    }
                    ],

    onDblClickCell: function (field, value, row, $element) {
        if ("Action" == field)
        {

            if (row["Status"] == "2" && value=="") {
                value_action = value, row_action = row; cell_Action = $element;
                $('#ActionText').val(value_action);
                $('#EditAction').modal('show');
            }
        }
        return true;
    },
    onEditableSave: function (field, row, oldValue, $el) {
        var idIndex = row['ID'];
        var fieldIndex = row[field];
        if (row['Status'] == "0") {
            $.ajax({
                url: '../Data/UpdateExpectedTime.ashx?ID=' + idIndex + '&name=' + field + '&value=' + fieldIndex,
                type: 'POST'
            });
        }


    },

    onExpandRow: function (index, row, $Subdetail) {
        InitSubTable(index, row, $Subdetail);
        // alert('test');
    }
});

    //$('#tb').bootstrapTable('hideColumn', 'ID');
    $('#tb').bootstrapTable('hideColumn', 'Enble');
   // $('#tb').bootstrapTable('hideColumn', 'DEK_Pallet');
   // $('#tb').bootstrapTable('hideColumn', 'Squeegee');
    $('#tb').bootstrapTable('hideColumn', 'StencilCount');
    $('#tb').bootstrapTable('hideColumn', 'DEK_PalletCount');
    $('#tb').bootstrapTable('hideColumn', 'Profile BoardCount');
    $('#tb').bootstrapTable('hideColumn', 'SqueegeeCount');
    $('#tb').bootstrapTable('hideColumn', 'Tool Side');

} //end initable


InitSubTable = function (index, row, $detail) {
    var parentid = row.MENU_ID;
    var cur_table = $detail.html('<table></table>').find('table');
    $(cur_table).bootstrapTable({
        url: '../Data/GetData_ByModle.ashx',
        method: 'get',      
        clickToSelect: true,
        detailView: true, //父子表
        uniqueId: "MENU_ID",
        pageSize: 10,
        pageList: [10, 25],
        columns: [{
            checkbox: true
        }, {
            field: 'MENU_NAME',
            title: '菜单名称'
        }, {
            field: 'MENU_URL',
            title: '菜单URL'
        }, {
            field: 'PARENT_ID',
            title: '父级菜单'
        }, {
            field: 'MENU_LEVEL',
            title: '菜单级别'
        }, ]       
    });
};
//sub
var value_action, row_action, cell_Action;

function UpdataData() {
    debugger;
    var taskid = $('#StencilTaskID').val();
    var Stencil = $('#StencilSlotsNo').val();
    if (Stencil == "") {
        alert("please select Stencil Slots first")
        return;
    }

    $.ajax({
        data: { ServiceKey: 'UpdataStencil', TaskID: taskid, Stencil: Stencil },
            url: '../Data/KittingServices.ashx',
            type: 'POST',
            success: function (data) {
                var result = JSON.parse(data);
                alert(result.msg);
                $('#tb').bootstrapTable('refresh');
            }
        });
      
    
}


function UpdataFeederCard() {

    var taskid=$('#FeederCar_TaskID').val();
    var feederCard = $('#FeederCar').val();
    if (feederCard == "")
    {
        alert("please scan feeder car first")
        return;
    }
    $.ajax(
        {
            data: { ServiceKey: 'UpdataFeederCard', TaskID: taskid, FeederCar: feederCard },
            url: '../Data/KittingServices.ashx',
            type: 'POST',
            success: function (data) {
                var result = JSON.parse(data);
                alert(result.msg);
                $('#tb').bootstrapTable('refresh');
            }
        });
}




//显示编辑框
function ShowEditAtion(row) {
    if (row)
    {
        $('#StencilTaskID').val(row);
        $('#EditAction').modal('show');
    }
}

//显示扫描框value_action = value, row_action = row; cell_Action = $element;
function ShowScanAtion(row) {
    if (row)
    {
        $('#FeederCar_TaskID').val(row);
        $('#ScanAction').modal('show');
    }
}

function Formatter_Action(value, row, index)
{
    return '<a href="#" onclick="ShowScanAtion(' + row["ID"] + ');">' + 'Scan' + '</a> <a href="#"  onclick="ShowEditAtion(' + row["ID"] + ');">' + 'Edit' + '</a>';
}


function Formatter_Squeegee(value, row, index) {

    if (row['SqueegeeCount'] == '0') {
        return '';
    }
    if (value == 'OK') {
        return value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['SqueegeeCount'] + '</span>';
    }
    return '<a href="#"  onclick="ShowGrid_Squeegee(' + row['ID'] + ');">' + value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['SqueegeeCount'] + '</span></a>';
}

function Formatter_DEK_Pallet(value, row, index) {

    if (row['DEK_PalletCount'] == '0') {
        return '';
    }
    if (value == 'OK') {
        return value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['DEK_PalletCount'] + '</span>';
  
    }
    return '<a href="#"  onclick="ShowGrid_DEK_Pallet(' + row['ID'] + ');">' + value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['DEK_PalletCount'] + '</span></a>';
}

function Formatter_Stencil(value, row, index) {

    if (row['StencilCount'] == '0') {
        return '';
    }
    if (value == 'OK') {
        return value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['StencilCount'] + '</span>';
  
    }
    return '<a href="#"  onclick="ShowGrid_Stencil(' + row['ID'] + ');">' + value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['StencilCount'] + '</span></a>';
                                           }

function Formatter_ProfileBoard(value, row, index) {

    if (row['Profile BoardCount'] == '0') {
        return '';
    }
    if (value == 'OK') {
        return value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['Profile BoardCount'] + '</span>';
    }
    return '<a href="#"  onclick="ShowGrid_ProfileBoard(' + row['ID'] + ');">' + value + '<span class="badge pull-right" style="background-color:#6faf6f;">' + row['Profile BoardCount'] + '</span></a>';
   
                                               // return '<a href="#">' + value + '<span class="badge pull-right" style="background-color:#55a9f1;">' + row['Profile BoardCount'] + '</span></a>';
}
function Formatter_FeederCar(value, row, index) {

    if (row['FeederCar'] == '') {
        return '';
    }

    return   'OK';
}

function Formatter_Feeder(value, row, index) {

    if (row['Feeder'] == '') {
        return '';
    }

    return 'OK';
}

// 3 警告，2：过期 1：完成 注意跟数据库的status 状态有点不一致，多了个3的状态对应警告
//当即将生产前15分钟内还未配料的任务状态为红色，15分钟以前的状态为黄色
function Formatter_Status(value, row, index) {
                                                    if (value == "0") {
                                                        return '<i class="fa fa-lightbulb-o fa-2x"></i>';
                                                    }
                                                    if (value == "1") {
                                                        return '<i class="fa fa-lightbulb-o fa-2x" style="color:Green;"></i>';
                                                    }
                                                    if (value == "2") {
                                                        return '<div style="color:Red;"> <i class="fa fa-lightbulb-o fa-2x"></i></div>';
                                                    }
                                                    if (value == "3") {
                                                        return '<div style="color:yellow;"> <i class="fa fa-lightbulb-o fa-2x"></i></div>';
                                                    }
                                                }


                                               

