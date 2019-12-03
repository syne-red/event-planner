using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EventPlanner
{
    class EventManager : IDisposable // IDisposable allows to put EventManager class inside a: using (...) {} statement
    {
        private readonly Database _database = new Database();
        
        // If a user is logs in, we set this value to the logged in user
        private User _loggedInUser = null;

        // Called when the using(eventManager) { } block ends
        public void Dispose()
        {
            // dispose the database connection and free up resources
            _database.Dispose();
        }

        // Runs at the start of the program
        public void Start()
        {
            // User is not logged in here, show the login menu
            ShowLoginMenu();
        }

        // Show the login menu when the program starts
        private void ShowLoginMenu()
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
                        CreateUser();
                        break;
                    case "2":
                        Login();
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
        private void ShowUserMenu()
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
                        ShowEvents();
                        break;
                    case "2":
                        JoinEvent();
                        break;
                    case "3":
                        ShowEventChat();
                        break;
                    case "4":
                        AddChatMessage();
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
        private void ShowAdminMenu()
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
                        CreateEvent();
                        break;
                    case "2":
                        ShowEvents();
                        break;
                    case "3":
                        ShowEventChat();
                        break;
                    case "4":
                        AddChatMessage();
                        break;
                    case "5":
                        DeleteMessage();
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
        private void CreateUser()
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

                if (_database.GetUserByEmail(email) != null)
                {
                    Logger.WriteLine("That email already exists");
                    continue;
                }

                _loggedInUser = _database.AddUser(email, passwordHash);
                Logger.WriteLine("Log in successfull!!!!!!");
                ShowUserMenu();
                break;
            }
        }

        // Handler for logging in
        private void Login()
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
                User user = _database.Login(email, passwordHash);
                if (user == null)
                {
                    Logger.WriteLine("Username or password wrong");
                    continue;
                }

                _loggedInUser = user;
                Logger.WriteLine($"Welcome to the matrix {user.Email}");

                if (user.HasRole("admin"))
                {
                    ShowAdminMenu();
                }
                else
                {
                    ShowUserMenu();
                }

                break;
            }
        }

        // [Admin] Create an event
        private void CreateEvent()
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

                _database.AddEvent(_loggedInUser.Id, name, description, maxParticipants, eventDate, location);
                Logger.WriteLine("--- Event added to database! ---");
                break;
            }
        }

        // Show all events
        private void ShowEvents()
        {
            Console.Clear();

            Logger.WriteLine("--------------------- Event List ---------------------");
            foreach (Event ev in _database.GetAllEvents())
            {
                Logger.WriteLine($"{ev.Id}, {ev.Name}");
            }
            Logger.WriteLine("--------------------- END ---------------------");
        }

        // Join an event
        private void JoinEvent()
        {
            Console.Clear();

            // ask for an event id, check event exist
            // add Participant to db
        }

        // Show all chat messages of an event
        private void ShowEventChat()
        {
            Console.Clear();

            // ask for an event id, check event exist
            // list event chat messages
        }

        // Add a chat message to an event
        private void AddChatMessage()
        {
            Console.Clear();

            // ask for an event id, check event exist
            // add message to db
        }

        // Delete an event chat message
        private void DeleteMessage()
        {
            Console.Clear();

            // ask for message id, check message exist
            // delete
        }
    }
}
