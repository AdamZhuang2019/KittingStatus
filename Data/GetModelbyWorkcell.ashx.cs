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
    public class GetModelbyWorkcell : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";            
            string workcell = context.Request["workcell"]; 
            string bayName = context.Request["bayName"];
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = new DAL.DbHelper().QueryDataTable("select top 1 Model from [EKS_T_Bay] where Workcell='" + workcell + "' and BayName='" + bayName + "'");

            string Model_list = "";
            List<DataModel.select2> ModelObj = new List<DataModel.select2>();
            if (dt.Rows.Count > 0)
            {
                string[] templist = Convert.ToString(dt.Rows[0]["Model"]).Split(';');
                for (int i = 0; i < templist.Length; i++)
                {
                    DataModel.select2 o = new DataModel.select2();
                    o.id = i;
                    o.text = templist[i].Trim();
                    ModelObj.Add(o);
                }
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