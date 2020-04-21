using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using kittingStatus.jabil.web.DAL;
using System.Data.SqlClient;
using System.Text;

namespace kittingStatus.jabil.web.Data
{
   
    /// <summary>
    /// Summary description for GetData_ByModleEKS_TError
    /// </summary>
    public class GetData_Log : IHttpHandler
    {
       // string constr = "server=HUA-PINATA-SQL;database=eTools;integrated security=SSPI";
        public void ProcessRequest(HttpContext context)
        {
          
            context.Response.ContentType = "text/plain"; 
             context.Response.Write(GetTableStr());
             
        }

        public string GetTableStr( )
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = new DAL.DbHelper().QueryDataTable("select *  from [EKS_Tlog] order by [ID] desc ");
            dt.TableName = "data";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Log"] =BinaryToString(Convert.ToString(dt.Rows[i]["Log"]));
                dt.Rows[i]["SQL"] = BinaryToString(Convert.ToString(dt.Rows[i]["SQL"]));
            }
            return ConvertJson.ToJson(dt);
        }

        public string BinaryToString(string str)
        {
            System.Text.RegularExpressions.CaptureCollection cs =
            System.Text.RegularExpressions.Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return Encoding.Unicode.GetString(data, 0, data.Length);

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