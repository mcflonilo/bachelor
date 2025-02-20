using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraBend.Databases.Project
{
    public class LoadContour
    {
        [Index(IsUnique = true)]
        public Guid Id { get; set; }

        [Index]
        public string Name { get; set; }

        public byte[] DeflectionAngleData { get; set; }

        public byte[] UmbilicalTensionData { get; set; }
    }
}
