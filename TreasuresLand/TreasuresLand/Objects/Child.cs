using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasuresLand.Objects
{
    public class Child
    {
        public bool Active { get; set; }
        public int Id{get;set;}
        public string Name{get;set;}
        public int Age { get; set; }
        public int Gender { get; set; }
        public string WhatsAppPhone{get;set;}
        public int AcademicYear{get;set;}
        public int EducationType{get;set;}
        public int ChildBirthOrder{get;set;}
        public string ChildTraits{get;set;}
        public string ChildHandling{get;set;}
        public string ChildFreeTime{get;set;}
        public string GuardianName{get;set;}
        public int GuardianType{get;set;}
        public string MotherPhone{get;set;}
        public string FatherPhone{get;set;}
        public string MotherJob{get;set;}
        public string FatherJob{get;set;}
        public string MotherQualifier{get;set;}
        public List<Class> RegisteredCourses { get; set; } = new List<Class>();

        public bool Equals(Child other)
        {
            bool test1 = (other.Id == this.Id && other.Name == this.Name &&
                other.Age == this.Age && other.Gender == this.Gender &&
                other.WhatsAppPhone == this.WhatsAppPhone && other.AcademicYear == this.AcademicYear &&
                other.EducationType == this.EducationType && other.ChildBirthOrder == this.ChildBirthOrder &&
                other.ChildTraits == this.ChildTraits && other.ChildHandling == this.ChildHandling &&
                other.ChildFreeTime == this.ChildFreeTime && other.GuardianName == this.GuardianName &&
                other.GuardianType == this.GuardianType && other.MotherPhone == this.MotherPhone &&
                other.FatherPhone == this.FatherPhone && other.MotherJob == this.MotherJob &&
                other.FatherJob == this.FatherJob && other.MotherQualifier == this.MotherQualifier);
            bool test2 = other.RegisteredCourses.SequenceEqual(this.RegisteredCourses);

            return test1 && test2;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here
            return Equals((Child)obj);
        }
    }
}
