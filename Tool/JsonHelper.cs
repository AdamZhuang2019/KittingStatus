using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kittingStatus.jabil.web.Tool
{
    public class JsonHelper
    {
        public static float GetJValueFloat(JToken jt, string key)
        {
            var result = GetJValue(jt, key);
            float f = 0;
            if (!float.TryParse(result, out f))
            {
                f = 0;
            }

            return f;


        }
        public static string GetJValue(JToken jt, string key)
        {
            if (jt == null || string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            if (jt[key] == null)
            {
                return string.Empty;
            }

            return jt[key].ToString();


        }
    }
}