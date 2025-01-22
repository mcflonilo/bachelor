using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Views
{
    public interface IButtonGroup
    {
        ButtonGroup GetButtonGroup();
        ButtonGroup GetButtonGroupToShowByDefault();
    }
}
