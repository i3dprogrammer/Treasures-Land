using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasuresLand.Objects
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Course
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AcademicYearStart { get; set; }
        public int AcademicYearEnd { get; set; }
        public int Cost { get; set; }
        public int PricePerChild { get; set; }
        //public bool Full { get; set; }
        //public bool Over { get; set; }
        public int Level { get; set; }
        //public DateTime StartDate { get; set; }
        public List<Class> Classes { get; set; } = new List<Class>();
        //public int ShiftedDays { get; set; }
        //public List<String> DaysPerWeek { get; set; } = new List<string>();
        //public List<DateTime> ShiftedDates { get; set; }
        public int TotalSessions { get; set; }
        public int SessionDuration { get; set; } 

        public Course()
        {

        }

        public Course(Course c)
        {
            Id = c.Id;
            Name = c.Name;
            AcademicYearEnd = c.AcademicYearEnd;
            AcademicYearStart = c.AcademicYearStart;
            Cost = c.Cost;
            PricePerChild = c.PricePerChild;
            //Full = c.Full;
            //Over = c.Over;
            Level = c.Level;
            //StartDate = c.StartDate; //FIXME: not really sure if this is OK.
            //EndDate = c.EndDate; //FIXME: not really sure if this is OK.
            //ShiftedDays = c.ShiftedDays;
            //c.DaysPerWeek.ForEach(x => DaysPerWeek.Add(x));
            TotalSessions = c.TotalSessions;
            Classes = c.Classes;
            SessionDuration = c.SessionDuration;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Course other = (Course)obj;

            return (other.Id == this.Id && other.Name == this.Name && other.AcademicYearEnd == this.AcademicYearEnd &&
                other.AcademicYearStart == this.AcademicYearStart && other.PricePerChild == this.PricePerChild &&
                other.Cost == this.Cost && other.Level == this.Level &&
                other.TotalSessions == this.TotalSessions && other.Classes.SequenceEqual(this.Classes)); 
                /*&& other.DaysPerWeek.SequenceEqual(this.DaysPerWeek) &&
                other.ShiftedDays == this.ShiftedDays) && other.ShiftedDates.SequenceEqual(this.ShiftedDates);*/
        }
    }
}
