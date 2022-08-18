using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{
    public static class DoException
    {
        public static string ToSimple(this Exception ex, string Action = "", string Remark = "")
        {
            if (ex == null) return string.Empty;
            return string.IsNullOrEmpty(Action)
                ? new { ex.Message, ex.Source, ex.StackTrace }.ToJson()
                : new { Action, ex.Message, ex.Source, ex.StackTrace, Remark }.ToJson();
        }
    }
}
