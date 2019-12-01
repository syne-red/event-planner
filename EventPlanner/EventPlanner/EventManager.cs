using System;
using System.Collections.Generic;
using System.Text;

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
                        CreateDestination();
                        break;
                    case "5":
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
            Logger.Write("Enter username: ");
            string username = Logger.ReadLine();

            Logger.Write("Enter password: ");
            string password = Logger.ReadPassword();

            string passwordHash = Hasher.Hash(password);

            Console.WriteLine($"You created user {username} with password: {passwordHash}");
        }
        
        public void CreateEvent()
        {
        }

        public void CreateDestination()
        {
        }

        public void ShowEvents()
        {
        }

        public void Login(Database database) 
        {
        }

        public void ShowLoginError() 
        {
        }
    }
}
