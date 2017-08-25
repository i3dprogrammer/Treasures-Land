using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasuresLand.Objects;

namespace TreasuresLand.SQL
{
    public class Children
    {
        private static string q_GetAllChildren = "SELECT * FROM Children";
        private static string q_AddChild = "INSERT INTO Children (Name,Age,Gender,WhatsAppPhone,AcademicYear,EducationType,ChildBirthOrder,ChildTraits,ChildHandling,ChildFreeTime,GuardianName,GuardianType,MotherPhone,FatherPhone,MotherJob,FatherJob,MotherQualifier) VALUES (@Name,@Age,@Gender,@WhatsAppPhone,@AcademicYear,@EducationType,@ChildBirthOrder,@ChildTraits,@ChildHandling,@ChildFreeTime,@GuardianName,@GuardianType,@MotherPhone,@FatherPhone,@MotherJob,@FatherJob,@MotherQualifier); SELECT last_insert_rowid();";
        private static string q_EditChild = "UPDATE Children SET Name = @Name, Age=@Age, Gender=@Gender, WhatsAppPhone = @WhatsAppPhone," +
                                        "AcademicYear = @AcademicYear, EducationType = @EducationType," +
                                        "ChildBirthOrder = @ChildBirthOrder, ChildTraits = @ChildTraits," +
                                        "ChildHandling = @ChildHandling, ChildFreeTime = @ChildFreeTime," +
                                        "GuardianName = @GuardianName, GuardianType = @GuardianType," +
                                        "MotherPhone = @MotherPhone, FatherPhone = @FatherPhone," +
                                        "MotherJob = @MotherJob, FatherJob = @FatherJob, MotherQualifier=@MotherQualifier WHERE Id = @Id";
        private static string q_DeleteChild = "DELETE FROM Children WHERE Id=@Id";
        
        private static string q_GetChildClasses = "SELECT * FROM ChildrenClasses WHERE ChildId=@ChildId";
        private static string q_AddChildClass = "INSERT INTO ChildrenClasses VALUES(@ChildId, @ClassId)";
        private static string q_ClearChildClasses = "DELETE FROM ChildrenClasses WHERE ChildId=@ChildId";

        private static string q_RemoveChildClass = "DELETE FROM ChildrenClasses WHERE ClassId=@ClassId";

        private enum ChildColumns
        {
            Active,
            Id,
            Name,
            Age,
            Gender,
            WhatsAppPhone,
            AcademicYear,
            EducationType,
            ChildBirthOrder,
            ChildTraits,
            ChildHandling,
            ChildFreeTime,
            GuardianName,
            GuardianType,
            MotherPhone,
            FatherPhone,
            MotherJob,
            FatherJob,
            MotherQualifier,

            //s_Attendance=2,
            //c_Id,
            //c_Name,
            //c_AcademicYearStart,
            //c_AcademicYearEnd,
            //c_InstructorCount,
            //c_Cost,
            //c_Full,
            //c_Over,
        }

        private enum ChildrenCoursesColumns
        {
            ChildId,
            ClassId,
            Attendance,
        }

        public static List<Child> GetAllChildren()
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<Child> ins = new List<Child>();
            using (var command = new SQLiteCommand(q_GetAllChildren, Connection.conn))
            {
                using (var reader = command.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var temp = new Child()
                            {
                                Id = reader.GetInt32((int)ChildColumns.Id),
                                Name = reader.GetString((int)ChildColumns.Name),
                                Age = reader.GetInt32((int)ChildColumns.Age),
                                Gender = reader.GetInt32((int)ChildColumns.Gender),
                                WhatsAppPhone = reader.GetString((int)ChildColumns.WhatsAppPhone),
                                AcademicYear = reader.GetInt32((int)ChildColumns.AcademicYear),
                                EducationType = reader.GetInt32((int)ChildColumns.EducationType),
                                ChildBirthOrder = reader.GetInt32((int)ChildColumns.ChildBirthOrder),
                                ChildTraits = reader.GetString((int)ChildColumns.ChildTraits),
                                ChildHandling = reader.GetString((int)ChildColumns.ChildHandling),
                                ChildFreeTime = reader.GetString((int)ChildColumns.ChildFreeTime),
                                GuardianName = reader.GetString((int)ChildColumns.GuardianName),
                                GuardianType = reader.GetInt32((int)ChildColumns.GuardianType),
                                MotherPhone = reader.GetString((int)ChildColumns.MotherPhone),
                                FatherPhone = reader.GetString((int)ChildColumns.FatherPhone),
                                MotherJob = reader.GetString((int)ChildColumns.MotherJob),
                                FatherJob = reader.GetString((int)ChildColumns.FatherJob),
                                MotherQualifier = reader.GetString((int)ChildColumns.MotherQualifier),
                            };
                            temp.RegisteredCourses = GetChildClasses(temp.Id);
                            ins.Add(temp);
                        }
                    }
                }
            }
            return ins;
        }

        public static void AddChild(Child newChild) //TODO INSERT for corresponding Courses he has
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using(var command = new SQLiteCommand(q_AddChild, Connection.conn)){
                command.Parameters.AddWithValue("@Name", newChild.Name);
                command.Parameters.AddWithValue("@Age", newChild.Age);
                command.Parameters.AddWithValue("@Gender", newChild.Gender);
                command.Parameters.AddWithValue("@WhatsAppPhone", newChild.WhatsAppPhone);
                command.Parameters.AddWithValue("@AcademicYear", newChild.AcademicYear);
                command.Parameters.AddWithValue("@EducationType", newChild.EducationType);
                command.Parameters.AddWithValue("@ChildBirthOrder", newChild.ChildBirthOrder);
                command.Parameters.AddWithValue("@ChildTraits", newChild.ChildTraits);
                command.Parameters.AddWithValue("@ChildHandling", newChild.ChildHandling);
                command.Parameters.AddWithValue("@ChildFreeTime", newChild.ChildFreeTime);
                command.Parameters.AddWithValue("@GuardianName", newChild.GuardianName);
                command.Parameters.AddWithValue("@GuardianType", newChild.GuardianType);
                command.Parameters.AddWithValue("@MotherPhone", newChild.MotherPhone);
                command.Parameters.AddWithValue("@FatherPhone", newChild.FatherPhone);
                command.Parameters.AddWithValue("@MotherJob", newChild.MotherJob);
                command.Parameters.AddWithValue("@FatherJob", newChild.FatherJob);
                command.Parameters.AddWithValue("@MotherQualifier", newChild.MotherQualifier);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    newChild.Id = reader.GetInt32(0);
                }
            }

            foreach (var val in newChild.RegisteredCourses)
            {
                AddChildClass(newChild.Id, val.Id);
            }
        }

        public static void EditChild(Child newChild) //TODO check changes for corresponding Courses he has
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_EditChild, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", newChild.Id);
                command.Parameters.AddWithValue("@Name", newChild.Name);
                command.Parameters.AddWithValue("@Age", newChild.Age);
                command.Parameters.AddWithValue("@Gender", newChild.Gender);
                command.Parameters.AddWithValue("@WhatsAppPhone", newChild.WhatsAppPhone);
                command.Parameters.AddWithValue("@AcademicYear", newChild.AcademicYear);
                command.Parameters.AddWithValue("@EducationType", newChild.EducationType);
                command.Parameters.AddWithValue("@ChildBirthOrder", newChild.ChildBirthOrder);
                command.Parameters.AddWithValue("@ChildTraits", newChild.ChildTraits);
                command.Parameters.AddWithValue("@ChildHandling", newChild.ChildHandling);
                command.Parameters.AddWithValue("@ChildFreeTime", newChild.ChildFreeTime);
                command.Parameters.AddWithValue("@GuardianName", newChild.GuardianName);
                command.Parameters.AddWithValue("@GuardianType", newChild.GuardianType);
                command.Parameters.AddWithValue("@MotherPhone", newChild.MotherPhone);
                command.Parameters.AddWithValue("@FatherPhone", newChild.FatherPhone);
                command.Parameters.AddWithValue("@MotherJob", newChild.MotherJob);
                command.Parameters.AddWithValue("@FatherJob", newChild.FatherJob);
                command.Parameters.AddWithValue("@MotherQualifier", newChild.MotherQualifier);
                command.ExecuteNonQuery();
            }

            ClearChildClasses(newChild.Id);

            foreach (var val in newChild.RegisteredCourses)
            {
                AddChildClass(newChild.Id, val.Id);
            }
        }

        public static void RemoveChild(Child child)
        {
            RemoveChild(child.Id);
        }

        public static void RemoveChild(int ChildId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            ClearChildClasses(ChildId);

            using (var command = new SQLiteCommand(q_DeleteChild, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", ChildId);
                command.ExecuteNonQuery();
            }
        }

        private static List<Class> GetChildClasses(int ChildId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<Class> courses = new List<Class>();

            using (var command = new SQLiteCommand(q_GetChildClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@ChildId", ChildId);
                using (var reader = command.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var course = Classes.GetClass(reader.GetInt32((int)ChildrenCoursesColumns.ClassId));
                            //TODO: USE JOIN, Get Course.Sessions list also --> this will optimize the time by A LOT.
                            courses.Add(course);
                        }
                    }
                }
            }
            return courses;
        }

        public static void AddChildClass(int ChildId, int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddChildClass, Connection.conn))
            {
                command.Parameters.AddWithValue("@ChildId", ChildId);
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.ExecuteNonQuery();
            }
        }

        private static void ClearChildClasses(int ChildId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_ClearChildClasses, Connection.conn))
            {
                command.Parameters.AddWithValue("@ChildId", ChildId);
                command.ExecuteNonQuery();
            }
        }

        public static void RemoveChildClass(int ClassId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_RemoveChildClass, Connection.conn))
            {
                command.Parameters.AddWithValue("@ClassId", ClassId);
                command.ExecuteNonQuery();
            }
        }
    }
}
