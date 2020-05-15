using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data;
using kittingStatus.jabil.web.DAL;
using System.Text;
using kittingStatus.jabil.web.BLL;

namespace kittingStatus.jabil.web
{
    public class Global : System.Web.HttpApplication
    {
        int _synchro = 1;
      

        protected void Application_Start(object sender, EventArgs e)
        {     
            //更新任务状态     
            System.Timers.Timer timer_RunProcess = new System.Timers.Timer(1000 * 30); //1800000
            timer_RunProcess.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed);
            timer_RunProcess.AutoReset = true;
            timer_RunProcess.Enabled = true;


            //更新钢网数据
            System.Timers.Timer timer_RunProcess_Stencil = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_Stencil.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_Stencil);
            timer_RunProcess_Stencil.AutoReset = true;
            timer_RunProcess_Stencil.Enabled = true;

            //更新炉温板数据
            System.Timers.Timer timer_RunProcess_ProfileBoard = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_ProfileBoard.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_ProfileBoard);
            timer_RunProcess_ProfileBoard.AutoReset = true;
            timer_RunProcess_ProfileBoard.Enabled = true;

          
            //更新托盘数据
            System.Timers.Timer timer_RunProcess_DEK_Pallet = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_DEK_Pallet.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_DEK_Pallet);
            timer_RunProcess_DEK_Pallet.AutoReset = true;
            timer_RunProcess_DEK_Pallet.Enabled = true;

            //更新刮刀数据
            System.Timers.Timer timer_RunProcess_Squeegee = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_Squeegee.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_Squeegee);
            timer_RunProcess_Squeegee.AutoReset = true;
            timer_RunProcess_Squeegee.Enabled = true;

            //
            System.Timers.Timer timer_RunProcess_FreeTask = new System.Timers.Timer(_synchro * 1000 * 60); //一分钟运行一次
            timer_RunProcess_FreeTask.Elapsed += Timer_RunProcess_FreeTask_Elapsed; ;
            timer_RunProcess_FreeTask.AutoReset = true;
            timer_RunProcess_FreeTask.Enabled = true;



        }
        /// <summary>
        /// 释放任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_RunProcess_FreeTask_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TaskBll.FreeTask();
        }

        #region 定时器事件执行
        /// <summary>
        /// 钢网数据抓取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_RunProcess_Elapsed_Stencil(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {

                // 在提醒时间内 是否还有任务处于未配料状态 且钢网数量为0
                System.Data.DataTable dt_task = new System.Data.DataTable();
                string dataupdatesql = "select [ID],[Stencil],[Profile Board],[Model],[Tool Side],[From_Tool Side],[From_Model] from [EKS_T_Task] where Enble=1 and [StencilCount]=0 and  DateAdd(N, -1*[RemindPreTime], [ExpectedTime])<=getdate() and [Status]=0  and  [ExpectedTime]>=getdate()";
                dt_task = new DAL.DbHelper().QueryDataTable(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }

                // 获取所有的钢网型号
                System.Data.DataTable dt_Model = new System.Data.DataTable();
                string dt_Modelsql = "select distinct [Model] from [EKS_T_Task] where Enble=1 and [StencilCount]=0 and DateAdd(N, -1*[RemindPreTime], [ExpectedTime])<=getdate() and [Status]=0  and  [ExpectedTime]>=getdate()";
                dt_Model = new DAL.DbHelper().QueryDataTable(dt_Modelsql);
                if (dt_Model.Rows.Count == 0)
                {
                    return;
                }

                AddLog("当前分析(Stencil)的任务", "任务个数：" + dt_task.Rows.Count.ToString(), dataupdatesql);




                DataTable DT_All = new DataTable();

                #region For
                for (int i = 0; i < dt_Model.Rows.Count; i++)
                {
                    // 找到型号相匹配的钢网，并且通过钢网的工具ID 把相关的几个扩展属性都检索出来（客户ID，库位ID，板面类型（B/T面）,modelID）
                    string _Model = Convert.ToString(dt_Model.Rows[i][0]);
                    string cmdText_Part = @"select  M.ToolID,P.ProcessFieldName, M.ProcessFieldValue from dbo.T_Tools_Attribute " +
                                            "as M inner join dbo.T_ProcessField as P on M.ProcessFieldID=P.ProcessFieldID " +
                                            "where  ToolID in (select distinct ToolID  from  " +
                                            "dbo.T_Tools_Attribute where ProcessFieldValue like '%" + _Model + "%' )  " +
                                            "and P.ProcessFieldName in ('Customer','Slot No','Model','Tool Side')"; //'Serial Number',工具-》工具类型-》工具制程-》制程相关的扩展属性-》扩展属性的值 ，所有扩展属性都为此工具的属性

                    DataTable dt_Part = SqlHelper.GetDataTableOfRecord(cmdText_Part);
                    dt_Part.TableName = "Data";

                    // 找到库存中的钢网
                    string cmdText_Main = @"select M.ToolID,T.ToolTypeName, M.ToolSN,P.ProcessDesc as 'Status',M.LastUpdatedDate  from dbo.T_Tools as M  " +  //M.CustomerID,
                                           "inner join dbo.T_Process P on M.Status=P.ProcessID  inner join dbo.T_Type T on M.ToolTypeID=T.ToolTypeID " +
                                           "where P.ProcessDesc='Storage' and  M.ToolID in (select distinct ToolID  from  " +
                                           "dbo.T_Tools_Attribute where ProcessFieldValue like '%" + _Model + "%' )";


                    DataTable dt_Main = SqlHelper.GetDataTableOfRecord(cmdText_Main);
                    // 增加6列数据
                    string[] NewField = new string[] { "Model", "Slot No", "Tool Side", "Customer", "Isuser", "IncludeModel" };
                    for (int j = 0; j < NewField.Length; j++)
                    {
                        dt_Main.Columns.Add(NewField[j]);
                    }


                    for (int k = 0; k < dt_Main.Rows.Count; k++)// 一行一行遍历
                    {
                        for (int j = 0; j < NewField.Length; j++)// 遍历新加的列
                        {

                            string fieldName = NewField[j];
                            if (fieldName == "Isuser")
                            {
                                dt_Main.Rows[k][fieldName] = 0;
                                continue;
                            }


                            var rows = dt_Part.Select("ToolID=" +
                                Convert.ToString(dt_Main.Rows[k]["ToolID"]) + " and ProcessFieldName='" + fieldName + "'");
                            if (rows.Length > 0)
                            {
                                if (fieldName == "Model")
                                {
                                    dt_Main.Rows[k][fieldName] = _Model;
                                    dt_Main.Rows[k]["IncludeModel"] = rows[0]["ProcessFieldValue"];// 取第一个值存入
                                    continue;
                                }


                                dt_Main.Rows[k][fieldName] = rows[0]["ProcessFieldValue"];// 取第一个值存入
                                continue;
                            }
                        }
                    }

                    if (DT_All == null)
                    {
                        DT_All = dt_Main.Copy();
                    }
                    else
                    {
                        DT_All.Merge(dt_Main);
                    }
                }
                #endregion

                Random ra = new Random();
                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string id = Convert.ToString(dt_task.Rows[i]["ID"]);
                    string Model = Convert.ToString(dt_task.Rows[i]["Model"]);
                  
                    string Stencil_Slot_No = "";
                    string Stencil_Models = "";
                    string _StencilCount = "0";
                    string _stencil_json = "";
                    try
                    {
                        var _rows_all = DT_All.Select("Model='" + Model + "' and  ToolTypeName='Stencil'");//查找所有的钢网

                        if (_rows_all.Length == 0)
                        {
                            continue;
                        }

                        var _rows = DT_All.Select("Isuser=0 and Model='" + Model + "' and  ToolTypeName='Stencil'"); // 查找未使用的钢网

                        if (_rows.Length > 0)
                        {
                            int num = ra.Next(0, _rows.Length - 1);//在未使用的钢网中随机取一个。
                            Stencil_Slot_No = Convert.ToString(_rows[num]["Slot No"]);
                            Stencil_Models = Convert.ToString(_rows[num]["IncludeModel"]);
                            _rows[num]["Isuser"] = 1;
                        }

                        DataTable _stencil_db = ToDataTable(_rows_all);
                        _stencil_db.TableName = "dt";
                        _stencil_json = ConvertJson.ToJson(_stencil_db);
                        _StencilCount = _rows_all.Length.ToString();
                    }
                    catch (Exception ex)
                    {
                        AddLog("异常出错", "Stencil数据分析", ex.ToString());
                    }

                    if (Stencil_Slot_No.Length > 0 && _StencilCount != "0")// 如果分配了库位
                    {
                        string sql = "UPDATE [EKS_T_Task] set [Stencil]='" + Stencil_Slot_No + "',[Stencil_Models]='" + Stencil_Models + "',[Stencil_Detail]='" + _stencil_json + "',[StencilCount]=" + _StencilCount + " where [ID]=" + id;
                        new  DAL.DbHelper().Execute(sql);
                        AddLog("保存数据", "Stencil   ID:" + id.ToString(), sql);
                    }
                }

            }
            catch (Exception ex)
            {
                AddLog("异常出错", "Stencil", ex.ToString());
            }

        }
        /// <summary>
        /// 刮刀数据抓取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_RunProcess_Elapsed_Squeegee(object sender, System.Timers.ElapsedEventArgs e)
        {
           // processForSpecial("SqueegeeCount", "Squeegee", "Squeegee_Share", "Squeegee_Detail", "Squeegee_Models", "Squeegee");
        }

        /// <summary>
        /// 托盘数据抓取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_RunProcess_Elapsed_DEK_Pallet(object sender, System.Timers.ElapsedEventArgs e)
        {

           // processForSpecial("DEK_PalletCount", "DEK_Pallet", "DEK_Pallet_Share", "DEK_Pallet_Detail", "DEK_Pallet_Models", "DEK Pallet");
           
        }
       

        
        /// <summary>
        ///  炉温板状态更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_RunProcess_Elapsed_ProfileBoard(object sender, System.Timers.ElapsedEventArgs e)
        {
           // processForSpecial("Profile BoardCount", "Profile Board", "ProfileBoard_Share", "Profile Board_Detail", "Profile Board_Models", "Profile Board");

        }    

        /// <summary>
        ///任务状态更新， 每隔半分钟扫描一次，判断钢网以及支撑台、刮刀、测温板,feedercar ,feeder 是否配足，过期的将不再处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_RunProcess_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {          
            // 每天晚上11点55分删除三天之前的日志
            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 55)
            {
                string deletesql = "delete from [Tlog] where LogTime<= DateAdd('d',-3, getdate()) ";
                int deleterow = new  DAL.DbHelper().Execute(deletesql);
            }

            try
            {
                System.Data.DataTable DataUpdate = new System.Data.DataTable();
                // status =0, 未配料
                string dataupdatesql = "select [ID],[Stencil],[Profile Board],[Squeegee],[DEK_Pallet],[FeederCar],[Feeder] from [T_Task] where Enble=1 and [Status]=0   and  [ExpectedTime]<getdate()";
                DataUpdate = new DAL.DbHelper().QueryDataTable(dataupdatesql);
                string u_Status = "2";//无料
                string u_ID = string.Empty;
                string u_Stencil = string.Empty;
                string u_ProfileBoard = string.Empty;
                string u_Squeegee = string.Empty;
                string u_DEK_Pallet = string.Empty;
                string u_FeederCar = string.Empty;
                string u_Feeder = string.Empty;

                for (int i = 0; i < DataUpdate.Rows.Count; i++)
                {
                     u_Status = "2";//无料
                     u_ID = Convert.ToString(DataUpdate.Rows[i]["ID"]);
                     u_Stencil = Convert.ToString(DataUpdate.Rows[i]["Stencil"]);
                     u_ProfileBoard = Convert.ToString(DataUpdate.Rows[i]["Profile Board"]);
                     u_Squeegee = Convert.ToString(DataUpdate.Rows[i]["Squeegee"]);
                     u_DEK_Pallet = Convert.ToString(DataUpdate.Rows[i]["DEK_Pallet"]);
                     u_FeederCar = Convert.ToString(DataUpdate.Rows[i]["FeederCar"]);
                     u_Feeder = Convert.ToString(DataUpdate.Rows[i]["Feeder"]);

                    if (!string.IsNullOrWhiteSpace(u_Stencil) && !string.IsNullOrWhiteSpace(u_ProfileBoard) && !string.IsNullOrWhiteSpace(u_Squeegee)
                        && !string.IsNullOrWhiteSpace(u_DEK_Pallet)&& !string.IsNullOrWhiteSpace(u_FeederCar) && !string.IsNullOrWhiteSpace(u_Feeder))
                    {
                        u_Status = "1";//料足
                    }
                    string u_sql = "UPDATE [T_Task] set [Status]=" + u_Status + " where [ID]=" + u_ID;
                    int temprow = new  DAL.DbHelper().Execute(u_sql);
                    AddLog("判定状态", "ID:" + u_ID, u_sql);
                }
            }
            catch(Exception ex)
            {
                AddLog("异常出错", "判定状态", ex.ToString());
            }
           
        }

        #endregion 

        /// <summary>
        ///  根据状态获取
        /// </summary>
        /// <param name="columnDeviceCount">工具数量</param>
        /// <param name="columnDevice">工具的名称</param>
        /// <param name="columnDevice_Share">工具是否共享</param>
        /// <param name="columnDevice_Detail">工具的详情</param>
        /// <param name="columnDevice_Models">工具的型号</param>
        /// <param name="ToolTypeName">工具类型</param>
        void processForSpecial(string columnDeviceCount, string columnDevice, string columnDevice_Share, string columnDevice_Detail, string columnDevice_Models, string ToolTypeName)
        {

            try
            {

                System.Data.DataTable dt_task = new System.Data.DataTable();
                // 第一步查看当前任务某一工具所分配的数量是否为0，且当前任务状态为未分配，并且在提醒时间内(提醒时间是配置在bay 信息表里面的)
                string dataupdatesql = "select * from [EKS_T_Task] where Enble=1 and [" + columnDeviceCount + "]=0 and  DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=getdate() and [Status]=0  and  [ExpectedTime]>=getdate()";
                dt_task = new DAL.DbHelper().QueryDataTable(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }

                #region 确认是否跳过
                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string _Share = Convert.ToString(dt_task.Rows[i][columnDevice_Share]);
                    if (_Share != "0")
                    {
                        string sql = "UPDATE [EKS_T_Task] set [" + columnDevice + "]='" + "OK" + "',[" + columnDevice_Detail + "]='" + "share" + "',[" + columnDeviceCount + "]=" + "1" + " where [ID]=" + Convert.ToString(dt_task.Rows[i]["ID"]);
                        new  DAL.DbHelper().Execute(sql);
                    }
                }

                #endregion
                // 执行跳过步骤后 再一次判断是否有符合条件的任务
                dt_task = new DAL.DbHelper().QueryDataTable(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }

                // 查找去重后的型号信息
                System.Data.DataTable dt_Model = new System.Data.DataTable();
                string dt_Modelsql = "select distinct [Model] from [EKS_T_Task] where Enble=1 and [" + columnDeviceCount + "]=0 and DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=getdate() and [Status]=0  and  [ExpectedTime]>=getdate()";
                dt_Model = new DAL.DbHelper().QueryDataTable(dt_Modelsql);
                if (dt_Model.Rows.Count == 0)
                {
                    return;
                }
                AddLog("当前分析(" + columnDevice + ")的任务", "任务个数：" + dt_task.Rows.Count.ToString(), dataupdatesql);




                DataTable DT_All = new DataTable();

                #region  与钢网一样的处理逻辑，首先根据钢网的型号获取到所有的工具ID，再根据工具ID取取所有的扩展属性。然后在扩展属性中查找出我们所需要的数据并存在DT_All 表中
                for (int i = 0; i < dt_Model.Rows.Count; i++)
                {
                    string _Model = Convert.ToString(dt_Model.Rows[i][0]);
                    string cmdText_Part = @"select  M.ToolID,P.ProcessFieldName, M.ProcessFieldValue from dbo.T_Tools_Attribute " +
                                            "as M inner join dbo.T_ProcessField as P on M.ProcessFieldID=P.ProcessFieldID " +
                                            "where  ToolID in (select distinct ToolID  from  " +
                                            "dbo.T_Tools_Attribute where ProcessFieldValue like '%" + _Model + "%' )  " +
                                            "and P.ProcessFieldName in ('Customer','Slot No','Model','Tool Side')"; //'Serial Number',
                    DataTable dt_Part = SqlHelper.GetDataTableOfRecord(cmdText_Part);


                    dt_Part.TableName = "Data";
                    string cmdText_Main = @"select M.ToolID,T.ToolTypeName, M.ToolSN,P.ProcessDesc as 'Status',M.LastUpdatedDate  from dbo.T_Tools as M  " +  //M.CustomerID,
                                           "inner join dbo.T_Process P on M.Status=P.ProcessID  inner join dbo.T_Type T on M.ToolTypeID=T.ToolTypeID " +
                                           "where P.ProcessDesc='Storage' and  M.ToolID in (select distinct ToolID  from  " +
                                           "dbo.T_Tools_Attribute where ProcessFieldValue like '%" + _Model + "%' )";
                    DataTable dt_Main = SqlHelper.GetDataTableOfRecord(cmdText_Main);

                    //给主表增加6列
                    string[] NewField = new string[] { "Model", "Slot No", "Tool Side", "Customer", "Isuser", "IncludeModel" };

                    for (int j = 0; j < NewField.Length; j++)
                    {
                        dt_Main.Columns.Add(NewField[j]);
                    }

                    //填充六列数据
                    for (int k = 0; k < dt_Main.Rows.Count; k++)
                    {
                        for (int j = 0; j < NewField.Length; j++)
                        {
                            //Isuser 赋值为0
                            string fieldName = NewField[j];
                            if (fieldName == "Isuser")
                            {
                                dt_Main.Rows[k][fieldName] = 0;
                                continue;
                            }

                            //从part表中查询数据
                            var rows = dt_Part.Select("ToolID=" +
                                Convert.ToString(dt_Main.Rows[k]["ToolID"]) + " and ProcessFieldName='" + fieldName + "'");

                            if (rows.Length > 0)
                            {
                                //如果为model
                                if (fieldName == "Model")
                                {
                                    dt_Main.Rows[k][fieldName] = _Model;
                                    dt_Main.Rows[k]["IncludeModel"] = rows[0]["ProcessFieldValue"];
                                    continue;
                                }


                                dt_Main.Rows[k][fieldName] = rows[0]["ProcessFieldValue"];
                                continue;
                            }
                        }
                    }

                    if (DT_All == null)
                    {
                        DT_All = dt_Main.Copy();
                    }
                    else
                    {
                        DT_All.Merge(dt_Main);
                    }
                }
                #endregion

                Random ra = new Random();
                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string id = Convert.ToString(dt_task.Rows[i]["ID"]);
                    string Model = Convert.ToString(dt_task.Rows[i]["Model"]);
                    string Slot_No = "";
                    string Device_Models = "";
                    string DeviceCount = "0";
                    string json = "";
                    try
                    {
                        var _rows_all = DT_All.Select("Model='" + Model + "' and ToolTypeName='" + ToolTypeName + "'");
                        var _rows = DT_All.Select("Isuser=0 and Model='" + Model + "' and  ToolTypeName='" + ToolTypeName + "'");
                        if (_rows.Length > 0)
                        {
                            int num = ra.Next(0, _rows.Length - 1);
                            Slot_No = Convert.ToString(_rows[num]["Slot No"]);
                            Device_Models = Convert.ToString(_rows[num]["IncludeModel"]);
                            _rows[num]["Isuser"] = 1;
                        }
                        if (_rows_all.Length == 0)
                        {
                            continue;
                        }
                        DataTable _temp_db = ToDataTable(_rows_all);
                        _temp_db.TableName = "dt";
                        json = ConvertJson.ToJson(_temp_db);
                        DeviceCount = _rows_all.Length.ToString();
                    }
                    catch (Exception ex)
                    {
                        AddLog("异常出错", columnDevice + "数据分析", ex.ToString());
                    }
                    string sql = "UPDATE [EKS_T_Task] set [" + columnDevice + "]='" + Slot_No + "',[" + columnDevice_Models + "]='" + Device_Models + "',[" + columnDevice_Detail + "]='" + json + "',[" + columnDeviceCount + "]=" + DeviceCount + " where [ID]=" + id;
                    new  DAL.DbHelper().Execute(sql);
                    AddLog("保存数据", columnDevice + "   ID:" + id.ToString(), sql);

                }

            }
            catch (Exception ex)
            {
                AddLog("异常出错", "Squeegee", ex.ToString());
            }

        }


        #region 帮助函数
        /// <summary>
        /// 根据把行数据集合转换为一个datatable
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构  
            foreach (DataRow row in rows)
                tmp.Rows.Add(row.ItemArray);  // 将DataRow添加到DataTable中  
            return tmp;
        } 
        public string StringToBinary(string str)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);
            StringBuilder strResult = new StringBuilder(data.Length * 8);

            foreach (byte b in data)
            {
                strResult.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return  strResult.ToString();           
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="TitleName"></param>
        /// <param name="Log"></param>
        /// <param name="SQL"></param>
        private void AddLog(string TitleName, string Log, string SQL)
        {
            try
            {
                string sql = "INSERT INTO [Tlog] ([TitleName], [Log],[SQL])";
                Log = StringToBinary(Log);
                SQL = StringToBinary(SQL);
                sql += string.Format("VALUES('{0}','{1}','{2}')", TitleName, Log, SQL);
                new  DAL.DbHelper().Execute(sql);
            }
            catch
            {
            }

        }
        #endregion
    }
}