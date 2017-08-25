using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TreasuresLand.Objects;
using System.Data.SQLite;

namespace TreasuresLand.SQL
{
    public class Courses
    {
        private static string q_GetAllCourses = " SELECT * FROM Courses";
        private static string q_GetCourse = "SELECT * FROM Courses WHERE Id=@Id";
        //private static string q_AddCourse = "INSERT INTO Courses VALUES(@Id,@Name,@AcademicYearStart,@AcademicYearEnd,@InstructorsCount,@Cost,@Full,@Over)";
        private static string q_AddCourse = "INSERT INTO Courses (Name,AcademicYearStart,AcademicYearEnd,Cost,PricePerChild,Level,TotalSessions,SessionDuration) VALUES (@Name,@AcademicYearStart,@AcademicYearEnd,@Cost,@PricePerChild,@Level,@TotalSessions,@SessionDuration); SELECT last_insert_rowid();";
        private static string q_EditCourse = "UPDATE Courses SET Name = @Name, AcademicYearStart = @AcademicYearStart, AcademicYearEnd=@AcademicYearEnd," +
                                        "Cost = @Cost, Level=@Level,PricePerChild=@PricePerChild," +
                                        "TotalSessions=@TotalSessions, SessionDuration=@SessionDuration WHERE Id = @Id";
        private static string q_DeleteCourse = "DELETE FROM Courses WHERE Id=@Id";

        private enum CourseColumns
        {
            Id,
            Name,
            AcademicYearStart,
            AcademicYearEnd,
            Cost,
            PricePerChild,
            Level,
            TotalSessions,
            SessionDuration,
        }

        public static List<Course> GetAllCourses()
        {
            Console.WriteLine("Fetching courses!");
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<Course> courses = new List<Course>();

            using (var command = new SQLiteCommand(q_GetAllCourses, Connection.conn))
            {
                using (var reader = command.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var temp = new Course()
                            {
                                Id = reader.GetInt32((int)CourseColumns.Id),
                                Name = reader.GetString((int)CourseColumns.Name),
                                AcademicYearStart = reader.GetInt32((int)CourseColumns.AcademicYearStart),
                                AcademicYearEnd = reader.GetInt32((int)CourseColumns.AcademicYearEnd),
                                Cost = reader.GetInt32((int)CourseColumns.Cost),
                                PricePerChild = reader.GetInt32((int)CourseColumns.PricePerChild),
                                Level = reader.GetInt32((int)CourseColumns.Level),
                                TotalSessions = reader.GetInt32((int)CourseColumns.TotalSessions),
                                SessionDuration = reader.GetInt32((int)CourseColumns.SessionDuration),
                            };
                            temp.Classes = Classes.GetCourseClasses(temp);
                            //TODO: Calculate the following when getting the classes -Optimization
                            temp.Classes.ForEach(x => x.CalculateEndDate(temp.TotalSessions)); 
                            temp.Classes.ForEach(x => x.CalculateSessionsLeft(temp.TotalSessions));
                            courses.Add(temp);
                        }
                    }
                }
            }

            return courses;
        }

        public static Course GetCourse(int CourseId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_GetCourse, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", CourseId);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    var temp = new Course()
                    {
                        Id = reader.GetInt32((int)CourseColumns.Id),
                        Name = reader.GetString((int)CourseColumns.Name),
                        AcademicYearStart = reader.GetInt32((int)CourseColumns.AcademicYearStart),
                        AcademicYearEnd = reader.GetInt32((int)CourseColumns.AcademicYearEnd),
                        Cost = reader.GetInt32((int)CourseColumns.Cost),
                        PricePerChild = reader.GetInt32((int)CourseColumns.PricePerChild),
                        Level = reader.GetInt32((int)CourseColumns.Level),
                        TotalSessions = reader.GetInt32((int)CourseColumns.TotalSessions),
                        SessionDuration = reader.GetInt32((int)CourseColumns.SessionDuration),
                    };
                    temp.Classes = Classes.GetCourseClasses(temp);
                    //TODO: Calculate the following when getting the classes -Optimization
                    temp.Classes.ForEach(x => x.CalculateEndDate(temp.TotalSessions));
                    temp.Classes.ForEach(x => x.CalculateSessionsLeft(temp.TotalSessions));
                    return temp;
                }
            }
            return null;
        }

        public static Course AddCourse(Course newCourse)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddCourse, Connection.conn))
            {
                command.Parameters.AddWithValue("@Name", newCourse.Name);
                command.Parameters.AddWithValue("@AcademicYearStart", newCourse.AcademicYearStart);
                command.Parameters.AddWithValue("@AcademicYearEnd", newCourse.AcademicYearEnd);
                command.Parameters.AddWithValue("@Cost", newCourse.Cost);
                command.Parameters.AddWithValue("@PricePerChild", newCourse.PricePerChild);
                command.Parameters.AddWithValue("@Level", newCourse.Level);
                command.Parameters.AddWithValue("@TotalSessions", newCourse.TotalSessions);
                command.Parameters.AddWithValue("@SessionDuration", newCourse.SessionDuration);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    newCourse.Id = reader.GetInt32(0);
                }
            }
            return newCourse;
        }

        public static void EditCourse(Course newCourse)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_EditCourse, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", newCourse.Id);
                command.Parameters.AddWithValue("@Name", newCourse.Name);
                command.Parameters.AddWithValue("@AcademicYearStart", newCourse.AcademicYearStart);
                command.Parameters.AddWithValue("@AcademicYearEnd", newCourse.AcademicYearEnd);
                command.Parameters.AddWithValue("@Cost", newCourse.Cost);
                command.Parameters.AddWithValue("@PricePerChild", newCourse.PricePerChild);
                command.Parameters.AddWithValue("@Level", newCourse.Level);
                command.Parameters.AddWithValue("@TotalSessions", newCourse.TotalSessions);
                command.Parameters.AddWithValue("@SessionDuration", newCourse.SessionDuration);
                command.ExecuteNonQuery();
            }
        }

        public static void RemoveCourse(Course course)
        {
            RemoveCourse(course.Id);
        }

        public static void RemoveCourse(int CourseId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            Classes.ClearCourseClasses(CourseId);
            Instructors.RemoveInstructorClass(CourseId);
            Children.RemoveChildClass(CourseId);

            using (var command = new SQLiteCommand(q_DeleteCourse, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", CourseId);
                command.ExecuteNonQuery();
            }
        }




    }
}
