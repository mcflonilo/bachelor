using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;

namespace UltraBend.Common.MVP
{
    public interface IPresenter<out TView, out TModel> : IPresenter
        where TView : class, IView
        where TModel : class
    {
        TView View { get; }
        TModel Model { get; }
    }
}
