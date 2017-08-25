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
    public class Employees
    {
        private static string q_GetAllEmployees = "SELECT * FROM Employees";
        private static string q_AddEmployee = "INSERT INTO Employees (Name, Address, Phone, NationalId, Qualifier, Gender, Age) VALUES(@Name, @Address, @Phone, @NationalId, @Qualifier, @Gender, @Age); SELECT last_insert_rowid();";
        private static string q_EditEmployee = "UPDATE Employees SET Name = @Name, Address = @Address," +
                                        "Phone = @Phone, NationalId = @NationalId, Qualifier=@Qualifier," + 
                                        "Gender=@Gender, Age=@Age WHERE Id = @Id";
        private static string q_RemoveEmployee = "DELETE FROM Employees WHERE Id=@Id";

        private enum EmployeeColumns
        {
            Id,
            Name,
            Address,
            Phone,
            NationalId,
            Qualifier,
            Gender,
            Age,
        }

        public static List<Employee> GetAllEmployees()
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            List<Employee> emps = new List<Employee>();
            using (var command = new SQLiteCommand(q_GetAllEmployees, Connection.conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var temp = new Employee()
                            {
                                Id = reader.GetInt32((int)EmployeeColumns.Id),
                                Name = reader.GetString((int)EmployeeColumns.Name),
                                Address = reader.GetString((int)EmployeeColumns.Address),
                                Phone = reader.GetString((int)EmployeeColumns.Phone),
                                NationalId = reader.GetString((int)EmployeeColumns.NationalId),
                                Qualifier = reader.GetString((int)EmployeeColumns.Qualifier),
                                Gender = reader.GetInt32((int)EmployeeColumns.Gender),
                                Age = reader.GetInt32((int)EmployeeColumns.Age),
                                /*Salary = reader.GetInt32((int)EmployeeColumns.Salary),
                                DailyWorkingHours = reader.GetInt32((int)EmployeeColumns.DailyWorkingHours),
                                Attendance = reader.GetString((int)EmployeeColumns.Attendance)*/
                            };
                            emps.Add(temp);
                        }
                    }
                }
            }
            return emps;
        }

        public static void AddEmployee(Employee newEmployee)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_AddEmployee, Connection.conn))
            {
                command.Parameters.AddWithValue("@Name", newEmployee.Name);
                command.Parameters.AddWithValue("@Address", newEmployee.Address);
                command.Parameters.AddWithValue("@Phone", newEmployee.Phone);
                command.Parameters.AddWithValue("@NationalId", newEmployee.NationalId);
                command.Parameters.AddWithValue("@Qualifier", newEmployee.Qualifier);
                command.Parameters.AddWithValue("@Gender", newEmployee.Gender);
                command.Parameters.AddWithValue("@Age", newEmployee.Age);
                /*command.Parameters.AddWithValue("@Salary", newEmployee.Salary);
                command.Parameters.AddWithValue("@DailyWorkingHours", newEmployee.DailyWorkingHours);
                command.Parameters.AddWithValue("@Attendance", newEmployee.Attendance);*/
                command.ExecuteNonQuery();
            }
        }

        public static void EditEmployee(Employee newEmployee)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_EditEmployee, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", newEmployee.Id);
                command.Parameters.AddWithValue("@Name", newEmployee.Name);
                command.Parameters.AddWithValue("@Address", newEmployee.Address);
                command.Parameters.AddWithValue("@Phone", newEmployee.Phone);
                command.Parameters.AddWithValue("@NationalId", newEmployee.NationalId);
                command.Parameters.AddWithValue("@Qualifier", newEmployee.Qualifier);
                command.Parameters.AddWithValue("@Gender", newEmployee.Gender);
                command.Parameters.AddWithValue("@Age", newEmployee.Age);
                /*command.Parameters.AddWithValue("@Salary", newEmployee.Salary);
                command.Parameters.AddWithValue("@DailyWorkingHours", newEmployee.DailyWorkingHours);
                command.Parameters.AddWithValue("@Attendance", newEmployee.Attendance);*/
                command.ExecuteNonQuery();
            }
        }

        public static void RemoveEmployee(Employee emp)
        {
            RemoveEmployee(emp.Id);
        }

        public static void RemoveEmployee(int EmployeeId)
        {
            if (Connection.conn is null)
            {
                throw new Exception("The database Connection.connection is not established yet!");
            }

            using (var command = new SQLiteCommand(q_RemoveEmployee, Connection.conn))
            {
                command.Parameters.AddWithValue("@Id", EmployeeId);
                command.ExecuteNonQuery();
            }
        }
    }
}
