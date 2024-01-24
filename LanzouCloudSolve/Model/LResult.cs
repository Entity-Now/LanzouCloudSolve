using System;
using System.Collections.Generic;
using System.Text;

namespace LanzouCloudSolve.Model
{
    /// <summary>
    /// 蓝奏云直链实体
    /// </summary>
    public class LResult
    {
        public int zt { get; set; }
        public string dom { get; set; }
        public string url { get; set; }
        public object inf { get; set; }
        public string address { get; set; }

        public string GetLink()
        {
            return dom + "/file/" + url;
        }
    }
}
