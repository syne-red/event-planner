using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EventPlanner
{
    class EventManager
    {
        public User LoggedInUser;

        public void Start()
        {
            Database database = new Database();
            LoginMenu(database);
        }

        // Show the login menu when the program starts
        public void LoginMenu(Database database)
        {
            Console.Clear();

            bool running = true;

            while (running)
            {
                Logger.WriteLine("--- Welcome to The event planer! ---");
                Logger.WriteLine("Select an option ");
                Logger.WriteLine(" 1.Create User Account ");
                Logger.WriteLine(" 2.Log in ");
                Logger.WriteLine(" 0.Exit ");
                Logger.Write("Input selection: ");

                string input = Logger.ReadLine();

                switch (input)
                {
                    case "1":
                        CreateUser(database);
                        break;
                    case "2":
                        Login(database);
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Logger.WriteLine("No input chosen");
                        break;
                }
            }
        }

        // Show the user menu when a user logs in
        public void UserMenu(Database database)
        {
            Console.Clear();

            bool running = true;

            while (running)
            {
                Logger.WriteLine("--- Welcome to The event planer! ---");
                Logger.WriteLine("Select an option ");
                Logger.WriteLine(" 1.Show events ");
                Logger.WriteLine(" 2.Join event ");
                Logger.WriteLine(" 3.Show event chat ");
                Logger.WriteLine(" 4.Add chat messsage ");
                Logger.WriteLine(" 0.Logout my good sir ");
                Logger.Write("Input selection: ");

                string input = Logger.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowEvents(database);
                        break;
                    case "2":
                        JoinEvent(database);
                        break;
                    case "3":
                        ShowEventChat(database);
                        break;
                    case "4":
                        AddChatMessage(database);
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Logger.WriteLine("No input chosen");
                        break;
                }
            }
        }

        // Show the admin menu when an administrator logs in
        public void AdminMenu(Database database)
        {
            Console.Clear();

            bool running = true;

            while (running)
            {
                Logger.WriteLine("--- Welcome Admin ---");
                Logger.WriteLine("Select an option ");
                Logger.WriteLine(" 1.Create event ");
                Logger.WriteLine(" 2.Show Events ");
                Logger.WriteLine(" 3.Show event chat ");
                Logger.WriteLine(" 4.Add chat message ");
                Logger.WriteLine(" 5.DELETE MESSAGE ");
                Logger.WriteLine(" 0.Logout sir ");
                Logger.Write("Input selection: ");

                string input = Logger.ReadLine();

                switch (input)
                {
                    case "1":
                        CreateEvent(database);
                        break;
                    case "2":
                        ShowEvents(database);
                        break;
                    case "3":
                        ShowEventChat(database);
                        break;
                    case "4":
                        AddChatMessage(database);
                        break;
                    case "5":
                        DeleteMessage(database);
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Logger.WriteLine("No input chosen");
                        break;
                }
            }
        }

        // Create a user account
        public void CreateUser(Database database)
        {
            Console.Clear();

            while (true)
            {
                Logger.Write("Enter email: ");
                string email = Logger.ReadLine();

                Logger.Write("Enter password: ");
                string password = Logger.ReadPassword();

                string passwordHash = Hasher.Hash(password);

                Logger.WriteLine($"You created user {email} with password: {passwordHash}");

                if (database.GetUserByEmail(email) != null)
                {
                    Logger.WriteLine("That email already exists");
                    continue;
                }

                LoggedInUser = database.AddUser(email, passwordHash);
                Logger.WriteLine("Log in successfull!!!!!!");
                UserMenu(database);
                break;
            }
        }

        // Handler for logging in
        public void Login(Database database)
        {
            Console.Clear();

            while (true)
            {
                Logger.Write("Enter email: ");
                string email = Logger.ReadLine();

                Logger.Write("Enter password: ");
                string password = Logger.ReadPassword();

                if (!Validation.IsValidEmail(email))
                {
                    Logger.WriteLine("The email you entered is not valid");
                    continue;
                }

                if (password == "")
                {
                    Logger.WriteLine("You must enter a password");
                    continue;
                }

                string passwordHash = Hasher.Hash(password);
                User user = database.Login(email, passwordHash);
                if (user == null)
                {
                    Logger.WriteLine("Username or password wrong");
                    continue;
                }

                LoggedInUser = user;
                Logger.WriteLine($"Welcome to the matrix {user.Email}");

                if (user.HasRole("admin"))
                {
                    AdminMenu(database);
                }
                else
                {
                    UserMenu(database);
                }

                break;
            }
        }

        // [Admin] Create an event
        public void CreateEvent(Database database)
        {
            Console.Clear();

            while (true)
            {
                Logger.Write("Event name: ");
                string name = Logger.ReadLine();

                Logger.Write("Description: ");
                string description = Logger.ReadLine();

                Logger.Write("Maximum participants: ");
                string maxParticipanInput = Logger.ReadLine();

                Logger.Write("Date: ");
                string date = Logger.ReadLine();

                Logger.Write("Location: ");
                string location = Logger.ReadLine();

                // Validation here
                if (!Validation.IsValidEventName(name))
                {
                    Logger.WriteLine("The event name is not valid");
                    continue;
                }

                if (!Validation.IsValidDescription(description))
                {
                    Logger.WriteLine("The description is not valid");
                    continue;
                }

                int maxParticipants;
                if (!int.TryParse(maxParticipanInput, out maxParticipants))
                {
                    Logger.WriteLine("The max participant is not a valid number");
                    continue;
                }

                if (!Validation.IsValidMaxParticipants(maxParticipants))
                {
                    Logger.WriteLine("The max participants must be between 1 and 10000");
                    continue;
                }

                DateTime eventDate;

                if (!DateTime.TryParse(date, out eventDate))
                {
                    Logger.WriteLine("The date is not valid");
                    continue;
                }

                if (!Validation.IsValidEventDate(eventDate))
                {
                    Logger.WriteLine("The date must be in future");
                    continue;
                }

                if (!Validation.IsValidLocation(location))
                {
                    Logger.WriteLine("The location is not valid");
                    continue;
                }

                database.AddEvent(LoggedInUser.Id, name, description, maxParticipants, eventDate, location);
                Logger.WriteLine("--- Event added to database! ---");
                break;

            }
        }

        // Show all events
        public void ShowEvents(Database database)
        {
            Console.Clear();

            Logger.WriteLine("--------------------- Event List ---------------------");
            foreach (Event ev in database.GetAllEvents())
            {
                Logger.WriteLine($"{ev.Id}, {ev.Name}");
            }
            Logger.WriteLine("--------------------- END ---------------------");
        }

        // Join an event
        public void JoinEvent(Database database)
        {
            Console.Clear();

            // ask for an event id, check event exist
            // add Participant to db
        }

        // Show all chat messages of an event
        public void ShowEventChat(Database database)
        {
            Console.Clear();

            // ask for an event id, check event exist
            // list event chat messages
        }

        // Add a chat message to an event
        public void AddChatMessage(Database database)
        {
            Console.Clear();

            // ask for an event id, check event exist
            // add message to db
        }

        // Delete an event chat message
        public void DeleteMessage(Database database)
        {
            Console.Clear();

            // ask for message id, check message exist
            // delete
        }

        public void ShowLoginError()
        {
            Console.Clear();

            // do we need this??
        }
    }
}
