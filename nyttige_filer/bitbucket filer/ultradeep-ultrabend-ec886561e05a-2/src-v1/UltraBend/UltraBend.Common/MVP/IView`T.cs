using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Common.MVP
{
    public interface IView<TModel> : IView
    {
        TModel Model { get; set; }
    }
}
