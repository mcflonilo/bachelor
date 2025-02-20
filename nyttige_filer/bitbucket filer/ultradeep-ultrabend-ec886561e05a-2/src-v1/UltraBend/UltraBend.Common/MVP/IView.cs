using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltraBend.Common.MVP
{
    public interface IView
    {

        void Show();
        DialogResult ShowDialog();

        event EventHandler Load;
    }
}
