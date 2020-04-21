using kittingStatus.jabil.web.DAL;
using kittingStatus.jabil.web.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.BLL
{
    public class ErrorBll
    {
        public static void LogError(Exception ex)
        {
            InsertLog("异常错误", ex.ToString(), ErrorLevel.Error, ex.InnerException != null ? "" : ex.InnerException.Message);
        }

        public static void LogError(string title,Exception ex)
        {
            InsertLog(title, ex.ToString(), ErrorLevel.Error, ex.InnerException!=null?"": ex.InnerException.Message);
        }

        public static void LogError(string title, Exception ex,string remark)
        {
            InsertLog(title, ex.ToString(), ErrorLevel.Error, remark);
        }

        public static void LogInfo(string title, string errorInfo, string remark="")
        {
            InsertLog(title, errorInfo, ErrorLevel.Info, remark);
        }

        public static void InsertLog(string title,string errorInfo, ErrorLevel level=ErrorLevel.Info,string remark="",string pageName="")
        {
            string sql = $@"INSERT INTO [dbo].[EKS_TError]
           (
            [Title]
           ,[ErrorInfor]
           ,[Remark]
           ,[Level]
           ,[PageName]) select @Title,@ErrorInfor,@Remark,@Level,@PageName";

            ProcedureParameter[] para = new ProcedureParameter[] {
                new ProcedureParameter("@Title",title),
                new ProcedureParameter("@ErrorInfor",errorInfo),
                new ProcedureParameter("@Remark",remark),
                new ProcedureParameter("@Level",(int)level),
                new ProcedureParameter("@PageName",pageName),

            };

            try
            {
                new DAL.DbHelper().Execute(sql, para);
            }
            catch (Exception ex)
            {

            }

        }
    }
}