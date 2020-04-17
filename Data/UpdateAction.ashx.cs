using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.Data
{
    /// <summary>
    /// Summary description for UpdateExpectedTime
    /// </summary>
    public class UpdateAction : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //    url: '../Data/UpdateExpectedTime.ashx?ID=' + idIndex + '&name=' + field + '&value=' + fieldIndex,
            try
            {
                string ID = context.Request["ID"];
                string name = context.Request["name"];
                string value = context.Request["value"];

                string sql = "update [EKS_T_Task]  set [Action]='" + value + "' ";
                sql += string.Format(" where ID={0}", ID);
                new  DAL.DbHelper().Execute(sql);   
            }
            catch
            {
              
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