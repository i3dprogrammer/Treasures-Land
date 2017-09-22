using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasuresLand.Objects
{
    public class Instructor //TODO: No fucking idea why the salary isnt a list here..
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Qualifier { get; set; }
        public byte[] CV { get; set; } = new byte[] { };
        public int Gender { get; set; }
        public int Age { get; set; }
        public int SalaryPerChild { get; set; }
        public List<Class> TeachingCourses { get; set; } = new List<Class>();
        public bool SalaryPaid { get; set; } = true;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Instructor)obj;

            // TODO: write your implementation of Equals() here
            return (other.Id == this.Id && other.Name == this.Name && other.Address == this.Address &&
                other.Phone == this.Phone && other.Qualifier == this.Qualifier &&
                other.Gender == this.Gender && other.SalaryPerChild == this.SalaryPerChild && other.Age == this.Age && other.CV.SequenceEqual(this.CV) &&
                other.TeachingCourses.SequenceEqual(this.TeachingCourses));
        }

        public override int GetHashCode()
        {
            var hashCode = -1000165297;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Phone);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Qualifier);
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(CV);
            hashCode = hashCode * -1521134295 + Gender.GetHashCode();
            hashCode = hashCode * -1521134295 + Age.GetHashCode();
            hashCode = hashCode * -1521134295 + SalaryPerChild.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Class>>.Default.GetHashCode(TeachingCourses);
            return hashCode;
        }

        public class _Salary
        {
            public int InstructorId { get; set; }
            public int CourseId { get; set; }
            public int Salary { get; set; }
            public bool Paid { get; set; }
        }

    }
}
