using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasuresLand.Objects;

namespace TreasuresLand.SQL
{
    public class Classes
    {

        private static string q_GetClass = "SELECT * FROM CourseClasses WHERE Id=@ClassId";
        private static string q_GetCourseClasses = "SELECT * FROM CourseClasses WHERE CourseId=@CourseId";
        private static string q_AddCourseClass = "INSERT INTO CourseClasses (Number, StartDate,StartTime,EndTime, [Full], [Over], CourseId) VALUES (@Number, @StartDate,@StartTime,@EndTime,@Full,@Over,@CourseId); SELECT last_insert_rowid();";
        private static string q_EditCourseClass = "UPDATE CourseClasses SET [Full]=@Full, [Over]=@Over, StartDate=@StartDate,StartTime=@StartTime,EndTime=@EndTime WHERE Id=@ClassId";
        private static string q_ClearCourseClasses = "DELETE FROM CourseClasses WHERE CourseId=@CourseId";
        private static string q_DeleteClass = "DELETE FROM CourseClasses WHERE Id=@Id";

        private static string q_GetClassSessions = "SELECT * FROM ClassSessions WHERE ClassId=@ClassId";
        private static string q_AddClassSession = "INSERT INTO ClassSessions (ClassId, SessionDay) VALUES (@ClassId,@SessionDay);";
        private static string q_ClearClassSessions = "DELETE FROM ClassSessions WHERE ClassId=@ClassId";

        private static string q_GetClassShiftedDates = "SELECT * FROM ClassShiftedDays WHERE ClassId=@ClassId";
        private static string q_AddClassShiftedDate = "INSERT INTO ClassShiftedDays VALUES (@ClassId, @ShiftedDate);";
        private static string q_ClearClassShiftedDates = "DELETE FROM ClassShiftedDays WHERE ClassId=@ClassId";

        private enum CourseClassesColumns
        {
            Id,
            Number,
            StartDate,
            StartTime,
            EndTime,
            Full,
            Over,
            CourseId,
        }

        private enum ClassSessionsColumns
        {
            CourseId,
            SessionDay,
        }

        private enum ClassShiftedDatesColumns
        {
            CourseId,
            ShiftedDate,
        }

        internal static List<Class> GetCourseClasses(Course newCourse)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<Class> courseClasses = new List<Class>();

            using (var command = new SQLiteCommand(q_GetCourseClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@CourseId", newCourse.Id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var temp = new Class()
                            {
                                Id = reader.GetInt32((int)CourseClassesColumns.Id),
                                Number = reader.GetInt32((int)CourseClassesColumns.Number),
                                Full = reader.GetBoolean((int)CourseClassesColumns.Full),
                                Over = reader.GetBoolean((int)CourseClassesColumns.Over),
                                CourseId = reader.GetInt32((int)CourseClassesColumns.CourseId),
                                Course = newCourse,
                            };

                            try
                            {
                                temp.StartDate = reader.GetDateTime((int)CourseClassesColumns.StartDate);
                            }
                            catch (Exception) { }

                            try
                            {
                                temp.StartTime = reader.GetDateTime((int)CourseClassesColumns.StartTime).TimeOfDay;
                                temp.EndTime = reader.GetDateTime((int)CourseClassesColumns.EndTime).TimeOfDay;
                            } catch (Exception) { }

                            temp.DaysPerWeek = GetClassSessions(temp.Id);
                            temp.ShiftedDates = GetClassShiftedDates(temp.Id);
                            courseClasses.Add(temp);
                        }
                    }
                }
            }

            return courseClasses;
        }

        internal static Class GetClass(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_GetClass, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var temp = new Class()
                            {
                                Id = reader.GetInt32((int)CourseClassesColumns.Id),
                                Number = reader.GetInt32((int)CourseClassesColumns.Number),
                                Full = reader.GetBoolean((int)CourseClassesColumns.Full),
                                Over = reader.GetBoolean((int)CourseClassesColumns.Over),
                                CourseId = reader.GetInt32((int)CourseClassesColumns.CourseId),
                            };

                            try
                            {
                                temp.StartDate = reader.GetDateTime((int)CourseClassesColumns.StartDate);
                            }
                            catch (Exception) { }

                            try
                            {
                                temp.StartTime = reader.GetDateTime((int)CourseClassesColumns.StartTime).TimeOfDay;
                                temp.EndTime = reader.GetDateTime((int)CourseClassesColumns.EndTime).TimeOfDay;
                            }
                            catch (Exception) { }

                            temp.Course = Courses.GetCourse(temp.CourseId);
                            temp.DaysPerWeek = GetClassSessions(temp.Id);
                            temp.ShiftedDates = GetClassShiftedDates(temp.Id);
                            return temp;
                        }
                    }
                }
            }
            return null;
        }

        public static Class AddCourseClass(Class newClass)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddCourseClass, Connection.conn))
            {
                command.Parameters.AddWithValue("@StartDate", newClass.StartDate);
                command.Parameters.AddWithValue("@StartTime", newClass.StartTime);
                command.Parameters.AddWithValue("@EndTime", newClass.EndTime);
                command.Parameters.AddWithValue("@Number", newClass.Number);
                command.Parameters.AddWithValue("@Full", newClass.Full);
                command.Parameters.AddWithValue("@Over", newClass.Over);
                command.Parameters.AddWithValue("@CourseId", newClass.CourseId);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    newClass.Id = reader.GetInt32((int)CourseClassesColumns.Id);
                }
            }

            foreach (var el in newClass.DaysPerWeek)
            {
                AddClassSession(el, newClass.Id);
            }

            return newClass;
        }

        public static void EditCourseClass(Class newClass)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_EditCourseClass, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", newClass.Id);
                command.Parameters.AddWithValue("@StartDate", newClass.StartDate);
                command.Parameters.AddWithValue("@StartTime", newClass.StartTime);
                command.Parameters.AddWithValue("@EndTime", newClass.EndTime);
                command.Parameters.AddWithValue("@Number", newClass.Number);
                command.Parameters.AddWithValue("@Full", newClass.Full);
                command.Parameters.AddWithValue("@Over", newClass.Over);
                command.ExecuteNonQuery();
            }

            ClearClassSessions(newClass.Id);

            foreach(var item in newClass.DaysPerWeek)
            {
                AddClassSession(item, newClass.Id);
            }
        }


        internal static void ClearCourseClasses(int CourseId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            foreach (var item in GetCourseClasses(Courses.GetCourse(CourseId)))
            {
                ClearClassSessions(item.Id);
                ClearClassShiftedDates(item.Id);
            }

            using (var command = new SQLiteCommand(q_ClearCourseClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@CourseId", CourseId);
                command.ExecuteNonQuery();
            }
        }

        public static void RemoveCourseClass(Class removedClass)
        {
            RemoveCourseClass(removedClass.Id);
        }

        public static void RemoveCourseClass(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            Children.RemoveChildClass(ClassId);
            Instructors.RemoveInstructorClass(ClassId);

            using (var command = new SQLiteCommand(q_DeleteClass, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", ClassId);
                command.ExecuteNonQuery();
            }
        }

        private static List<String> GetClassSessions(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<String> daysPerWeek = new List<String>();

            using (var command = new SQLiteCommand(q_GetClassSessions, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            daysPerWeek.Add(reader.GetString((int)ClassSessionsColumns.SessionDay));
                        }
                    }
                }
            }

            return daysPerWeek;
        }

        private static void AddClassSession(string SessionDay, int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddClassSession, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.Parameters.AddWithValue("@SessionDay", SessionDay);
                command.ExecuteNonQuery();
            }

        }

        private static void ClearClassSessions(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_ClearClassSessions, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.ExecuteNonQuery();
            }
        }

        private static List<DateTime> GetClassShiftedDates(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            var shiftedDates = new List<DateTime>();

            using (var command = new SQLiteCommand(q_GetClassShiftedDates, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            shiftedDates.Add(reader.GetDateTime((int)ClassShiftedDatesColumns.ShiftedDate));
                        }
                    }
                }
            }

            return shiftedDates;
        }

        public static void AddClassShiftedDate(int ClassId, DateTime ShiftedDate)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddClassShiftedDate, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.Parameters.AddWithValue("@ShiftedDate", ShiftedDate);
                command.ExecuteNonQuery();
            }
        }

        public static void ClearClassShiftedDates(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_ClearClassShiftedDates, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.ExecuteNonQuery();
            }
        }


    }
}
