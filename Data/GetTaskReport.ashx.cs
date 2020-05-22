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
    public class GetTaskReport : IHttpHandler
    {
       // string constr = "server=HUA-PINATA-SQL;database=eTools;integrated security=SSPI";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain"; 
             context.Response.Write(GetTableStr(context));
             
        }

        public string GetTableStr(HttpContext context)
        {
            var workcell = context.Request["workcell"];
            var bay= context.Request["bay"];
            var keywork= context.Request["keywork"];

            ProcedureParameter[] para = new ProcedureParameter[] {
                new ProcedureParameter("@workcell",workcell),
                new ProcedureParameter("@bay",bay),
                new ProcedureParameter("@keywork",keywork),

            };

            System.Data.DataTable dt = new System.Data.DataTable();
            string sql = @"select  a.[ID],[Workcell],[BayName],isnull([Tool Side],'') as [Tool Side],[Model],[CreatedTime],[ExpectedTime],
                                                    case when  [Status]=2 and datediff(MINUTE,getdate(),ExpectedTime)>=15  then 3 else [status] end  as [Status],
                                                    isnull([Stencil], '') as Stencil,isnull([FeederCar],'') as FeederCar,isnull([Feeder],'') as Feeder,
                                                    isnull([DEK_Pallet],'') as DEK_Pallet,isnull([Profile Board],'') as [Profile Board],isnull([Squeegee],'') as Squeegee,[StencilCount],
                                                    [DEK_PalletCount],[Profile BoardCount],[SqueegeeCount],isnull([Action],'') as Action,a.uph as UPH,a.buildplan as BuildPlan,isnull (a.RealExpectedTime,'')  AS RealExpectedTime,
													CONVERT(varchar,b.ShiftStartTime,108) as ShiftStartTime,CONVERT(varchar,b.shiftEndTime,108) as shiftEndTime ,b.ShiftDescription
                                                    from [EKS_T_Task] a join EKS_T_ShiftInfo b on a.[Shift]=b.ShiftID  where Enble='1' and DATEDIFF(DAY,getdate(),ExpectedTime)>=0 ";

            if (!string.IsNullOrWhiteSpace(workcell))
            {
                sql = $"{sql}  and workcell =@workcell";
            }

            if (!string.IsNullOrWhiteSpace(bay))
            {
                sql = $"{sql}  and BayName =@bay";
            }

            if (!string.IsNullOrWhiteSpace(keywork))
            {
                sql = $"{sql}  and ( Workcell like '%'+@keywork+'%' or BayName like '%'+@keywork+'%' or model like '%'+@keywork+'%' )";
            }

            sql = $"{sql}  order by  [ExpectedTime] asc";

            try
            {
                dt = new DAL.DbHelper().QueryDataTable(sql,para);
                dt.TableName = "data";
            }
            catch (Exception ex)
            {
                
            }

            return ConvertJson.ToJson(dt);
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