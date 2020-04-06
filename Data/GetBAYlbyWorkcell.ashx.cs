using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace kittingStatus.jabil.web.Data
{
    /// <summary>
    /// Summary description for GetModelbyWorkcell
    /// </summary>
    public class GetBAYlbyWorkcell : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";            
            string workcell = context.Request["workcell"];          
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = DAL.DbHelper.ExecuteSql_Table("select  BayName from [T_Bay] where Workcell='" + workcell + "'");
            string Model_list = "";
            List<DataModel.select2> ModelObj = new List<DataModel.select2>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataModel.select2 o = new DataModel.select2();
                o.id = i;
                o.text = Convert.ToString(dt.Rows[i]["BayName"]).Trim();
                ModelObj.Add(o);
            }                  
            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
            Model_list = jsonSerialize.Serialize(ModelObj);
            context.Response.Write(Model_list);
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