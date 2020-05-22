using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using kittingStatus.jabil.web.DataModel;
using kittingStatus.jabil.web.BLL;
using System.Threading.Tasks;
using System.Data;
using System.Web.Script.Serialization;

namespace kittingStatus.jabil.web.Data
{
    /// <summary>
    /// Summary description for SaveTask
    /// </summary>
    public class KittingServices : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string ServiceKey = context.Request["ServiceKey"];

            if (string.IsNullOrWhiteSpace(ServiceKey))
            {
                context.Response.Write(JsonConvert.SerializeObject(SerResult.Create(-1, "Service Key is error")));
            }

            try
            {
                switch (ServiceKey)
                {
                    case "UpdataFeederCard":
                        UpdataFeederCard(context); break;
                    case "UpdataStencil":
                        UpdataStencil(context); break;
                    case "GetWorkCellList":
                        GetWorkcellList(context); break;
                    case "GetBayList":
                        GetBayList(context);break;
                    default:
                        {
                            context.Response.Write(JsonConvert.SerializeObject(SerResult.Success()));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                ErrorBll.InsertLog($"调用服务器接口异常ServiceKey:[{ServiceKey}]",$"ex:[{ex.ToString()}]");
                context.Response.Write(JsonConvert.SerializeObject(SerResult.Create(-1,"系统繁忙！")));
            }
        }

        private void UpdataStencil(HttpContext context)
        {
            string taskid = context.Request["TaskID"];
            string stencil = context.Request["Stencil"];

            var result = TaskBll.UpdataStencil(taskid, stencil);
            Task.Factory.StartNew(() =>
            {
                TaskBll.UpdataTaskStatus(taskid);
            }
           );
            context.Response.Write(JsonConvert.SerializeObject(result));
        }

        public void UpdataFeederCard(HttpContext context)
        {
            string taskid= context.Request["TaskID"];
            string feedercard = context.Request["FeederCar"];

            feedercard = feedercard.TrimEnd(';');
            feedercard = $"{feedercard};";

            var result= TaskBll.UpdataFeederCard(taskid, feedercard);
            Task.Factory.StartNew(() =>
            {
                TaskBll.UpdataTaskStatus(taskid);
            }
            );
           
            context.Response.Write(JsonConvert.SerializeObject(result));
        }


        public void GetWorkcellList(HttpContext context)
        {
            DataTable dt= WorkCellBll.GetWorkcellList();
            List<object> ModelObj = new List<object>();

            if (dt != null && dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {  
                    ModelObj.Add( 
                        new 
                        { 
                            id=dr["Workcell"].ToString(),
                            text= dr["Workcell"].ToString()
                        }

                        );
                }
               
            }

            context.Response.Write(JsonConvert.SerializeObject(ModelObj));
        }

        public void GetBayList(HttpContext context)
        {
            string workcell = context.Request["WorkCell"];
            DataTable dt = WorkCellBll.GetBayList(workcell);
            List<object> ModelObj = new List<object>();

            if (dt != null && dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ModelObj.Add(
                        new
                        {
                            id = dr["BayName"].ToString(),
                            text = dr["BayName"].ToString()
                        }

                        );
                }

            }

            context.Response.Write(JsonConvert.SerializeObject(ModelObj));
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