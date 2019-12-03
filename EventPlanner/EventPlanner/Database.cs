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

        public User AddUser(string email, string passwordHash)
        {
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [User] " +
                    "([Email], [PasswordHash]) " +
                    "VALUES (@email, @passwordHash)";
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("passwordHash", passwordHash);

                cmd.ExecuteNonQuery();
            }

            return GetUserByEmail(email);
        }


        public User GetUserById(int Id)
        {
            User user = null;
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [user] WHERE id = @id";
                cmd.Parameters.AddWithValue("id", Id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User();

                        user.Id = (int)reader["Id"];
                        user.Email = (string)reader["Email"];
                        user.PasswordHash = (string)reader["PasswordHash"];
                        
                        
                    }
                }
            }
            if(user != null)
            {
                user.Roles = GetUserRoles(user.Id);
            }
            

            return user;
        }

        public User GetUserByEmail(string email)
        {
            User user = null;
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [user] WHERE email = @email";
                cmd.Parameters.AddWithValue("email", email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User();

                        user.Id = (int)reader["Id"];
                        user.Email = (string)reader["Email"];
                        user.PasswordHash = (string)reader["PasswordHash"];
                        

                        
                    }
                }
            }
            if (user != null)
            {
                user.Roles = GetUserRoles(user.Id);
            }

            return user;
        }

        public List<string> GetUserRoles (int userId)
        {
            List<string> roles = new List<string>();
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM[UserRole] JOIN[Role] ON " +
                    "[Role].Id = [UserRole].RoleId WHERE [UserRole].UserId = @id";
                cmd.Parameters.AddWithValue("id", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        roles.Add((string)reader["Role"]);

                        
                    }
                }

            }
            return roles;
        }

        public User Login (string email, string passwordHash)
        {
            User user = null;

            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [user] WHERE Email = @email AND PasswordHash = @passwordHash";
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("passwordHash", passwordHash);
                
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User();


                        user.Id = (int)reader["Id"];
                        user.Email = (string)reader["Email"];
                        user.PasswordHash = (string)reader["PasswordHash"];
                        

                        
                    }
                }
            }
            if (user != null)
            {
                user.Roles = GetUserRoles(user.Id);
            }

            return user;
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
                    }
                }
            }
            if (ev != null)
            {
                ev.Participant.AddRange(GetEventParticipants(ev.Id));
                ev.ChatMessages.AddRange(GetEventChatMessages(ev.Id));
            }


            
            return ev;
        }

        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [Event]";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        Event ev = new Event();

                        ev.Id = (int)reader["Id"];
                        ev.Name = (string)reader["Name"];
                        ev.Description = (string)reader["Description"];
                        ev.MaxParticipant = (int)reader["MaxParticipant"];
                        ev.Date = (DateTime)reader["Date"];
                        ev.Location = (string)reader["Location"];


                        events.Add(ev);
                    }
                }
            }
            foreach (Event ev in events)
            {
                ev.Participant.AddRange(GetEventParticipants(ev.Id));
            }
            foreach (Event ev in events)
            {
                ev.ChatMessages.AddRange(GetEventChatMessages(ev.Id));
            }
            return events;
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

        public List<ChatMessage> GetEventChatMessages(int eventId)
        {
            List<ChatMessage> chatMessages = new List<ChatMessage>();

            using (SqlCommand cmd = con.CreateCommand()) // Short variable for the sql command
            {
                cmd.CommandText = "SELECT * FROM [ChatMessage] WHERE[EventId] = @id"; // Ask an sql question to the database
                cmd.Parameters.AddWithValue("id", eventId);

                using (SqlDataReader reader = cmd.ExecuteReader())  // Run the query save the result in reader variable
                {
                    while (reader.Read())
                    {
                        ChatMessage chatMessage = new ChatMessage();
                        chatMessage.Id = (int)reader["Id"];
                        chatMessage.UserId = (int)reader["UserId"];
                        chatMessage.Date = (DateTime)reader["Date"];
                        chatMessage.EventId = (int)reader["EventId"];
                        chatMessage.Message = (string)reader["Message"];

                        chatMessages.Add(chatMessage);
                    }
                }
            }

            return chatMessages;
        }
    }
}
