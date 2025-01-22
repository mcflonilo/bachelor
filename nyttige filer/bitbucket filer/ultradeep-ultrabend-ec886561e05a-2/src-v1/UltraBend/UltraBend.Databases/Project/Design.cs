using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Project
{
    public class Design
    {
        [Index(IsUnique = true)]
        public Guid Id { get; set; }

        [Index]
        public string Name { get; set; }
        


        public Design()
        {
        }
        public virtual ICollection<DesignSection> Sections { get; set; } = new List<DesignSection>();

        public virtual ICollection<Case> Cases { get; set; } = new List<Case>();

        public virtual ICollection<Study> Studies { get; set; } = new List<Study>();

        public virtual Umbilical Umbilical{ get; set; }

        public Guid? UmbilicalId { get; set; }
    }
}
