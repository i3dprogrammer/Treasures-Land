using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasuresLand.Objects;

namespace DrNadaTreasureLand.Objects
{
    class InstructorSalaryListViewItem
    {
        public bool Paid { get; set; }
        public int Salary { get; set; }
        public Instructor Instructor { get; set; }
        public Course Course { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (InstructorSalaryListViewItem)obj;
            return other.Paid == this.Paid && other.Salary == this.Salary && other.Instructor == this.Instructor && other.Course == this.Course;
        }

        public override int GetHashCode()
        {
            var hashCode = 903062700;
            hashCode = hashCode * -1521134295 + Paid.GetHashCode();
            hashCode = hashCode * -1521134295 + Salary.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Instructor>.Default.GetHashCode(Instructor);
            return hashCode;
        }
    }
}
