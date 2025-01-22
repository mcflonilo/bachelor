using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using SQLite.CodeFirst;

namespace UltraBend.Databases.DataSet
{
    [AddINotifyPropertyChangedInterface]
    public class DataSet
    {
        [Autoincrement]
        public int Id { get; set; }

        [Index]
        [Required]
        public string Name { get; set; }

        public virtual ICollection<DataItem> DataItems { get; set; }
    }
}
