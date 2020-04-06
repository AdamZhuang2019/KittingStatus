using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data;
using kittingStatus.jabil.web.DAL;
using System.Text;

namespace kittingStatus.jabil.web
{
    public class Global : System.Web.HttpApplication
    {
        int _synchro = 1;
      

        protected void Application_Start(object sender, EventArgs e)
        {          
            System.Timers.Timer timer_RunProcess = new System.Timers.Timer(1000 * 30); //1800000
            timer_RunProcess.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed);
            timer_RunProcess.AutoReset = true;
            timer_RunProcess.Enabled = true;



            System.Timers.Timer timer_RunProcess_Stencil = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_Stencil.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_Stencil);
            timer_RunProcess_Stencil.AutoReset = true;
            timer_RunProcess_Stencil.Enabled = true;


            System.Timers.Timer timer_RunProcess_ProfileBoard = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_ProfileBoard.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_ProfileBoard);
            timer_RunProcess_ProfileBoard.AutoReset = true;
            timer_RunProcess_ProfileBoard.Enabled = true;



            System.Timers.Timer timer_RunProcess_DEK_Pallet = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_DEK_Pallet.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_DEK_Pallet);
            timer_RunProcess_DEK_Pallet.AutoReset = true;
            timer_RunProcess_DEK_Pallet.Enabled = true;


            System.Timers.Timer timer_RunProcess_Squeegee = new System.Timers.Timer(_synchro * 1000 * 60); //1800000
            timer_RunProcess_Squeegee.Elapsed += new System.Timers.ElapsedEventHandler(timer_RunProcess_Elapsed_Squeegee);
            timer_RunProcess_Squeegee.AutoReset = true;
            timer_RunProcess_Squeegee.Enabled = true;


        }
        void timer_RunProcess_Elapsed_Squeegee(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {

                System.Data.DataTable dt_task = new System.Data.DataTable();
                string dataupdatesql = "select * from [T_Task] where Enble=1 and [SqueegeeCount]=0 and  DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }
                #region 确认是否跳过
                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string DEK_Pallet_Share = Convert.ToString(dt_task.Rows[i]["Squeegee_Share"]);
                    if (DEK_Pallet_Share != "0")
                    {
                        string sql = "UPDATE [T_Task] set [Squeegee]='" + "OK" + "',[Squeegee_Detail]='" + "share" + "',[SqueegeeCount]=" + "1" + " where [ID]=" + Convert.ToString(dt_task.Rows[i]["ID"]);
                        DAL.DbHelper.ExecuteSql(sql);
                    }
                }

                #endregion

                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }
                #region 确认之前是否有可以共用

                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string Model = Convert.ToString(dt_task.Rows[i]["Model"]);
                    string From_Model = Convert.ToString(dt_task.Rows[i]["From_Model"]);
                    string From_ToolSide = Convert.ToString(dt_task.Rows[i]["From_Tool Side"]);
                    string FromRecord_sql = "select top 1 * from [T_Task] where Enble=1 and [Status]>0  and  [ExpectedTime]<now() and [Model]='" + From_Model + "' and [Tool Side]='" + From_ToolSide + "' and [Squeegee_Models] like '%" + Model + "%' order by [ExpectedTime] desc";
                    DataTable recorddt = DAL.DbHelper.ExecuteSql_Table(FromRecord_sql);
                    if (recorddt.Rows.Count == 0)
                    {
                        continue;
                    }
                    string sql = "UPDATE [T_Task] set [ShareID]=" + Convert.ToString(recorddt.Rows[0]["ID"]) + ",[Squeegee]='" + "OK" + "',[Squeegee_Models]='" + Convert.ToString(recorddt.Rows[0]["Squeegee_Models"]) + "',[Squeegee_Detail]='" + "share" + "',[SqueegeeCount]=" + "1" + " where [ID]=" + Convert.ToString(dt_task.Rows[i]["ID"]);
                    DAL.DbHelper.ExecuteSql(sql);

                }


                #endregion

                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }
                System.Data.DataTable dt_Model = new System.Data.DataTable();
                string dt_Modelsql = "select distinct [Model] from [T_Task] where Enble=1 and [SqueegeeCount]=0 and DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_Model = DAL.DbHelper.ExecuteSql_Table(dt_Modelsql);
                if (dt_Model.Rows.Count == 0)
                {
                    return;
                }
                AddLog("当前分析(Squeegee)的任务", "任务个数：" + dt_task.Rows.Count.ToString(), dataupdatesql);




                DataTable DT_All = new DataTable();

                #region For
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
                    string[] NewField = new string[] { "Model", "Slot No", "Tool Side", "Customer", "Isuser", "IncludeModel" };
                    for (int j = 0; j < NewField.Length; j++)
                    {
                        dt_Main.Columns.Add(NewField[j]);
                    }
                    for (int k = 0; k < dt_Main.Rows.Count; k++)
                    {
                        for (int j = 0; j < NewField.Length; j++)
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
                    string ToolSide = Convert.ToString(dt_task.Rows[i]["Tool Side"]);
                    string Squeegee_Slot_No = "";
                    string Squeegee_Models = "";
                    string SqueegeeCount = "0";
                    string Squeegee_json = "";
                    try
                    {
                        var _rows_all = DT_All.Select("Model='" + Model + "' and ToolTypeName='Squeegee'");

                        var _rows = DT_All.Select("Isuser=0 and Model='" + Model + "' and  ToolTypeName='Squeegee'");
                        if (_rows.Length > 0)
                        {
                            int num = ra.Next(0, _rows.Length - 1);
                            Squeegee_Slot_No = Convert.ToString(_rows[num]["Slot No"]);
                            Squeegee_Models = Convert.ToString(_rows[num]["IncludeModel"]);
                            _rows[num]["Isuser"] = 1;
                        }
                        if (_rows_all.Length == 0)
                        {
                            continue;
                        }
                        DataTable _temp_db = ToDataTable(_rows_all);
                        _temp_db.TableName = "dt";
                        Squeegee_json = ConvertJson.ToJson(_temp_db);
                        SqueegeeCount = _rows_all.Length.ToString();
                    }
                    catch (Exception ex)
                    {
                        AddLog("异常出错", "Squeegee数据分析", ex.ToString());
                    }
                    // if (Stencil_Slot_No.Length > 0 && _StencilCount != "0")
                    {
                        string sql = "UPDATE [T_Task] set [Squeegee]='" + Squeegee_Slot_No + "',[Squeegee_Models]='" + Squeegee_Models + "',[Squeegee_Detail]='" + Squeegee_json + "',[SqueegeeCount]=" + SqueegeeCount + " where [ID]=" + id;

                        DAL.DbHelper.ExecuteSql(sql);
                        AddLog("保存数据", "Squeegee   ID:" + id.ToString(), sql);
                    }
                }

            }
            catch (Exception ex)
            {
                AddLog("异常出错", "Squeegee", ex.ToString());
            }

        }
        void timer_RunProcess_Elapsed_DEK_Pallet(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                System.Data.DataTable dt_task = new System.Data.DataTable();
                string dataupdatesql = "select * from [T_Task] where Enble=1 and [DEK_PalletCount]=0 and  DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }
                #region 确认是否跳过
                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string DEK_Pallet_Share = Convert.ToString(dt_task.Rows[i]["DEK_Pallet_Share"]);
                    if (DEK_Pallet_Share != "0")
                    {
                        string sql = "UPDATE [T_Task] set [DEK_Pallet]='" + "OK" + "',[DEK_Pallet_Detail]='" + "share" + "',[DEK_PalletCount]=" + "1" + " where [ID]=" + Convert.ToString(dt_task.Rows[i]["ID"]);
                        DAL.DbHelper.ExecuteSql(sql);
                    }

                   
                }

                #endregion
                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }
                #region 确认之前是否有可以共用

                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string Model = Convert.ToString(dt_task.Rows[i]["Model"]);
                    string From_Model = Convert.ToString(dt_task.Rows[i]["From_Model"]);
                    string From_ToolSide = Convert.ToString(dt_task.Rows[i]["From_Tool Side"]);
                    string FromRecord_sql = "select top 1 * from [T_Task] where Enble=1 and [Status]>0  and  [ExpectedTime]<now() and [Model]='" + From_Model + "' and [Tool Side]='" + From_ToolSide + "' and [DEK_Pallet_Models] like '%" + Model + "%' order by [ExpectedTime] desc";
                    DataTable recorddt = DAL.DbHelper.ExecuteSql_Table(FromRecord_sql);                   

                    if (recorddt.Rows.Count == 0)
                    {
                        continue;
                    }
                    string sql = "UPDATE [T_Task] set [ShareID]=" + Convert.ToString(recorddt.Rows[0]["ID"]) + ",[DEK_Pallet]='" + "OK" + "',[DEK_Pallet_Models]='" + Convert.ToString(recorddt.Rows[0]["DEK_Pallet_Models"]) + "',[DEK_Pallet_Detail]='" + "share" + "',[DEK_PalletCount]=" + "1" + " where [ID]=" + Convert.ToString(dt_task.Rows[i]["ID"]);
                    DAL.DbHelper.ExecuteSql(sql);                    
                }
                #endregion

                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }
                System.Data.DataTable dt_Model = new System.Data.DataTable();
                string dt_Modelsql = "select distinct [Model] from [T_Task] where Enble=1 and [DEK_PalletCount]=0 and DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_Model = DAL.DbHelper.ExecuteSql_Table(dt_Modelsql);
                if (dt_Model.Rows.Count == 0)
                {
                    return;
                }
                AddLog("当前分析(DEK_Pallet)的任务", "任务个数：" + dt_task.Rows.Count.ToString(), dataupdatesql);




                DataTable DT_All = new DataTable();

                #region For
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
                    string[] NewField = new string[] { "Model", "Slot No", "Tool Side", "Customer", "Isuser", "IncludeModel" };
                    for (int j = 0; j < NewField.Length; j++)
                    {
                        dt_Main.Columns.Add(NewField[j]);
                    }
                    for (int k = 0; k < dt_Main.Rows.Count; k++)
                    {
                        for (int j = 0; j < NewField.Length; j++)
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
                    string ToolSide = Convert.ToString(dt_task.Rows[i]["Tool Side"]);
                    string DEK_Pallet_Slot_No = "";
                    string DEK_Pallet_Models = "";
                    string DEK_PalletCount = "0";
                    string DEK_Pallet_json = "";
                    try
                    {
                        var _rows_all = DT_All.Select("Model='" + Model + "' and ToolTypeName='DEK Pallet'");

                        var _rows = DT_All.Select("Isuser=0 and Model='" + Model + "' and  ToolTypeName='DEK Pallet'");
                        if (_rows.Length > 0)
                        {
                            int num = ra.Next(0, _rows.Length - 1);
                            DEK_Pallet_Slot_No = Convert.ToString(_rows[num]["Slot No"]);
                            DEK_Pallet_Models = Convert.ToString(_rows[num]["IncludeModel"]);
                            _rows[num]["Isuser"] = 1;
                        }
                        if (_rows_all.Length == 0)
                        {
                            continue;
                        }
                        DataTable _temp_db = ToDataTable(_rows_all);
                        _temp_db.TableName = "dt";
                        DEK_Pallet_json = ConvertJson.ToJson(_temp_db);
                        DEK_PalletCount = _rows_all.Length.ToString();
                    }
                    catch (Exception ex)
                    {
                        AddLog("异常出错", "DEK Pallet数据分析", ex.ToString());
                    }
                    // if (Stencil_Slot_No.Length > 0 && _StencilCount != "0")
                    {
                        string sql = "UPDATE [T_Task] set [DEK_Pallet]='" + DEK_Pallet_Slot_No + "',[DEK_Pallet_Models]='" + DEK_Pallet_Models + "',[DEK_Pallet_Detail]='" + DEK_Pallet_json + "',[DEK_PalletCount]=" + DEK_PalletCount + " where [ID]=" + id;

                        DAL.DbHelper.ExecuteSql(sql);
                        AddLog("保存数据", "DEK Pallet   ID:" + id.ToString(), sql);
                                    
                    }
                }

            }
            catch (Exception ex)
            {
                AddLog("异常出错", "Squeegee", ex.ToString());
            }

        }     
        void  timer_RunProcess_Elapsed_Stencil(object sender, System.Timers.ElapsedEventArgs e)
        {
          
            try
            {

                System.Data.DataTable dt_task = new System.Data.DataTable();
                string dataupdatesql = "select [ID],[Stencil],[Profile Board],[Model],[Tool Side],[From_Tool Side],[From_Model] from [T_Task] where Enble=1 and [StencilCount]=0 and  DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }

                #region 确认之前是否有可以共用

                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string Model = Convert.ToString(dt_task.Rows[i]["Model"]);
                    string From_Model = Convert.ToString(dt_task.Rows[i]["From_Model"]);
                    string From_ToolSide = Convert.ToString(dt_task.Rows[i]["From_Tool Side"]);
                    string FromRecord_sql = "select top 1 * from [T_Task] where Enble=1 and [Status]>0  and  [ExpectedTime]<now() and [Model]='" + From_Model + "' and [Tool Side]='" + From_ToolSide + "' and [Stencil_Models] like '%" + Model + "%' order by [ExpectedTime] desc";
                    DataTable recorddt = DAL.DbHelper.ExecuteSql_Table(FromRecord_sql);
                    if (recorddt.Rows.Count == 0)
                    {
                        continue;
                    }
                    string sql = "UPDATE [T_Task] set [ShareID]=" + Convert.ToString(recorddt.Rows[0]["ID"]) + ",[Stencil]='" + "OK" + "',[Stencil_Models]='" + Convert.ToString(recorddt.Rows[0]["Stencil_Models"]) + "',[Stencil_Detail]='" + "share" + "',[StencilCount]=" + "1" + " where [ID]=" + Convert.ToString(dt_task.Rows[i]["ID"]);
                    DAL.DbHelper.ExecuteSql(sql);
                    
                }


                #endregion

                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }
                System.Data.DataTable dt_Model = new System.Data.DataTable();
                string dt_Modelsql = "select distinct [Model] from [T_Task] where Enble=1 and [StencilCount]=0 and DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_Model = DAL.DbHelper.ExecuteSql_Table(dt_Modelsql);
                if (dt_Model.Rows.Count == 0)
                {
                    return;
                }
                AddLog("当前分析(Stencil)的任务", "任务个数：" + dt_task.Rows.Count.ToString(), dataupdatesql);



              
                DataTable DT_All = new DataTable();

                #region For
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
                    string[] NewField = new string[] { "Model", "Slot No", "Tool Side", "Customer", "Isuser", "IncludeModel" };
                    for (int j = 0; j < NewField.Length; j++)
                    {
                        dt_Main.Columns.Add(NewField[j]);
                    }
                    for (int k = 0; k < dt_Main.Rows.Count; k++)
                    {
                        for (int j = 0; j < NewField.Length; j++)
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
                    string ToolSide = Convert.ToString(dt_task.Rows[i]["Tool Side"]);
                    string Stencil_Slot_No = "";
                    string Stencil_Models = "";
                    string _StencilCount = "0";
                    string _stencil_json = "";
                    try
                    {
                        var _rows_all = DT_All.Select("Model='" + Model + "' and [Tool Side]='" + ToolSide + "' and ToolTypeName='Stencil'");

                        var _rows = DT_All.Select("Isuser=0 and Model='" + Model + "' and [Tool Side]='" + ToolSide + "' and ToolTypeName='Stencil'");
                        if (_rows.Length > 0)
                        {
                            int num = ra.Next(0, _rows.Length - 1);
                            Stencil_Slot_No = Convert.ToString(_rows[num]["Slot No"]);
                            Stencil_Models = Convert.ToString(_rows[num]["IncludeModel"]);
                            _rows[num]["Isuser"] = 1;
                        }
                        if (_rows_all.Length == 0)
                        {
                            continue;
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
                    if (Stencil_Slot_No.Length > 0 && _StencilCount != "0")
                    {
                        string sql = "UPDATE [T_Task] set [Stencil]='" + Stencil_Slot_No + "',[Stencil_Models]='" + Stencil_Models + "',[Stencil_Detail]='" + _stencil_json + "',[StencilCount]=" + _StencilCount + " where [ID]=" + id;
                        DAL.DbHelper.ExecuteSql(sql);
                        AddLog("保存数据", "Stencil   ID:" + id.ToString(), sql);                        
                    }                  
                }

            }
            catch(Exception ex)
            {
                AddLog("异常出错", "Stencil", ex.ToString());
            }
          
        }
        void timer_RunProcess_Elapsed_ProfileBoard(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
               

                System.Data.DataTable dt_task = new System.Data.DataTable();
                string dataupdatesql = "select [ID],[Stencil],[Profile Board],[Model],[Tool Side],[From_Tool Side],[From_Model] from [T_Task] where Enble=1 and [Profile BoardCount]=0 and  DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql); 
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }

                #region 确认之前是否有可以共用

                for (int i = 0; i < dt_task.Rows.Count; i++)
                {
                    string Model = Convert.ToString(dt_task.Rows[i]["Model"]);
                    string From_Model = Convert.ToString(dt_task.Rows[i]["From_Model"]);
                    string From_ToolSide = Convert.ToString(dt_task.Rows[i]["From_Tool Side"]);
                    string FromRecord_sql = "select top 1 * from [T_Task] where Enble=1 and [Status]>0  and  [ExpectedTime]<now() and [Model]='" + From_Model + "' and [Tool Side]='" + From_ToolSide + "' and [Profile Board_Models] like '%" + Model + "%' order by [ExpectedTime] desc";
                    DataTable recorddt = DAL.DbHelper.ExecuteSql_Table(FromRecord_sql);
                    if (recorddt.Rows.Count == 0)
                    {
                        continue;
                    }
                    string sql = "UPDATE [T_Task] set [ShareID]=" + Convert.ToString(recorddt.Rows[0]["ID"]) + ",[Profile Board]='" + "OK" + "',[Profile Board_Models]='" + Convert.ToString(recorddt.Rows[0]["Profile Board_Models"]) + "',[Profile Board_Detail]='" + "share" + "',[Profile BoardCount]=" + "1" + " where [ID]=" + Convert.ToString(dt_task.Rows[i]["ID"]);    
                    DAL.DbHelper.ExecuteSql(sql);
                }


                #endregion

                dt_task = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);
                if (dt_task.Rows.Count == 0)
                {
                    return;
                }

                AddLog("当前分析(Profile Board)的任务", "任务个数：" + dt_task.Rows.Count.ToString(), dataupdatesql);
                System.Data.DataTable dt_Model = new System.Data.DataTable();
                string dt_Modelsql = "select distinct [Model] from [T_Task] where Enble=1 and [Profile BoardCount]=0 and DateAdd('n', -1*[RemindPreTime], [ExpectedTime])<=now() and [Status]=0  and  [ExpectedTime]>=now()";
                dt_Model = DAL.DbHelper.ExecuteSql_Table(dt_Modelsql);
                if (dt_Model.Rows.Count == 0)
                {
                    return;
                }

                DataTable DT_All = new DataTable();
                #region For
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
                    string cmdText_Main = @"select M.ToolID,T.ToolTypeName, M.ToolSN,P.ProcessDesc as 'Status',M.LastUpdatedDate from dbo.T_Tools as M  " +  //M.CustomerID,
                                           "inner join dbo.T_Process P on M.Status=P.ProcessID  inner join dbo.T_Type T on M.ToolTypeID=T.ToolTypeID " +
                                           "where P.ProcessDesc='Storage' and  M.ToolID in (select distinct ToolID  from  " +
                                           "dbo.T_Tools_Attribute where ProcessFieldValue like '%" + _Model + "%' )";
                    DataTable dt_Main = SqlHelper.GetDataTableOfRecord(cmdText_Main);         
                    string[] NewField = new string[] { "Model", "Slot No", "Tool Side", "Customer", "Isuser", "IncludeModel" };
                    for (int j = 0; j < NewField.Length; j++)
                    {
                        dt_Main.Columns.Add(NewField[j]);
                    }
                    for (int k = 0; k < dt_Main.Rows.Count; k++)
                    {
                        for (int j = 0; j < NewField.Length; j++)
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
                    string ToolSide = Convert.ToString(dt_task.Rows[i]["Tool Side"]);
                    string ProfileBoard_Slot_No = "";
                    string ProfileBoard_Models = "";
                    string _ProfileBoardCount = "0";
                    string _ProfileBoard_json = "";
                    try
                    {
                        var _rows_all = DT_All.Select("Model='" + Model + "' and  ToolTypeName='Profile Board'");

                        var _rows = DT_All.Select("Isuser=0 and Model='" + Model + "' and  ToolTypeName='Profile Board'");
                        if (_rows.Length > 0)
                        {
                            int num = ra.Next(0, _rows.Length - 1);
                            ProfileBoard_Slot_No = Convert.ToString(_rows[num]["Slot No"]);
                            ProfileBoard_Models = Convert.ToString(_rows[num]["IncludeModel"]);                            
                            _rows[num]["Isuser"] = 1;
                        }
                        DataTable _stencil_db = ToDataTable(_rows_all);
                        _stencil_db.TableName = "dt";
                        _ProfileBoard_json = ConvertJson.ToJson(_stencil_db);
                        _ProfileBoardCount = _rows_all.Length.ToString();
                    }
                    catch (Exception ex)
                    {
                        AddLog("异常出错", "Profile Board数据分析", ex.ToString());
                    }
                    if (ProfileBoard_Slot_No.Length > 0 && _ProfileBoardCount != "0")
                    {
                        string sql = "UPDATE [T_Task] set [Profile Board]='" + ProfileBoard_Slot_No + "',[Profile Board_Models]='" + ProfileBoard_Models + "',[Profile Board_Detail]='" + _ProfileBoard_json + "',[Profile BoardCount]=" + _ProfileBoardCount + " where [ID]=" + id;
                        DAL.DbHelper.ExecuteSql(sql);
                        AddLog("保存数据", "Profile Board   ID:" + id.ToString(), sql);
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog("异常出错", "Profile Board]", ex.ToString());
            }
        }    
        private void AddLog(string TitleName, string Log, string SQL)
        {
            try
            {
                string sql = "INSERT INTO [Tlog] ([TitleName], [Log],[SQL])";
                Log = StringToBinary(Log);
                SQL = StringToBinary(SQL);
                sql += string.Format("VALUES('{0}','{1}','{2}')", TitleName, Log, SQL);
                DAL.DbHelper.ExecuteSql(sql);
            }
            catch 
            {
            }
          
        }
        void timer_RunProcess_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {          

            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 55)
            {
                string deletesql = "delete from [Tlog] where LogTime<= DateAdd('d',-3, now()) ";
                int deleterow = DAL.DbHelper.ExecuteSql(deletesql);
                AddLog("删除Log", "[Tlog]", deletesql);
            }
            try
            {
                System.Data.DataTable DataUpdate = new System.Data.DataTable();
                string dataupdatesql = "select [ID],[Stencil],[Profile Board],[Squeegee],[DEK_Pallet] from [T_Task] where Enble=1 and [Status]=0   and  [ExpectedTime]<now()";
                DataUpdate = DAL.DbHelper.ExecuteSql_Table(dataupdatesql);              
                for (int i = 0; i < DataUpdate.Rows.Count; i++)
                {
                    string u_Status = "2";
                    string u_ID = Convert.ToString(DataUpdate.Rows[i]["ID"]);
                    string u_Stencil = Convert.ToString(DataUpdate.Rows[i]["Stencil"]);
                    string u_ProfileBoard = Convert.ToString(DataUpdate.Rows[i]["Profile Board"]);
                    string u_Squeegee = Convert.ToString(DataUpdate.Rows[i]["Squeegee"]);
                    string u_DEK_Pallet = Convert.ToString(DataUpdate.Rows[i]["DEK_Pallet"]);

                    if (u_Stencil.Length > 0 && u_ProfileBoard.Length > 0 && u_Squeegee.Length > 0 && u_DEK_Pallet.Length > 0)
                    {
                        u_Status = "1";
                    }
                    string u_sql = "UPDATE [T_Task] set [Status]=" + u_Status + " where [ID]=" + u_ID;
                    int temprow = DAL.DbHelper.ExecuteSql(u_sql);
                    AddLog("判定状态", "ID:" + u_ID, u_sql);
                }
            }
            catch(Exception ex)
            {
                AddLog("异常出错", "判定状态", ex.ToString());
            }
           
        }

        private DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构  
            foreach (DataRow row in rows)
                tmp.Rows.Add(row.ItemArray);  // 将DataRow添加到DataTable中  
            return tmp;
        } 

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

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
    }
}