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
            MainMenu();
        }

        public void MainMenu()
        {
            Database database = new Database();

            bool running = true;

            while (running)
            {
                Logger.WriteLine("--- Welcome to The event planer! ---");
                Logger.WriteLine("Select an option ");
                Logger.WriteLine(" 1.Create Traveler ");
                Logger.WriteLine(" 2.Create Event ");
                Logger.WriteLine(" 3.Log in ");
                Logger.WriteLine(" 4.Create destination ");
                Logger.WriteLine(" 5.Show events "); // list events
                Logger.WriteLine(" 0.Exit ");
                Console.Write("Input selection: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        CreateUser();
                        break;
                    case "2":
                        CreateEvent();
                        break;
                    case "3":
                        Login(database);
                        break;
                    case "4":
                        ShowEvents();
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

        public void CreateUser()
        {
            Logger.Write("Enter email: ");
            string email = Logger.ReadLine();

            Logger.Write("Enter password: ");
            string password = Logger.ReadPassword();

            string passwordHash = Hasher.Hash(password);

            Console.WriteLine($"You created user {email} with password: {passwordHash}");
        }
        
        public void CreateEvent()
        {

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

                Database database = new Database();
                database.AddEvent(1, name, description, maxParticipants, eventDate, location);
                Logger.WriteLine("Event added to database!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                break;

            }
        }


        public void ShowEvents()
        {
        }

        public void Login(Database database) 
        {
            while (true)
            {
                Logger.Write("Enter email: ");
                string email = Logger.ReadLine();

                Logger.Write("Enter password: ");
                string password = Logger.ReadPassword();

                if (!Validation.IsValidEmail(email))
                {
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
                break;
                

            }

        }

        public void ShowLoginError() 
        {
        }
    }
}
