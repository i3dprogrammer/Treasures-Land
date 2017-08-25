using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasuresLand.Objects;

namespace DrNadaTreasureLand.Objects
{
    class CheckedObject
    {
        public bool Checked { get; set; }
        public Class Class { get; set; }
        public Course Course { get; set;}
        public Child Child { get; set; }
        public Instructor Instructor { get; set; }
    }
}
