using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.DataModel
{
    public class baseModel
    {
        private string m_ID;

        public string ID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_ID))
                {
                    m_ID = Guid.NewGuid().ToString();
                }

                return m_ID;
            }
        }
    }


    public class BuildPlanInfo:baseModel
    {
        public string BuildPlanID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Workcell { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Bay { get; set; }
        /// <summary>
        /// 工位
        /// </summary>
        public string Station { get; set; }
        /// <summary>
        /// 模型
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 生产技术
        /// </summary>
        public string BuildPlan { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        public string Shift { get; set; }
        /// <summary>
        /// 班次日期
        /// </summary>
        public string ShiftDate { get; set; }
        /// <summary>
        /// 班次开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 班次结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        ///  UPH
        /// </summary>
        public float UPH { get; set; }
    }

    /// <summary>
    ///  BuildPlan 班次
    /// </summary>
    public class BuildPlanShiftDate
    {
        public DateTime ShiftDate { get; set; }
    }
}