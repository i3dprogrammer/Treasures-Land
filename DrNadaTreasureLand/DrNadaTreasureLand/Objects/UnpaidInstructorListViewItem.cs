using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasuresLand.Objects;

namespace DrNadaTreasureLand.Objects
{
    class UnpaidInstructorListViewItem1
    {
        public TreasuresLand.Objects.Instructor Instructor { get; set; }
        public TreasuresLand.Objects.Course Course { get; set; }
        public int Salary { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            UnpaidInstructorListViewItem1 other = (UnpaidInstructorListViewItem1)obj;

            return (other.Instructor == this.Instructor && other.Course == this.Course && other.Salary == this.Salary);
        }

        public override int GetHashCode()
        {
            var hashCode = -1310140427;
            hashCode = hashCode * -1521134295 + EqualityComparer<Instructor>.Default.GetHashCode(Instructor);
            hashCode = hashCode * -1521134295 + EqualityComparer<Course>.Default.GetHashCode(Course);
            hashCode = hashCode * -1521134295 + Salary.GetHashCode();
            return hashCode;
        }
    }
}
