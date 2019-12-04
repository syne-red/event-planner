using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

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

                Logger.WriteLine("");
                Logger.WriteLine(" ---------------- WELCOME TO THE EVENT PLANNER! -----------------");
                Logger.WriteLine("");
                Logger.WriteLine(" ----------------------------------------------------------------");
                Logger.WriteLine("|    1. Create User Account    |    2. Log in    |    0. Exit    |");
                Logger.WriteLine(" ----------------------------------------------------------------");
                Logger.WriteLine("");
                Logger.Write("Select menu number: ");

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

            Logger.WriteLine("");
            Logger.WriteLine(" -------------------------------------------------- WELCOME USER! -----------------------------------------------------");
            Logger.WriteLine("");

            while (running)
            {
                Logger.WriteLine(" ----------------------------------------------------------------------------------------------------------------------");
                Logger.WriteLine("|    1. Show events    |    2. Join event    |    3. Show event chat    |    4. Add chat messsage    |    0. Logout    |");
                Logger.WriteLine(" ----------------------------------------------------------------------------------------------------------------------");
                Logger.WriteLine("");
                Logger.Write("Select menu number: ");

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

            Logger.WriteLine("");
            Logger.WriteLine(" ----------------------------------------------------------------- WELCOME ADMIN! --------------------------------------------------------------------");
            Logger.WriteLine("");

            while (running)
            {
                Logger.WriteLine(" --------------------------------------------------------------------------------------------------------------------------------------------------");
                Logger.WriteLine("|    1. Create event    |    2. Show Events    |    3. Show event chat    |    4. Add chat messsage    |    5. DELETE MESSAGE    |    0. Logout    |");
                Logger.WriteLine(" --------------------------------------------------------------------------------------------------------------------------------------------------");
                Logger.WriteLine("");
                Logger.Write("Select menu number: ");

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

                Logger.WriteLine($"You created user {email}");

                if (_database.GetUserByEmail(email) != null)
                {
                    Logger.WriteLine("That email already exists");
                    continue;
                }

                _loggedInUser = _database.AddUser(email, passwordHash);
                Logger.WriteLine("Log in successfull !");
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
                Logger.WriteLine("");
                Logger.WriteLine("  - EVENT ADDED!");
                break;
            }
        }

        // Show all events
        private void ShowEvents()
        {
            Console.Clear();

            Logger.WriteLine("------------- Event List ------------");
            foreach (Event ev in _database.GetAllEvents())
            {
                Logger.WriteLine($"| {ev.Id}, {ev.Name}");
            }
            Logger.WriteLine("---------------- END ----------------");
            Logger.WriteLine("");
        }

        // Join an event
        private void JoinEvent()
        {
            Console.Clear();

            Logger.WriteLine("------------- Event List ------------");
            foreach (Event ev in _database.GetAllEvents())
            {
                Logger.WriteLine($"| {ev.Id}, {ev.Name}");
            }
            Logger.WriteLine("---------------- END ----------------");
            Logger.WriteLine("");

            while (true)
            {

                Logger.Write("Select event number to join: ");
                string eventIdInput = Logger.ReadLine();

                int eventId = Int32.Parse(eventIdInput);


                if (_database.GetEvent(eventId) == null)
                {
                    Logger.WriteLine("");
                    Logger.WriteLine("  - SELECT A VALID EVENT ID!");
                }
                else
                {
                    _database.AddEventParticipant(_loggedInUser.Id, eventId);
                    Logger.WriteLine("");
                    Logger.WriteLine("  - EVENT JOINED!");
                    break;
                }
            }

        }

        // Show all chat messages of an event
        private void ShowEventChat()
        {
            Console.Clear();

            Logger.WriteLine("------------- Event List ------------");
            foreach (Event ev in _database.GetAllEvents())
            {
                Logger.WriteLine($"| {ev.Id}, {ev.Name}");
            }
            Logger.WriteLine("---------------- END ----------------");
            Logger.WriteLine("");

            while (true)
            {

                Logger.Write("Select event number: ");
                string eventIdInput = Logger.ReadLine();

                int eventId = Int32.Parse(eventIdInput);


                if (_database.GetEvent(eventId) == null)
                {
                    Logger.WriteLine("");
                    Logger.WriteLine("  - SELECT A VALID EVENT ID!");
                    continue;
                }

                Logger.WriteLine("");

                // Call function
                foreach (ChatMessage chatMessage in _database.GetEventChatMessages(eventId))
                {
                    Logger.WriteLine($"{chatMessage.Id}, {chatMessage.Date}, {chatMessage.Message}");
                }
                break;
            }
            // ask for an event id, check event exist
            // list event chat messages
        }

        // Add a chat message to an event
        private void AddChatMessage()
        {
            Console.Clear();

            Logger.WriteLine("------------- Event List ------------");
            foreach (Event ev in _database.GetAllEvents())
            {
                Logger.WriteLine($"| {ev.Id}, {ev.Name}");
            }
            Logger.WriteLine("---------------- END ----------------");
            Logger.WriteLine("");

            while (true)
            {
                Logger.Write("Select event number: ");
                string eventIdInput = Logger.ReadLine();

                int eventId = Int32.Parse(eventIdInput);


                if (_database.GetEvent(eventId) == null)
                {
                    Logger.WriteLine("");
                    Logger.WriteLine("  - SELECT A VALID EVENT ID!");

                    continue;
                }

                Logger.WriteLine("");
                Logger.Write("Enter chat message: ");
                string message = Logger.ReadLine();

                if (!Validation.IsValidChatMessage(message))
                {
                    Logger.WriteLine("Invalid chat message!");
                    continue;
                }

                _database.AddChatMessage(_loggedInUser.Id, eventId, message);

                break;
            }
            // ask for an event id, check event exist
            // add message to db
        }

        // Delete an event chat message
        private void DeleteMessage()
        {
            Console.Clear();

            Logger.WriteLine("------------- Event List ------------");
            foreach (Event ev in _database.GetAllEvents())
            {
                Logger.WriteLine($"| {ev.Id}, {ev.Name}");
            }
            Logger.WriteLine("---------------- END ----------------");
            Logger.WriteLine("");

            while (true)
            {
                Logger.Write("Enter event id: ");
                string eventIdInput = Logger.ReadLine();
                int eventId = int.Parse(eventIdInput);

                Event ev = _database.GetEvent(eventId);

                if (ev == null)
                {
                    Logger.WriteLine("Event not found");
                    continue;
                }

                foreach(ChatMessage chatMessage in ev.ChatMessages)
                {
                    Logger.WriteLine($"{chatMessage.Id}, {chatMessage.Date}, {chatMessage.Message}");
                }

                Logger.Write("Enter chat message id: ");
                string messageIdInput = Logger.ReadLine();
                int messageId = int.Parse(messageIdInput);

                if (!_database.DeleteChatMessage(messageId))
                {
                    Logger.WriteLine("Chat message not found");
                    continue;
                }

                Logger.WriteLine("Chat message deleted!");
                break;
            }

        }
    }
}
