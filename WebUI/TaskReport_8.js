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
    //if (d <= new Date()) {
    //    return false;
    //}
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
        showRefresh: false,
        search: false,      //是否显示表格搜索，此搜索是客户端搜索，不会进服务端                     
        // showColumns: true,
        queryParams: queryParams,
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
                width: 40
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
                field: 'Tool Side',
                title: 'Tool Side',
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
                field: 'BuildPlan',
                title: 'BuildPlan',
                align: 'center',
                valign: 'middle',
                sortable: false
            },
            {
                field: 'UPH',
                title: 'UPH',
                align: 'center',
                valign: 'middle',
                sortable: false
            },

            {
                field: 'Tool Side',
                title: 'Tool Side',
                align: 'center',
                valign: 'middle',
                sortable: true
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
            },
            {
                field: 'RealExpectedTime',
                title: 'RealExpectedTime',
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
                    if (currentdate == "1900-01-01 0:0") {
                        return "empty";
                    }

                    return currentdate;
                },
                editable: {
                    type: 'text',  //编辑框的类型。支持text|textarea|select|date|checklist等
                    savenochange: true,
                    //type: "select",              //编辑框的类型。支持text|textarea|select|date|checklist等
                    //source: [{ value: 1, text: "开发部" }, { value: 2, text: "销售部" }, { value: 3, text: "行政部" }],
                    title: '格式：1900-01-01 0:0 ， 直接提交默认为当前时间',
                    disabled: false,             //是否禁用编辑
                    //emptytext: "空文本",          //空值的默认文本
                    mode: "popup",              //编辑框的模式：支持popup和inline两种模式，默认是popup
                    validate: function (v) { }
                    // 填写了这个如果不实现display 则会影响到显示值为empty
                    //display: function (v, r) {
                    //    $(this).html(v);
                    //},

                }
            },

            {
                field: 'ShiftStartTime',
                title: 'ShiftStart',
                align: 'center',
                valign: 'middle',
                sortable: true
                
            },

            {
                field: 'shiftEndTime',
                title: 'shiftEnd',
                align: 'center',
                valign: 'middle',
                sortable: true
                
            },

            {
                field: 'ShiftDescription',
                title: 'Shift',
                align: 'center',
                valign: 'middle',
                sortable: true
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
                // formatter: Formatter_FeederCar,
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

            //{
            //    field: 'Profile Board',
            //    title: 'Profile Board',
            //    align: 'center',
            //    valign: 'middle',
            //    formatter: Formatter_ProfileBoard,
            //    sortable: true
            //},
            // {
            //     field: 'DEK_Pallet',
            //     title: 'DEK_Pallet',
            //     align: 'center',
            //     valign: 'middle',
            //     formatter: Formatter_DEK_Pallet,
            //     sortable: true
            // },
            //    {
            //        field: 'Squeegee',
            //        title: 'Squeegee',
            //        align: 'center',
            //        valign: 'middle',
            //        formatter: Formatter_Squeegee,
            //        sortable: true
            //    },
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
            if ("Action" == field) {

                if (row["Status"] == "2" && value == "") {
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
            if (field == "RealExpectedTime")
            {
                if (fieldIndex != undefined && fieldIndex == "empty")
                {
                    //
                    fieldIndex = new Date().Format("yyyy-MM-dd HH:mm:ss");
                }
            }
            
          //  if (row['Status'] == "0") {
                $.ajax({
                    url: '../Data/UpdateExpectedTime.ashx?ID=' + idIndex + '&name=' + field + '&value=' + fieldIndex,
                    type: 'POST',
                    success: function (data) {
                        var result = JSON.parse(data);
                        alert(result.msg);
                        if (result.Status != 0)
                        {
                            $('#tb').bootstrapTable('refresh');
                        }
                    }
                });
        //    }


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
    //$('#tb').bootstrapTable('hideColumn', 'Tool Side');

    $('#tb').bootstrapTable('hideColumn', 'shiftEndTime');
    $('#tb').bootstrapTable('hideColumn', 'ShiftStartTime');
    $('#tb').bootstrapTable('hideColumn', 'ShiftDescription');



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
        },]
    });
};
//sub
var value_action, row_action, cell_Action;


function queryParams(params) {
    var param = {
        workcell: $("#SelectWorkCell option:selected").text(),
        bay: $("#SelectBay option:selected").text(),
        keywork: $("#KeyWork").val(),
        limit: this.limit, // 页面大小
        offset: this.offset, // 页码
        pageindex: this.pageNumber,
        pageSize: this.pageSize
    }
    return param;
}

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
        type: 'POST'
        
    });


}


function UpdataFeederCard() {

    var taskid = $('#FeederCar_TaskID').val();
    var feederCard = $('#FeederCar').val();
    if (feederCard == "") {
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
    if (row) {
        $('#StencilTaskID').val(row);
        $('#EditAction').modal('show');
    }
}

//显示扫描框value_action = value, row_action = row; cell_Action = $element;
function ShowScanAtion(taskID, feederCar) {
    if (taskID) {
        $('#FeederCar_TaskID').val(taskID);
        $('#FeederCar').val(feederCar);
        $('#ScanAction').modal('show');

        $('#ScanAction').on('shown.bs.modal', function () {
            // 执行一些动作...
            setFocus('FeederCar');
        })
    }
}

function setFocus(id) {
    var t = $("#" + id).val();
    $("#" + id).val("").focus().val(t);
}


function Formatter_Action(value, row, index) {
    return '<a href="#" onclick="ShowScanAtion(\'' + row["ID"] + '\',\'' + row["FeederCar"] + '\');">' + 'Scan' + '</a> <a href="#"  onclick="ShowEditAtion(\'' + row["ID"] + '\');">' + 'Edit' + '</a>';
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

    return 'OK';
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

// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}


