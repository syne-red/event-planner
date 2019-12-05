using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace EventPlanner
{
    class Database : IDisposable // IDisposable allows to put Database class inside a: using (...) {} statement
    {
        private readonly string _connectionString;
        private readonly SqlConnection _connection;

        public Database()
        {
            // read the connection string from a file named db.txt
            _connectionString = File.ReadAllText("db.txt");

            // create and open a connection to database
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        // Must be called when this class is no longer needed to close SQL connection
        public void Dispose()
        {
            // close the database connection
            _connection.Dispose();
        }
        /// <summary>
        /// Opens a sql connection and inserts a user to the user table
        /// </summary>
        /// <param name="email">The users inputed email</param>
        /// <param name="passwordHash">The users inputed password</param>
        public User AddUser(string email, string passwordHash)
        {
            using (SqlCommand cmd = _connection.CreateCommand())
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

        /// <summary>
        /// Gets a user from the database on the user id
        /// </summary>
        /// <param name="Id">UserId</param>
        public User GetUserById(int Id)
        {
            User user = null;
            using (SqlCommand cmd = _connection.CreateCommand())
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
                    }
                }
            }

            if(user != null)
            {
                user.Roles = GetUserRoles(user.Id);
            }

            return user;
        }

        /// <summary>
        /// Selects a user from the user table on the user email
        /// </summary>
        /// <param name="email">The users email</param>
        public User GetUserByEmail(string email)
        {
            User user = null;
            using (SqlCommand cmd = _connection.CreateCommand())
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
                    }
                }
            }
            // If the user is logged in, set role
            if (user != null)
            {
                user.Roles = GetUserRoles(user.Id);
            }

            return user;
        }
        /// <summary>
        /// Select all roles from the database
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        public List<string> GetUserRoles(int userId)
        {
            List<string> roles = new List<string>();

            using (SqlCommand cmd = _connection.CreateCommand())
            {
                // Join the tables UserRole with Role
                cmd.CommandText = "SELECT * FROM [UserRole] JOIN [Role] ON [Role].[Id] = [UserRole].[RoleId] WHERE [UserRole].[UserId] = @userId";
                cmd.Parameters.AddWithValue("userId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add((string)reader["Role"]);
                    }
                }
            }

            return roles;
        }

        /// <summary>
        /// Login a user check email and password from database
        /// </summary>
        /// <param name="email">The users email</param>
        /// <param name="passwordHash">The users password</param>
        public User Login(string email, string passwordHash)
        {
            User user = null;

            using (SqlCommand cmd = _connection.CreateCommand())
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
                    }
                }
            }

            if (user != null)
            {
                user.Roles = GetUserRoles(user.Id);
            }

            return user;
        }

        /// <summary>
        /// Insert an event to the database
        /// </summary>
        /// <param name="creatorId">The creator of the event</param>
        /// <param name="name">Name of event</param>
        /// <param name="description">The event description</param>
        /// <param name="maxParticipant">Number of max participants that can attend the event</param>
        /// <param name="date">The date when the event is happening</param>
        /// <param name="location">The location of the event</param>
        public void AddEvent(int creatorId, string name, string description, int maxParticipant, DateTime date, string location)
        {
            using (SqlCommand cmd = _connection.CreateCommand())
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

        /// <summary>
        /// Insert a participant to the event
        /// </summary>
        /// <param name="userId">The participant</param>
        /// <param name="eventId">The event the participant is added to</param>
        public void AddEventParticipant(int userId, int eventId)
        {
            using (SqlCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [Participant] " +
                    "([UserId], [EventId]) " +
                    "VALUES (@userId, @eventId)";
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("eventId", eventId);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get an event from the database
        /// </summary>
        /// <param name="id">The id of the event</param>
        public Event GetEvent(int id)
        {
            Event ev = null;

            using (SqlCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [Event] WHERE [Id] = @id";
                cmd.Parameters.AddWithValue("id", id);

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
                ev.Participants.AddRange(GetEventParticipants(ev.Id));
                ev.ChatMessages.AddRange(GetEventChatMessages(ev.Id));
            }

            return ev;
        }

        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();

            using (SqlCommand cmd = _connection.CreateCommand())
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
                ev.Participants.AddRange(GetEventParticipants(ev.Id));
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

            using (SqlCommand cmd = _connection.CreateCommand()) // Short variable for the sql command
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

            using (SqlCommand cmd = _connection.CreateCommand()) // Short variable for the sql command
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

        public void AddChatMessage(int userId, int eventId, string message)
        {
            using (SqlCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [ChatMessage] ([EventId], [UserId], [Date], [Message]) VALUES (@eventId, @userId, @date, @message)";
                cmd.Parameters.AddWithValue("eventId", eventId);
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("date", DateTime.Now);
                cmd.Parameters.AddWithValue("message", message);

                cmd.ExecuteNonQuery();
            }
        }

        public bool DeleteChatMessage(int messageId)
        {
            using (SqlCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM [ChatMessage] WHERE [Id] = @id";
                cmd.Parameters.AddWithValue("id", messageId);

                return cmd.ExecuteNonQuery() != 0;
                
            }
        }
    }
}
