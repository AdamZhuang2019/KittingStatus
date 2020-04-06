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
             string ExpectedTime = context.Request["ExpectedTime"];
            

            string Share_ProfileBoard = context.Request["Share_ProfileBoard"];
            string Share_DEKPallet = context.Request["Share_DEKPallet"];
             string ShareSqueegee = context.Request["ShareSqueegee"];


            if (Share_ProfileBoard == "Y")
            {
                Share_ProfileBoard = "1";
            }
            else
            {
                Share_ProfileBoard = "0";
            }

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
                 string sql = "INSERT INTO [T_Task] ([Workcell], [BayName],[Model],[ExpectedTime],[RemindPreTime],[RemindEmail],[CreatebyUser],[DEK_Pallet_Share],[Squeegee_Share],[ProfileBoard_Share])";
                 sql += string.Format("VALUES('{0}','{1}','{2}','{3}',{4},'{5}','{6}',{7},{8},{9})", Workcell, BayName, Model, ExpectedTime, Convert.ToString(dt.Rows[0]["RemindPreTime"]),
                     Convert.ToString(dt.Rows[0]["RemindEmail"]), context.Request.UserHostName + ";" + 
                     context.Request.UserHostAddress, Share_DEKPallet, ShareSqueegee, Share_ProfileBoard);
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