﻿using System;
using System.Collections.Generic;
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
            //    url: '../Data/UpdateExpectedTime.ashx?ID=' + idIndex + '&name=' + field + '&value=' + fieldIndex,
            try
            {
                string ID = context.Request["ID"];
                string name = context.Request["name"];
                string value = context.Request["value"];             

                string sql = "update [T_Task]  set [ExpectedTime]='" + value + "' ";
                sql += string.Format(" where ID={0}", ID);
                DAL.DbHelper.ExecuteSql(sql);   
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