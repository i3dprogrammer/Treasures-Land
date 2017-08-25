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



/*
Data Source=JA7IM\SQLEXPRESS;Integrated Security=True;;MultipleActiveResultSets=true

CREATE TABLE [dbo].[Children] ([Id] INT IDENTITY (1, 1) NOT NULL, [Name] NVARCHAR (50) NOT NULL, [Age]             INT           NOT NULL,
    [Gender]          INT           NOT NULL,
    [WhatsAppPhone]   NVARCHAR (50) NULL,
    [AcademicYear]    INT           NOT NULL,
    [EducationType]   INT           NOT NULL,
    [ChildBirthOrder] INT           NULL,
    [ChildTraits]     NVARCHAR (50) NULL,
    [ChildHandling]   NVARCHAR (50) NULL,
    [ChildFreeTime]   NVARCHAR (50) NULL,
    [GuardianName]    NVARCHAR (50) NOT NULL,
    [GuardianType]    INT           NOT NULL,
    [MotherPhone]     NVARCHAR (50) NULL,
    [FatherPhone]     NVARCHAR (50) NULL,
    [MotherJob]       NVARCHAR (50) NULL,
    [FatherJob]       NVARCHAR (50) NULL,
    [MotherQualifier] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[Courses] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (50) NOT NULL,
    [AcademicYearStart] INT           NOT NULL,
    [AcademicYearEnd]   INT           NOT NULL,
    [Cost]              INT           NOT NULL,
    [PricePerChild]     INT           NOT NULL,
    [Full]              BIT           NOT NULL,
    [Over]              BIT           NOT NULL,
    [Level]             INT           NOT NULL,
    [StartDate]         DATE          NOT NULL,
    [EndDate]           DATE          NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[ChildrenCourses] (
    [ChildId]    INT           NOT NULL,
    [CourseId]   INT           NOT NULL,
    [Attendance] NVARCHAR (50) NULL,
    CONSTRAINT [FK_ChildrenCourses_ToChildren] FOREIGN KEY ([ChildId]) REFERENCES [dbo].[Children] ([Id]),
    CONSTRAINT [FK_ChildrenCourses_ToCourses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses] ([Id])
);
GO

CREATE TABLE [dbo].[CourseSessions] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50) NULL,
    [CourseId]  INT           NOT NULL,
    [Number]    INT           NOT NULL,
    [StartDate] DATETIME      NOT NULL,
    [EndDate]   DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CourseSessions_ToCourses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses] ([Id])
);
GO

CREATE TABLE [dbo].[Employees] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [Address]    NVARCHAR (50) NOT NULL,
    [Phone]      NVARCHAR (50) NOT NULL,
    [NationalID] NVARCHAR (50) NOT NULL,
    [Qualifier]  NVARCHAR (50) NOT NULL,
    [Gender]     INT           NOT NULL,
    [Age]        INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[EmployeeWorkingHours] (
    [Id]             INT      NOT NULL,
    [FridayStart]    TIME (7) NOT NULL,
    [FridayEnd]      TIME (7) NOT NULL,
    [SaturdayStart]  TIME (7) NOT NULL,
    [SaturdayEnd]    TIME (7) NOT NULL,
    [SundayStart]    TIME (7) NOT NULL,
    [SundayEnd]      TIME (7) NOT NULL,
    [MondayStart]    TIME (7) NOT NULL,
    [MondayEnd]      TIME (7) NOT NULL,
    [TuesdayStart]   TIME (7) NOT NULL,
    [TuesdayEnd]     TIME (7) NOT NULL,
    [WednesdayStart] TIME (7) NOT NULL,
    [WednesdayEnd]   TIME (7) NOT NULL,
    [ThursdayStart]  TIME (7) NOT NULL,
    [ThursdayEnd]    TIME (7) NOT NULL,
    CONSTRAINT [FK_EmployeeWorkingHours_ToEmployees] FOREIGN KEY ([Id]) REFERENCES [dbo].[Employees] ([Id])
);
GO

CREATE TABLE [dbo].[Instructors] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50)   NOT NULL,
    [Address]   NVARCHAR (50)   NOT NULL,
    [Phone]     NVARCHAR (50)   NOT NULL,
    [Qualifier] NVARCHAR (50)   NOT NULL,
    [CV]        VARBINARY (MAX) NOT NULL,
    [Gender]    INT             NOT NULL,
    [Age]       INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[InstructorCourses] (
    [InstructorId] INT NOT NULL,
    [CourseId]     INT NOT NULL,
    CONSTRAINT [FK_InstructorCourses_ToCoaches] FOREIGN KEY ([InstructorId]) REFERENCES [dbo].[Instructors] ([Id]),
    CONSTRAINT [FK_InstructorCourses_ToCourses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses] ([Id])
);
GO

CREATE TABLE [dbo].[InstructorAbsence] (
    [InstructorId] INT  NOT NULL,
    [Month]        DATE NOT NULL,
    [Hours]        INT  NOT NULL,
    CONSTRAINT [FK_InstructorAbsence_ToInstructors] FOREIGN KEY ([InstructorId]) REFERENCES [dbo].[Instructors] ([Id])
);
GO

CREATE TABLE [dbo].[InstructorSalary] (
    [InstructorId] INT NOT NULL,
    [CourseId]     INT NOT NULL,
    [Salary]       INT NOT NULL,
    [Paid]         BIT NOT NULL,
    CONSTRAINT [FK_InstructorSalary_ToInstructor] FOREIGN KEY ([InstructorId]) REFERENCES [dbo].[Instructors] ([Id])
);
GO


*/
