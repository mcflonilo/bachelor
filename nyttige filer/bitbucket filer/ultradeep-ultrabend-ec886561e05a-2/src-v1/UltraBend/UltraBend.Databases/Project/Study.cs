using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Project
{
    public class Study
    {

        [Index(IsUnique = true)]
        public Guid Id { get; set; }

        [Index]
        public string Name { get; set; }

        [Index("IX_StudyDesign")]
        public Guid? DesignId { get; set; }

        public virtual Design Design { get; set; }

        public Study()
        {

        }

        public virtual ICollection<Case> Cases { get; set;} = new List<Case>();

    }
}
