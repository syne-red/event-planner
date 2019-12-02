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
            con.Open();
        }

        public void Close()
        {
            con.Close();
        }

        public User GetUserById(int Id)
        {

            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [user] WHERE id = @id";
                cmd.Parameters.AddWithValue("id", Id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User();

                        user.Id = (int)reader["Id"];
                        user.Email = (string)reader["Email"];
                        user.PasswordHash = (string)reader["Password"];

                        return user;
                    }
                }
            }

            return null;
        }

        public User Login (string email, string passwordHash)
        {

            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [user] WHERE Email = @email AND PasswordHash = @passwordHash";
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("passwordHash", passwordHash);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User();

                        user.Email = (string)reader["Email"];
                        user.PasswordHash = (string)reader["PasswordHash"];

                        return user;
                    }
                }
            }

            return null;
        }

        public void AddEvent(int creatorId, string name, string description, int maxParticipant, DateTime date, string location)
        {
            
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [Event] " +
                    "([CreatorId], [Name], [Description], [MaxParticipant], [Date], [Location]) " + 
                    "VALUES (@creatorId, @name, @description, @maxParticipant, @date, @location)";
                cmd.Parameters.AddWithValue("creatorId", creatorId);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("description", description);
                cmd.Parameters.AddWithValue("maxParticipant", maxParticipant);
                cmd.Parameters.AddWithValue("date", date);
                cmd.Parameters.AddWithValue("location", location);

                cmd.ExecuteNonQuery();
            }
        }
        public Event GetEvent(int Id)
        {

            Event ev = null;
            int creatorId = 0;

            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [Event] WHERE [Id] = @id";
                cmd.Parameters.AddWithValue("id", Id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ev = new Event();

                        ev.Id = (int)reader["Id"];
                        ev.Name = (string)reader["Name"];
                        ev.Description = (string)reader["Description"];
                        ev.MaxParticipant = (int)reader["MaxParticipant"];
                        ev.Date = (DateTime)reader["Date"];
                        ev.Location = (string)reader["Location"];

                        creatorId = (int)reader["CreatorId"];
                    }
                }
            }

            if (ev != null)
            {
                ev.Creator = GetUserById(creatorId);

                // get event participants
            }

            return ev;
        }

        public List<User> GetEventParticipants(int eventId)
        {
            List<int> userIds = new List<int>();

            using (SqlCommand cmd = con.CreateCommand()) // Short variable for the sql command
            {
                cmd.CommandText = "SELECT * FROM [Participant] WHERE [EventId] = @eventId"; // Ask an sql question to the database
                cmd.Parameters.AddWithValue("eventId", eventId);

                using (SqlDataReader reader = cmd.ExecuteReader())  // Run the query save the result in reader variable
                {
                    while (reader.Read())
                    {
                        int userId = (int)reader["UserId"];
                        userIds.Add(userId);
                    }
                }
            }

            List<User> users = new List<User>();

            foreach (int id in userIds)
            {
                User user = GetUserById(id);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }
    }
}
