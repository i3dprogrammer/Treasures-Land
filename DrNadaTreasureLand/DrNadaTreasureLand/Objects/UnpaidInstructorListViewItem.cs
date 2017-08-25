using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrNadaTreasureLand.Objects
{
    class UnpaidInstructorListViewItem
    {
        public TreasuresLand.Objects.Instructor Instructor { get; set; }
        public TreasuresLand.Objects.Course Course { get; set; }
        public int Salary { get; set; }

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

            UnpaidInstructorListViewItem other = (UnpaidInstructorListViewItem)obj;

            return (other.Instructor == this.Instructor && other.Course == this.Course && other.Salary == this.Salary);
        }

    }
}
