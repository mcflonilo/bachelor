using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Controls
{
    public interface ICsvBindingList
    {
        bool RaiseListChangedEvents { get; set; }
        void ResetBindings();
    }

    public class CsvBindingList<T> : BindingList<T>, ICsvBindingList
    {
        public CsvBindingList() : base() {
        }

        /// <include file='doc\BindingList.uex' path='docs/doc[@for="BindingList.BindingList1"]/*' />
        /// <devdoc>
        ///     Constructor that allows substitution of the inner list with a custom list.
        /// </devdoc>
        public CsvBindingList(IList<T> list) : base(list) {
        }
    }
}
