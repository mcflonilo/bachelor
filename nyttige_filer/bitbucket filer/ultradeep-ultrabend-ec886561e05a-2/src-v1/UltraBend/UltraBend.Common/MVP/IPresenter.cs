using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltraBend.Common.MVP
{
    public interface IPresenter : IDisposable
    {
        void Show();
        DialogResult ShowDialog();
    }
}
