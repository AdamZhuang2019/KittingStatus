using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.Data
{
    /// <summary>
    /// Summary description for GetData_ByID
    /// </summary>
    public class GetData_ByID : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

           
            string ID =context.Request["ID"]; 
            string Tpye =context.Request["Tpye"]; //"T6B60-60002";ID=" + ID + "&Tpye="+Tpye;
            System.Data.DataTable dt = new System.Data.DataTable();
            if (Tpye == "Stencil")
            {
                dt = new DAL.DbHelper().QueryDataTable("select [Stencil_Detail] from [EKS_T_Task] where ID=" + ID);
                if (dt.Rows.Count > 0)
                {
                    context.Response.Write(Convert.ToString(dt.Rows[0][0]));
                    return ;
                }
                return;
            }
            if (Tpye == "ProfileBoard")
            {
                dt = new DAL.DbHelper().QueryDataTable("select [Profile Board_Detail] from [EKS_T_Task] where ID=" + ID);
                context.Response.Write(Convert.ToString(dt.Rows[0][0]));
                return;
            }


            if (Tpye == "Squeegee")
            {
                dt = new DAL.DbHelper().QueryDataTable("select [Squeegee_Detail] from [EKS_T_Task] where ID=" + ID);
                if (dt.Rows.Count > 0)
                {
                    context.Response.Write(Convert.ToString(dt.Rows[0][0]));
                    return;
                }
                return;
            }
            if (Tpye == "DEK_Pallet")
            {
                dt = new DAL.DbHelper().QueryDataTable("select [DEK_Pallet_Detail] from [EKS_T_Task] where ID=" + ID);
                context.Response.Write(Convert.ToString(dt.Rows[0][0]));
                return;
            }

            context.Response.Write("");
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