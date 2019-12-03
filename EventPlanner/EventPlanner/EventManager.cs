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

        public void UserMenu(Database database)
        {
            Console.Clear();
            bool running = true;

            while (running)
            {
                Logger.WriteLine("--- Welcome to The event planer! ---");
                Logger.WriteLine("Select an option ");
                Logger.WriteLine(" 1.Show events ");
                Logger.WriteLine(" 2.Join event "); // list events
                Logger.WriteLine(" 3.Show event chat ");
                Logger.WriteLine(" 4.Add chat messsage ");
                Logger.WriteLine(" 0.Logout my good sir ");
                Console.Write("Input selection: ");

                string input = Console.ReadLine();

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
                Console.Write("Input selection: ");

                string input = Console.ReadLine();

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
                Console.Write("Input selection: ");

                string input = Console.ReadLine();

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

                Console.WriteLine($"You created user {email} with password: {passwordHash}");

                if (database.GetUserByEmail(email) != null)
                {
                    continue;
                }

                LoggedInUser = database.AddUser(email, passwordHash);
                Console.WriteLine("Log in successfull!!!!!!");
                UserMenu(database);
                break;
            }
            


        }
        
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
                    continue;
                }

                if (!Validation.IsValidDescription(description))
                {
                    continue;
                }

                int maxParticipants;
                if (!int.TryParse(maxParticipanInput, out maxParticipants))
                {
                    continue;
                }
                if (!Validation.IsValidMaxParticipants(maxParticipants))
                {
                    continue;
                }

                DateTime eventDate;

                if(!DateTime.TryParse(date, out eventDate))
                {
                    continue;
                }

                if (!Validation.IsValidEventDate(eventDate))
                {
                    continue;
                }

                if (!Validation.IsValidLocation(location))
                {
                    continue;
                }

                database.AddEvent(1, name, description, maxParticipants, eventDate, location);
                Logger.WriteLine("--- Event added to database! ---");
                break;

            }
        }


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

        public void JoinEvent(Database database)
        {
            Console.Clear();
        }

        public void ShowEventChat(Database database)
        {
            Console.Clear();
        }

        public void AddChatMessage(Database database)
        {
            Console.Clear();
        }

        public void DeleteMessage(Database database)
        {
            Console.Clear();
        }

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
                    Logger.WriteLine("fel");
                    continue;
                }

                if (password == "")
                {
                    continue;
                }

                string passwordHash = Hasher.Hash(password);
                User user = database.Login(email, passwordHash);
                if (user == null)
                {
                    continue;
                }
                LoggedInUser = user;
                Console.WriteLine($"Welcome to the matrix {user.Email}");
                
                if (user.HasRole("admin"))
                {
                    AdminMenu(database);
                }
                else
                {
                    UserMenu(database);
                }

                // show main menu if user, or admin menu if admin ...
                // check user role
                break;
                

            }

        }

        public void ShowLoginError() 
        {
            Console.Clear();
        }
    }
}
