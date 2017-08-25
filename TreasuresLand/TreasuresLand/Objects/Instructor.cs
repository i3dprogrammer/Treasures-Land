using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasuresLand.Objects
{
    public class Instructor
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

        public class _Salary
        {
            public int InstructorId { get; set; }
            public int CourseId { get; set; }
            public int Salary { get; set; }
            public bool Paid { get; set; }
        }

    }
}
