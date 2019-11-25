using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace EventPlanner
{
    class Database
    {
        private string connectionString;
        private SqlConnection con;

        public Database()
        {
            // read the connection string from a file named db.txt
            connectionString = File.ReadAllText("db.txt");

            con = new SqlConnection(connectionString);
        }

        public void DBConn()
        {
            //var con = new SqlConnection(connectionString);
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();

            var cmd = con.CreateCommand();


            cmd.CommandText = "SELECT * FROM test;";

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                Console.WriteLine(id);
            }
            //cmd.CommandText = "select ...";

            //var reader = cmd.ExecuteReader();

            //while (reader.Read())
            //{
            //    var name = reader.GetString(0);
            //}
        }

        public void GetUserById(int Id)
        {
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();

            var cmd = con.CreateCommand();

            cmd.CommandText = "SELECT * FROM [user] WHERE id = " + Id;
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                Console.WriteLine($"{id}, {name}");
            }
        }

        public void GetEvent(int Id) { }
    }
}
