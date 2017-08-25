using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasuresLand.Objects
{
    public class Class
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int Number { get; set; }
        public List<String> DaysPerWeek { get; set; } = new List<string>();
        public List<DateTime> ShiftedDates { get; set; } = new List<DateTime>();
        public bool Full { get; set; }
        public bool Over { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int SessionsLeft { get; set; }

        /// <summary>
        /// The course containing this class.
        /// </summary>
        public Course Course { get; set; }
        public Instructor Instructor { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Class)obj;

            return (other.Id == this.Id && other.CourseId == this.CourseId && other.StartDate == this.StartDate &&
                other.Over == this.Over && other.Full == this.Full && other.Number == this.Number &&
                other.DaysPerWeek.SequenceEqual(this.DaysPerWeek) && other.ShiftedDates.SequenceEqual(this.ShiftedDates));
        }

        public void CalculateEndDate(int totalSessions)
        {
            if (StartDate == null)
                return;

            DateTime start = new DateTime(StartDate.Value.Ticks);
            int count = totalSessions;
            while (count > 0)
            {
                if (!ShiftedDates.Exists(x => x.Day == start.Day && x.Month == start.Month && x.Year == start.Year))
                {
                    string currentDayName = ((DayOfWeek)start.DayOfWeek).ToString();
                    if (DaysPerWeek.Contains(currentDayName))
                    {
                        count--;
                        if (count == 0)
                            EndDate = start;
                    }
                }
                start = start.AddDays(1);
            }

        }

        public void CalculateSessionsLeft(int totalSessions)
        {
            if (StartDate == null)
                return;

            DateTime start = new DateTime(StartDate.Value.Ticks);
            int ret = totalSessions;

            while (ret > 0 && start <= DateTime.Now)
            {
                if (!ShiftedDates.Exists(x => x.Day == start.Day && x.Month == start.Month && x.Year == start.Year))
                {
                    string currentDayName = ((DayOfWeek)start.DayOfWeek).ToString();
                    if (DaysPerWeek.Contains(currentDayName))
                        ret--;
                }
                start = start.AddDays(1);
            }

            SessionsLeft = ret;
        }

    }
}
