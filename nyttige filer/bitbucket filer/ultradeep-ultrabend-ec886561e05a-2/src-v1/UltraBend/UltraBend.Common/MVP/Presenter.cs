using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltraBend.Common.MVP
{
    public abstract class Presenter<TView, TModel> : IPresenter<TView, TModel>
        where TView : class, IView
        where TModel : class
    {


        protected Presenter(TView view)
        {
            this.View = view;
        }

        public TView View { get; }
        public TModel Model { get; set; }
        public virtual void Show()
        {
            View.Show();
        }
        
        public virtual DialogResult ShowDialog()
        {
            return View.ShowDialog();
        }

        public void Dispose()
        {

        }
    }
}
