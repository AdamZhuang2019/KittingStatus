using kittingStatus.jabil.web.DAL;
using kittingStatus.jabil.web.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.BLL
{
    public class TaskBll
    {
        /// <summary>
        /// update Feeder Card status 
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="FeederCarNo"></param>
        /// <returns></returns>
        public static SerResult UpdataFeederCard(string taskID,string FeederCarNo)
        {
            string sql = $@" update [dbo].[EKS_T_Task] set FeederCar=@FeederCar,Feeder='OK' where ID=@ID ";

            ProcedureParameter[] para = new ProcedureParameter[] {
                new ProcedureParameter("@FeederCar",FeederCarNo),
                new ProcedureParameter("@ID",taskID),

            };

            try
            {
                new DAL.DbHelper().Execute(sql, para);
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("更新喂料车状态异常！", ex);
                return SerResult.Create(-1,"系统繁忙！");
            }

            return SerResult.Success();
        }

        public static SerResult UpdataStencil(string taskID, string stencil)
        {
            string sql = $@" update [dbo].[EKS_T_Task] set Stencil=@Stencil,StencilCount=1 where ID=@ID ";

            ProcedureParameter[] para = new ProcedureParameter[] {
                new ProcedureParameter("@Stencil",stencil),
                new ProcedureParameter("@ID",taskID),

            };

            try
            {
                new DAL.DbHelper().Execute(sql, para);
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("更新钢网状态异常！", ex);
                return SerResult.Create(-1, "系统繁忙！");
            }

            return SerResult.Success();
        }

        public static SerResult UpdataTaskStatus(string taskID)
        {
            string sql = $@"if exists ( select 1 from [dbo].[EKS_T_Task] a where a.ID=@TaskID and Feeder is not null and Feeder <> ''
                            and Stencil is not null and Stencil <>'' and DEK_Pallet is not null  and DEK_Pallet <> ''
                            and [Profile Board] is not null and [Profile Board] <>'' and Squeegee is not null and Squeegee <>'')
                            begin
                                 update [dbo].[EKS_T_Task] set Status='1' where ID=@TaskID
                            end ";

            ProcedureParameter[] para = new ProcedureParameter[] {
                new ProcedureParameter("@TaskID",taskID),

            };

            try
            {
                new DAL.DbHelper().Execute(sql, para);
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("更新任务状态异常！", ex);
                return SerResult.Create(-1, "系统繁忙！");
            }

            return SerResult.Success();
        }

        /// <summary>
        /// 释放任务，一个小时后自动释放
        /// </summary>
        /// <returns></returns>
        public static SerResult FreeTask()
        {
            string sql = $@" update [dbo].[EKS_T_Task] set Enble =0 where datediff(MINUTE,ExpectedTime ,getdate())>60 and status='1' ";

            try
            {
                new DAL.DbHelper().Execute(sql);
            }
            catch (Exception ex)
            {
                ErrorBll.LogError("释放任务状态异常！", ex);
                return SerResult.Create(-1, "系统繁忙！");
            }

            return SerResult.Success();
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static DataRow GetTaskInfo(string taskId)
        {

            string sql = $@" select * from  [dbo].[EKS_T_Task]  where ID=@ID ";

            ProcedureParameter[] para = new ProcedureParameter[] {
                new ProcedureParameter("@ID",taskId),
            };

            try
            {
               return new DAL.DbHelper().QueryDataRow(sql, para);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}