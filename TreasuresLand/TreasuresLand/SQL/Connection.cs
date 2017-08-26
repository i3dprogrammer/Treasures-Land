using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;

namespace TreasuresLand.SQL
{
    public class Connection
    {
        internal static SQLiteConnection conn = new SQLiteConnection("Data Source=MainDB.sqlite;Version=3;");

        public static void CreateDatabase()
        {
            SQLiteConnection.CreateFile("MainDB.sqlite");
        }

        public static void CreateTables()
        {
            string[] db = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "\\db_updated.txt");
            string sql = db.Aggregate((x, y) => x + "\n" + y);
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
        }

        public static void Connect()
        {
            if (!File.Exists(".\\MainDB.sqlite"))
            { 
                CreateDatabase();
                CreateTables();
            }

            if(conn.State == System.Data.ConnectionState.Closed)
                conn.Open();
        }

        //public static void Connect(string connString = "")
        //{
        //    conn = new SqlConnection();
        //    if (connString == "")
        //        conn.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=true";
        //    else
        //        conn.ConnectionString = connString;
        //    conn.Open();
        //}

        //public static bool IsConnected()
        //{
        //    return (conn != null && conn.State == System.Data.ConnectionState.Open);
        //}

        //public static void Dispose()
        //{
        //    conn.Close();
        //    conn.Dispose();
        //}
    }
}