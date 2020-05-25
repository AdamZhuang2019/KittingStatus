using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabilCommon;
using kittingStatus.jabil.web.DataModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using kittingStatus.jabil.web.Tool;
using kittingStatus.jabil.web.DAL;
using System.Data;

namespace kittingStatus.jabil.web.BLL
{
    public class BulidPlanBll
    {
        public static DbHelper sqlser = new DbHelper();
        public static string BulidPlanServerUrl = System.Configuration.ConfigurationManager.AppSettings["BuildPlanServerUrl"];

        public static List<BuildPlanInfo> GetSynBulidPlanData(DateTime start, DateTime end)
        {
            if (start == null || start == DateTime.MinValue)
            {
                start = DateTime.Now.AddDays(-3);
            }

            if (end == null || end == DateTime.MinValue)
            {
                end = DateTime.Now;
            }

            List<BuildPlanInfo> list = new List<BuildPlanInfo>();
            BuildPlanInfo temp = null;

            string para = $"start={start.ToString("yyyy-MM-dd")}&end={end.ToString("yyyy-MM-dd")}";
            var result = JabilCommon.HttpMethods.HttpPost(BulidPlanServerUrl, para);

            if (string.IsNullOrWhiteSpace(result))
            {
                return list;
            }

            JObject jo = (JObject)JsonConvert.DeserializeObject(result);

            if (jo["Status"] == null)
            {
                return list;
            }

            var status = jo["Status"].ToString();
            if (status != "0")
            {
                return list;
            }

            if (jo["Data"] == null)
            {
                return list;
            }

            var data = (JArray)jo["Data"];
            foreach (var it in data)
            {
                temp = new BuildPlanInfo();
                temp.BuildPlanID = JsonHelper.GetJValue(it, "BuildPlanID");
                temp.Workcell = JsonHelper.GetJValue(it, "Workcell");
                temp.Bay = JsonHelper.GetJValue(it, "Bay");
                temp.Station = JsonHelper.GetJValue(it, "Station");
                temp.Model = JsonHelper.GetJValue(it, "Model");
                temp.BuildPlan = JsonHelper.GetJValue(it, "BuildPlan");
                temp.Shift = JsonHelper.GetJValue(it, "Shift");
                temp.ShiftDate = JsonHelper.GetJValue(it, "ShiftDate");
                temp.StartTime = JsonHelper.GetJValue(it, "StartTime");
                temp.EndTime = JsonHelper.GetJValue(it, "EndTime");
                temp.UPH = JsonHelper.GetJValueFloat(it, "UPH");
                list.Add(temp);
            }

            return list;

        }
        /// <summary>
        /// 清理数据
        /// </summary>
        public static void ClearBulidPlanData()
        {
            string sql = $@"truncate table EKS_T_BulidPlanData";
            try
            {
                sqlser.Execute(sql, null);
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("清除EKS_T_BulidPlanData表数据异常！", ex);

            }
        }
        /// <summary>
        /// 插入buildplan数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool InsertBulidPlanData(BuildPlanInfo item)
        {
            string sql = $@" INSERT INTO [dbo].[EKS_T_BulidPlanData]
                               ([ID],[BuildPlanID] ,[Workcell],[Bay],[Station],[Model]
                               ,[BuildPlan],[Shift],[ShiftDate],[StartTime] ,[EndTime],[UPH])
                         VALUES
                               (@ID,@BuildPlanID,@Workcell,@Bay,@Station,@Model,
                                @BuildPlan,@Shift,@ShiftDate,@StartTime,@EndTime,@UPH)";

            DAL.ProcedureParameter[] para = new DAL.ProcedureParameter[] {
                new DAL.ProcedureParameter("@ID",item.ID),
                new DAL.ProcedureParameter("@BuildPlanID",item.BuildPlanID),
                new DAL.ProcedureParameter("@Workcell",item.Workcell),
                new DAL.ProcedureParameter("@Bay",item.Bay),
                new DAL.ProcedureParameter("@Station",item.Station),
                new DAL.ProcedureParameter("@Model",item.Model),
                new DAL.ProcedureParameter("@BuildPlan",item.BuildPlan),
                new DAL.ProcedureParameter("@Shift",item.Shift),
                new DAL.ProcedureParameter("@ShiftDate",item.ShiftDate),
                new DAL.ProcedureParameter("@StartTime",item.StartTime),
                new DAL.ProcedureParameter("@EndTime",item.EndTime),
                new DAL.ProcedureParameter("@UPH",item.UPH),
            };

            try
            {
               return sqlser.Execute(sql, para)>0;
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("同步buildplan数据时，插入buildplan数据异常！", ex);
                return false;
            }

        }


        public static bool UpdateBuildPlanCostTime()
        {
            string sql = $@" update  [dbo].[EKS_T_BulidPlanData] set CostTime=0;
                             update [dbo].[EKS_T_BulidPlanData] set CostTime = Round(BuildPlan/UPH/0.9,2) where UPH <> 0 ";

            try
            {
                return sqlser.Execute(sql, null) > 0;
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("同步buildplan数据时，更新CostTime异常！", ex);
                return false;
            }
        }

        /// <summary>
        /// bulidplan 数据转换为task
        /// </summary>
        public static void ExecuteBuildPlanData2Task()
        {

            object locko = new object();
            lock (locko)
            {
                ErrorBll.LogInfo("开始同步BuildPlan数据", "开始同步BuildPlan数据");
                //抓取数据
                List<BuildPlanInfo> list = GetSynBulidPlanData(DateTime.Now.AddDays(-3).Date, DateTime.Now.AddDays(1).Date);
                if (list.Count == 0)
                {
                    return;
                }

                //
                ClearBulidPlanData();

                int allCount = list.Count;
                int successCount = 0;
                foreach (var it in list)
                {
                    if (InsertBulidPlanData(it))
                    {
                        successCount++;
                    }
                }

                ErrorBll.LogInfo("插入BulidPlan数据到本地数据库", $"总记录数:[{allCount}],成功数：[{successCount}]");
                //先计算CostTime
                UpdateBuildPlanCostTime();

                //先计算时间，分别计算出
                var dates = GetDistinctBulidPlanDate();
                foreach (var date in dates)
                {
                    var types = GetDistinctBulidPlanShiftType(date);
                    foreach (var type in types)
                    {
                        var workcells = GetDistinctBulidPlanWorkcell(date, type);
                        foreach (var wc in workcells)
                        {
                            var bays = GetDistinctBulidPlanBays(date, type, wc);
                            foreach (var bay in bays)
                            {
                                // 执行存储过程进行排班
                                string sql = $@"P_BuildPlanShiftType2Task";

                                DAL.ProcedureParameter[] para = new DAL.ProcedureParameter[] {
                                                            new DAL.ProcedureParameter("@shiftdate",date),
                                                            new DAL.ProcedureParameter("@shift",type),
                                                            new DAL.ProcedureParameter("@workcell",wc),
                                                            new DAL.ProcedureParameter("@bay",bay),
                                                            new DAL.ProcedureParameter("@Status",-1,SqlDbType.Int,8,ParameterDirection.Output),
                                                            new DAL.ProcedureParameter("@Msg","",SqlDbType.VarChar,50,ParameterDirection.Output)
                                                        };

                                para[4].Direction = ParameterDirection.Output;
                                para[5].Direction = ParameterDirection.Output;

                                try
                                {
                                    sqlser.Execute(sql, para, true);

                                    var result = para[4].Value == null ? -1 : para[4].Value.GetInt();
                                    if (result == 0)
                                    {
                                        ErrorBll.LogInfo("计算单个生产计划的开始生产时间", $"[{date.ToString("yyyy-MM-dd")}] 班次：[{type}] workcell:[{wc}] bay:[{bay}] 当前班次的所有生产计划的具体生产时间成功！");
                                    }
                                    else
                                    {
                                        ErrorBll.LogInfo("计算单个生产计划的开始生产时间", $"[{date.ToString("yyyy-MM-dd")}] 班次：[{type}] workcell:[{wc}] bay:[{bay}] 当前班次的所有生产计划的具体生产时间失败,[{para[5].Value}]");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ErrorBll.LogError($"计算单个生产计划的开始生产时间 [{date.ToString("yyyy-MM-dd")}] 班次：[{type}] workcell:[{wc}] bay:[{bay}] 当前班次的所有生产计划的具体生产时间失败,[{para[5].Value}]", ex);
                                }
                            }
                        }

                    }
                }

                //正式创建Task 
                SynTask();
            }




        }

        /// <summary>
        /// 获取所有的排班日期
        /// </summary>
        /// <returns></returns>
        public static List<DateTime> GetDistinctBulidPlanDate()
        {
            List<DateTime> result = new List<DateTime>();
           string sql = $@"select distinct ShiftDate from  EKS_T_BulidPlanData";
            try
            {
                DataTable dt= sqlser.QueryDataTable(sql, null);
                if (dt.Rows.Count == 0)
                {
                    return result;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["ShiftDate"] != DBNull.Value)
                    {
                        result.Add(dr["ShiftDate"].ToDate());
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("获取ShiftDate异常！", ex);
                return result;

            }
        }


        /// <summary>
        /// 获取排班日期内所有的排班类型
        /// </summary>
        /// <returns></returns>
        public static List<int> GetDistinctBulidPlanShiftType(DateTime date)
        {
            List<int> result = new List<int>();
            string sql = $@"select distinct [Shift] from  EKS_T_BulidPlanData where ShiftDate='{date.ToString("yyyy-MM-dd")}'";
            try
            {
                DataTable dt = sqlser.QueryDataTable(sql, null);
                if (dt.Rows.Count == 0)
                {
                    return result;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Shift"] != DBNull.Value)
                    {
                        result.Add(dr["Shift"].ToInt());
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("获取ShiftType异常！", ex);
                return result;

            }
        }


        /// <summary>
        /// 获取排班日期内一个排班类型里面的所有workcell
        /// </summary>
        /// <param name="date"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static List<string> GetDistinctBulidPlanWorkcell(DateTime date,int shift)
        {
            List<string> result = new List<string>();
            string sql = $@"select distinct Workcell from  EKS_T_BulidPlanData where ShiftDate='{date.ToString("yyyy-MM-dd")}' and [shift]='{shift}' and workcell <> '' ";
            try
            {
                DataTable dt = sqlser.QueryDataTable(sql, null);
                if (dt.Rows.Count == 0)
                {
                    return result;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Workcell"] != DBNull.Value)
                    {
                        result.Add(dr["Workcell"].ToString());
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("获取Workcell异常！", ex);
                return result;

            }
        }


        /// <summary>
        /// 获取排班日期内一个排班类型里面的所有workcell
        /// </summary>
        /// <param name="date"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static List<string> GetDistinctBulidPlanBays(DateTime date, int shift,string workcell)
        {
            List<string> result = new List<string>();
            string sql = $@"select distinct Bay from  EKS_T_BulidPlanData where ShiftDate='{date.ToString("yyyy-MM-dd")}' and [shift]='{shift}' and workcell = '{workcell}' and bay <> '' ";
            try
            {
                DataTable dt = sqlser.QueryDataTable(sql, null);
                if (dt.Rows.Count == 0)
                {
                    return result;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Bay"] != DBNull.Value)
                    {
                        result.Add(dr["Bay"].ToString());
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("获取Bay异常！", ex);
                return result;

            }
        }
        /// <summary>
        /// 同步任务
        /// </summary>
        public static void SynTask()
        {
            ErrorBll.LogInfo("开始同步生成Task任务", $"任务同步完成！");
            // 执行存储过程进行排班
            string sql = $@"P_Task_SYN";

            DAL.ProcedureParameter[] para = new DAL.ProcedureParameter[] {
                                                            new DAL.ProcedureParameter("@Status",0,SqlDbType.Int,8,ParameterDirection.Output),
                                                            new DAL.ProcedureParameter("@Msg","",SqlDbType.VarChar,50,ParameterDirection.Output)
                                                        };

            para[0].Direction = ParameterDirection.Output;
            para[1].Direction = ParameterDirection.Output;

            try
            {
                sqlser.Execute(sql, para, true);

                var result = para[0].Value == null ? -1 : para[0].Value.GetInt();
                if (result == 0)
                {
                    ErrorBll.LogInfo("同步Task任务", $"任务同步完成！");
                }
                else
                {
                    ErrorBll.LogInfo("同步Task任务", $"任务同步失败！");
                }
            }
            catch (Exception ex)
            {
                ErrorBll.LogError($"同步Task任务异常", ex);
            }
        }

    }
}