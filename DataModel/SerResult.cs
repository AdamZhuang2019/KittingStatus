using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.DataModel
{
    /// <summary>
    /// 服务返回结果
    /// </summary>
    public class SerResult
    {
        public int status { get; set; }
        public string msg { get; set; }

        public static SerResult Success()
        {
            return SerResult.Create(0, "成功！");
        }

        public static SerResult Error()
        {
            return  SerResult.Create(-1,"失败！");
        }

        public static SerResult Create(int status, string msg)
        {
            return new SerResult { status = status, msg = msg };
        }
    }
}