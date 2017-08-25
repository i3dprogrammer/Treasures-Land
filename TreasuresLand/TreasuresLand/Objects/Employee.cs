using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasuresLand.Objects
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string NationalId { get; set; }
        public string Qualifier { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        /*
        public int Salary;
        public int DailyWorkingHours;
        public string Attendance;*/

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

            var other = (Employee)obj;

            return (other.Id == this.Id && other.Name == this.Name && other.Address == this.Address &&
                other.Phone == this.Phone && other.NationalId == this.NationalId && other.Age == this.Age &&
                other.Qualifier == this.Qualifier && other.Gender == this.Gender);
        }
    }
}
