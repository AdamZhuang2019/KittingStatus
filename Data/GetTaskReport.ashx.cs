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
             context.Response.Write(GetTableStr());
             
        }

        public string GetTableStr( )
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = new DAL.DbHelper().QueryDataTable(@"select  [ID],[Workcell],[BayName],isnull([Tool Side],'') as [Tool Side],[Model],[CreatedTime],[ExpectedTime],
        case when  [Status]=2 and datediff(MINUTE,getdate(),ExpectedTime)>=15  then 3 else [status] end  as [Status],
        isnull([Stencil], '') as Stencil,isnull([FeederCar],'') as FeederCar,isnull([Feeder],'') as Feeder,
		isnull([DEK_Pallet],'') as DEK_Pallet,isnull([Profile Board],'') as [Profile Board],isnull([Squeegee],'') as Squeegee,[StencilCount],
		[DEK_PalletCount],[Profile BoardCount],[SqueegeeCount],isnull([Action],'') as Action from [EKS_T_Task] where Enble='1' order by  status desc, [ExpectedTime] desc");
            dt.TableName = "data";
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