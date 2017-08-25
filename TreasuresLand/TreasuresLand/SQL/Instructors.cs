using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasuresLand.Objects;

namespace TreasuresLand.SQL
{
    public class Instructors
    {   
        private static string q_GetAllInsturctors = "SELECT * FROM Instructors";
        private static string q_AddInsturctor = "INSERT INTO Instructors (Name, Address, Phone, Qualifier, CV, Gender, Age) VALUES(@Name,@Address,@Phone,@Qualifier, @CV, @Gender, @Age); SELECT last_insert_rowid();";
        private static string q_EditInsturctor = "UPDATE Instructors SET Name = @Name, Address = @Address," +
                                        "Phone = @Phone, Qualifier = @Qualifier, CV=@CV,Gender=@Gender," +
                                        "Age=@Age WHERE Id = @Id";
        private static string q_RemoveInstructor = "DELETE FROM Instructors WHERE Id=@Id";
        //private static string q_GetInstructorCourses = "SELECT * FROM InstructorCourses INNER JOIN Courses ON InstructorCourses.CourseId=Courses.Id WHERE InstructorId=@InstructorId";

        private static string q_GetInstructorClasses = "SELECT * FROM InstructorClasses WHERE InstructorId=@InstructorId";
        private static string q_AddInstructorClasses = "INSERT INTO InstructorClasses VALUES(@InstructorId, @ClassId)";
        private static string q_ClearInstructorClasses = "DELETE FROM InstructorClasses WHERE InstructorId=@InstructorId";
        private static string q_RemoveInstructorClasses = "DELETE FROM InstructorClasses WHERE ClassId=@ClassId";

        private static string q_AddInstructorAbsence = "INSERT INTO InstructorAbsence VALUES (@InstructorId, @Month, @Hours)";
        private static string q_GetInstructorAbsence = "SELECT * FROM InstructorAbsence WHERE InstructorId=@InstructorId";
        private static string q_ClearInstructorAbsence = "DELETE FROM InstructorAbsence WHERE InstructorId=@InstructorId";


        //TODO: Note, this will cause problems if there's multiple rows with the same values.
        private static string q_GetPreviousInstructorSalaries = "SELECT * FROM InstructorSalary WHERE InstructorId=@InstructorId";
        private static string q_AddInstructorSalary = "INSERT INTO InstructorSalary VALUES(@InstructorId,@CourseId,@Salary, @Paid)";
        private static string q_PayInstructorSalary = "UPDATE InstructorSalary SET Paid=@Paid WHERE InstructorId=@InstructorId AND CourseId=@CourseId";
        private static string q_ClearInstructorSalaries = "DELETE FROM InstructorSalary WHERE InstructorId=@InstructorId";

        private enum InstructorColumns
        {
            Id,
            Name,
            Address,
            Phone,
            Qualifier,
            CV,
            Gender,
            Age,
        }

        private enum InstructorCoursesColumns
        {
            InstructorId,
            ClassId,
        }

        private enum InstructorAbsenceColumns
        {
            InstructorId,
            Month,
            Hours,
        }

        private enum InstructorSalaryColumns
        {
            InstructorId,
            CourseId,
            Salary,
            Paid,
        }

        public static List<Instructor> GetAllInstructors()
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<Instructor> ins = new List<Instructor>();
            using (var command = new SQLiteCommand(q_GetAllInsturctors, Connection.conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var temp = new Instructor()
                            {
                                Id = reader.GetInt32((int)InstructorColumns.Id),
                                Name = reader.GetString((int)InstructorColumns.Name),
                                Address = reader.GetString((int)InstructorColumns.Address),
                                Phone = reader.GetString((int)InstructorColumns.Phone),
                                Qualifier = reader.GetString((int)InstructorColumns.Qualifier),
                                Gender = reader.GetInt32((int)InstructorColumns.Gender),
                                Age = reader.GetInt32((int)InstructorColumns.Age),
                            };

                            try
                            {
                                temp.CV = (byte[])reader[(int)InstructorColumns.CV];
                            }
                            catch { }

                            temp.TeachingCourses = GetInstructorClasses(temp.Id);
                            ins.Add(temp);
                        }
                    }
                }
            }
            return ins;
        }

        public static void AddInstructor(Instructor newIns)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddInsturctor, Connection.conn))
            {
                command.Parameters.AddWithValue("@Name", newIns.Name);
                command.Parameters.AddWithValue("@Address", newIns.Address);
                command.Parameters.AddWithValue("@Phone", newIns.Phone);
                command.Parameters.AddWithValue("@Qualifier", newIns.Qualifier);
                command.Parameters.AddWithValue("@CV", newIns.CV);
                command.Parameters.AddWithValue("@Gender", newIns.Gender);
                command.Parameters.AddWithValue("@Age", newIns.Age);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    newIns.Id = reader.GetInt32(0);
                }
            }

            foreach (var val in newIns.TeachingCourses)
            {
                AddInstructorClass(newIns.Id, val.Id);
            }
        }

        public static void EditInstructor(Instructor newIns)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_EditInsturctor, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", newIns.Id);
                command.Parameters.AddWithValue("@Name", newIns.Name);
                command.Parameters.AddWithValue("@Address", newIns.Address);
                command.Parameters.AddWithValue("@Phone", newIns.Phone);
                command.Parameters.AddWithValue("@Qualifier", newIns.Qualifier);
                command.Parameters.AddWithValue("@CV", newIns.CV);
                command.Parameters.AddWithValue("@Gender", newIns.Gender);
                command.Parameters.AddWithValue("@Age", newIns.Age);
                command.ExecuteNonQuery();
            }

            ClearInstructorClasses(newIns.Id);

            foreach (var val in newIns.TeachingCourses)
            {
                AddInstructorClass(newIns.Id, val.Id);
            }
        }

        private static List<Class> GetInstructorClasses(int InstructorId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<Class> courses = new List<Class>();

            using (var command = new SQLiteCommand(q_GetInstructorClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            courses.Add(SQL.Classes.GetClass(reader.GetInt32((int)InstructorCoursesColumns.ClassId)));
                            //TODO: USE JOIN, this will optimize the time by A LOT.
                        }
                    }
                }
            }
            return courses;
        }

        public static void AddInstructorClass(int InstructorId, int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddInstructorClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.ExecuteNonQuery();
            }
        }

        internal static void ClearInstructorClasses(int InstructorId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_ClearInstructorClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.ExecuteNonQuery();
            }
        }

        public static void RemoveInstructorClass(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_RemoveInstructorClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.ExecuteNonQuery();
            }
        }

        public static void AddInstructorAbsence(int InstructorId, int Hours)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            var currentMonth = DateTime.Now.Date;
            using (var command = new SQLiteCommand(q_AddInstructorAbsence, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.Parameters.AddWithValue("@Month", currentMonth.Year + "/" + currentMonth.Month + "/1");
                command.Parameters.AddWithValue("@Hours", Hours);
                command.ExecuteNonQuery();
            }
        }

        public static int GetInstructorAbsence(int InstructorId, DateTime Month)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            int total = 0;

            using (var command = new SQLiteCommand(q_GetInstructorAbsence, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.Parameters.AddWithValue("@Month", Month.Year + "/" + Month.Month + "/1");
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            total += (int)reader[(int)InstructorAbsenceColumns.Hours];
                        }
                    }
                }
            }

            return total;

        }

        public static void ClearInstructorAbsence(int InstructorId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_ClearInstructorAbsence, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.ExecuteNonQuery();
            }

        }

        public static void RemoveInstructor(Instructor ins)
        {
            RemoveInstructor(ins.Id);
        }

        public static void RemoveInstructor(int InstructorId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            ClearInstructorAbsence(InstructorId);
            ClearInstructorClasses(InstructorId);
            ClearInstructorSalaries(InstructorId);

            using (var command = new SQLiteCommand(q_RemoveInstructor, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", InstructorId);
                command.ExecuteNonQuery();
            }
        }

        private static void ClearInstructorSalaries(int InstructorId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_ClearInstructorSalaries, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.ExecuteNonQuery();
            }
        }

        public static void AddInstructorSalary(int InstructorId, int CourseId, int Salary)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddInstructorSalary, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.Parameters.AddWithValue("@CourseId", CourseId);
                command.Parameters.AddWithValue("@Salary", Salary);
                command.Parameters.AddWithValue("@Paid", false);
                command.ExecuteNonQuery();
            }
        }

        public static List<Instructor._Salary> GetPreviousInstructorSalary(int InsId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            var salaries = new List<Instructor._Salary>();

            using (var command = new SQLiteCommand(q_GetPreviousInstructorSalaries, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InsId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var salary = new Instructor._Salary()
                            {
                                InstructorId = InsId,
                                CourseId = reader.GetInt32((int)InstructorSalaryColumns.CourseId),
                                Salary = reader.GetInt32((int)InstructorSalaryColumns.Salary),
                                Paid = reader.GetBoolean((int)InstructorSalaryColumns.Paid),
                            };
                            salaries.Add(salary);
                        }
                    }
                }
            }

            return salaries;
        }

        public static void PayInstructorSalary(int InstructorId, int CourseId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_PayInstructorSalary, Connection.conn))
            {
                command.Parameters.AddWithValue("@InstructorId", InstructorId);
                command.Parameters.AddWithValue("@CourseId", CourseId);
                command.Parameters.AddWithValue("@Paid", true);
                command.ExecuteNonQuery();
            }
        }

    }
}
