using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrNadaTreasureLand.Objects
{
    public class Filter
    {
        public string Filtered { get; set; }
        public string Property { get; set; }
        public string FilterType { get; set; }
        public string Data { get; set; }
        public string Action { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Filter other = (Filter)obj;

            // TODO: write your implementation of Equals() here
            return (other.Filtered == this.Filtered && other.Property == this.Property &&
                other.FilterType == this.FilterType && other.Data == this.Data &&
                other.Action == this.Action);
        }
    }
}
