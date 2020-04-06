using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using kittingStatus.jabil.web.DAL;
using System.Data.SqlClient;

namespace kittingStatus.jabil.web.Data
{
   
    /// <summary>
    /// Summary description for GetData_ByModle
    /// </summary>
    public class GetData_ByModle : IHttpHandler
    {
       // string constr = "server=HUA-PINATA-SQL;database=eTools;integrated security=SSPI";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string Model = context.Request["Model"]; //"T6B60-60002";
            if (Model != null)
            {
                if (Model.Length > 2)
                {
                    context.Response.Write(GetTableStr(Model));
                }
                else
                {
                    context.Response.Write("");
                }
            }
            else
            {
                context.Response.Write("");
            }
        }

        public string GetTableStr(string Model)
        {

            //string Modles = "'T6B60-60002','C5F98-60002','B5L47-60002','G3Q47-60001','G3Q47-60001-C','G3Q35-60001','G3Q35-60001-C','G3Q34-60001','G3Q50-60001','G3Q50-60001-C'";

             string cmdText_Part = @"select  M.ToolID,P.ProcessFieldName, M.ProcessFieldValue from dbo.T_Tools_Attribute " +
                                "as M inner join dbo.T_ProcessField as P on M.ProcessFieldID=P.ProcessFieldID " +
                                "where  ToolID in ( " +
                                "select distinct ToolID  from  " +
                                "dbo.T_Tools_Attribute where ProcessFieldValue like '%" + Model + "%' )  " +
                                 "and P.ProcessFieldName in ('Customer','Slot No','Model','Tool Side')"; //'Serial Number',
             DataTable dt_Part = SqlHelper.GetDataTableOfRecord(cmdText_Part);
             dt_Part.TableName = "Data";

             string cmdText_Main = @"select M.ToolID,T.ToolTypeName, M.ToolSN,P.ProcessDesc as 'Status',M.LastUpdatedDate from dbo.T_Tools as M  " +  //M.CustomerID,
                                    "inner join dbo.T_Process P on M.Status=P.ProcessID  inner join dbo.T_Type T on M.ToolTypeID=T.ToolTypeID " +
                                    "where P.ProcessDesc='Storage' and  M.ToolID in ( " +
                                    "select distinct ToolID  from  "+
                                    "dbo.T_Tools_Attribute where ProcessFieldValue like '%" + Model + "%' )";
             DataTable dt_Main = SqlHelper.GetDataTableOfRecord(cmdText_Main);
            string[] NewField = new string[] { "Model","Slot No","Tool Side","Customer"};
            for (int i = 0; i < NewField.Length; i++)
			{
			  dt_Main.Columns.Add(NewField[i]);
			}
           
        
            for (int i = 0; i < dt_Main.Rows.Count; i++)
            {
                for (int j = 0; j < NewField.Length; j++)
                {
                    string fieldName = NewField[j];
                    var rows = dt_Part.Select("ToolID=" +
                        Convert.ToString(dt_Main.Rows[i]["ToolID"]) + " and ProcessFieldName='" + fieldName + "'");
                    if (rows.Length > 0)
                    {
                        dt_Main.Rows[i][fieldName] = rows[0]["ProcessFieldValue"];
                        continue;
                    }
                }               
            }

           // dt_Main.Columns.Remove("ToolID");
            dt_Main.DefaultView.Sort = "Model ASC";
            dt_Main = dt_Main.DefaultView.ToTable();
            return ConvertJson.ToJson(dt_Main);
        }


        //private  DataSet ReturnDateSet(string cmdText)
        //{

        //    using (SqlConnection conn = new SqlConnection(constr))
        //    {
        //        DataSet ds = new DataSet();
        //        try
        //        {
        //            conn.Open();
        //            SqlCommand cmd;
        //            cmd = new SqlCommand(cmdText, conn);
        //            cmd.CommandTimeout = 600;
        //            SqlDataAdapter dtadapter = new SqlDataAdapter(cmd);
        //            dtadapter.Fill(ds, "DataInfo");
        //            return ds;
        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }

        //}



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}