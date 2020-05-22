
using kittingStatus.jabil.web.BLL;
using kittingStatus.jabil.web.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.Data
{
    /// <summary>
    /// Summary description for UpdateExpectedTime
    /// </summary>
    public class UpdateExpectedTime : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string ID = context.Request["ID"];
            string name = context.Request["name"];
            string value = context.Request["value"];
            try
            {
               

                DataRow dr=TaskBll.GetTaskInfo(ID);
                if (dr == null || dr["Status"] == null)
                {
                    context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { status = -1, msg = "计划的TaskID非法！" }));
                    return;
                }

                var status = dr["Status"] == DBNull.Value ? "-1" : dr["Status"].ToString();

                if (status != "1") // 非配料状态
                {
                    context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject( new { status=-1,msg="状态异常，该计划为无料状态！" }));
                    return;
                }


                ProcedureParameter[] para = new ProcedureParameter[] {
                new ProcedureParameter("@CurBuildPlanID",int.Parse(dr["BuildPlanID"].ToString())),
                new ProcedureParameter("@RealExpectedTime",value)

                };


                new DAL.DbHelper().Execute("P_UpdateExpectedTime",para,true);

                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { status = 0, msg = "操作完成！" }));
            }
            catch(Exception ex)
            {
                BLL.ErrorBll.LogError($"更新计划Taskid:[{ID}]的realExpectedTime异常", ex);
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { status = -1, msg = "系统繁忙！" }));

            }
              
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}