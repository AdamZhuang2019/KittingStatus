using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.Data
{
    /// <summary>
    /// Summary description for SaveTask
    /// </summary>
    public class SaveTask : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string Workcell = context.Request["Workcell"]; 
             string BayName = context.Request["BayName"]; 
             string Model = context.Request["Model"]; 
             string ToolSide = context.Request["ToolSide"]; 
             string ExpectedTime = context.Request["ExpectedTime"];
             string From_Model = context.Request["From_Model"]; 
             string From_ToolSide = context.Request["From_ToolSide"];

             string Share_DEKPallet = context.Request["Share_DEKPallet"];
             string ShareSqueegee = context.Request["ShareSqueegee"];

             if (Share_DEKPallet == "Y")
             {
                 Share_DEKPallet = "1";
             }
             else
             {
                 Share_DEKPallet = "0";
             }

             if (ShareSqueegee == "Y")
             {
                 ShareSqueegee = "1";
             }
             else
             {
                 ShareSqueegee = "0";
             }

             System.Data.DataTable dt = new System.Data.DataTable();
             dt = DAL.DbHelper.ExecuteSql_Table("select top 1 * from [T_Bay] where Workcell='" + Workcell + "' and BayName='" + BayName + "'");
             if (dt.Rows.Count > 0)
             {
                 string sql = "INSERT INTO [T_Task] ([Workcell], [BayName],[Tool Side],[Model],[ExpectedTime],[RemindPreTime],[RemindEmail],[CreatebyUser],[From_Tool Side],[From_Model],[DEK_Pallet_Share],[Squeegee_Share])";
                 sql += string.Format("VALUES('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}','{8}','{9}',{10},{11})", Workcell, BayName, ToolSide, Model, ExpectedTime, Convert.ToString(dt.Rows[0]["RemindPreTime"]),
                     Convert.ToString(dt.Rows[0]["RemindEmail"]), context.Request.UserHostName + ";" + 
                     context.Request.UserHostAddress, From_ToolSide, From_Model, Share_DEKPallet, ShareSqueegee);
                 DAL.DbHelper.ExecuteSql(sql);
             }
             context.Response.Write("OK");
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