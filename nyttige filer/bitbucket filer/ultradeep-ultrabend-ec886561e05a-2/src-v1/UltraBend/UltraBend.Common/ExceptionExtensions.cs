using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Common
{
    public static class ExceptionExtensions
    {
        public static void Trace(this Exception _this)
        {
            System.Diagnostics.Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, _this);
        }
    }
}
