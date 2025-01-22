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
    public class DataItem
    {
        [Autoincrement]
        public int Id { get; set; }

        [Index("IX_DataSetDataItem")]
        [Required]
        public int DataSetId { get; set; }

        public virtual DataSet DataSet { get; set; }
        public byte[] Data { get; set; }
    }
}
