using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.BLL
{
    public class WorkCellBll
    {
        /// <summary>
        /// 获取workcell
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWorkcellList()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = new DAL.DbHelper().QueryDataTable("select distinct Workcell from [dbo].[EKS_T_Task] where Workcell is not null and Workcell <> '' ");
            return dt;
        }

        /// <summary>
        /// 获取拉信息
        /// </summary>
        /// <param name="workcell"></param>
        /// <returns></returns>
        public static DataTable GetBayList( string workcell)
        {
            string sql = "select distinct BayName from [dbo].[EKS_T_Task] where Workcell is not null and Workcell <> '' ";

            if (!string.IsNullOrWhiteSpace(workcell))
            {
                sql = $"{sql} and workcell='{workcell}'";
            }

            return new DAL.DbHelper().QueryDataTable(sql);
        }
    }
}