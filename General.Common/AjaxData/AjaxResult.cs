using System;
using System.Collections.Generic;
using System.Text;

namespace General.Common
{
    /// <summary>
    /// ajax请求的结果
    /// </summary>
    public class AjaxResult
    {
        
        public bool Status { get; set; }

        public object Data { get; set; }

        public string Message { get; set; }
    }
}
