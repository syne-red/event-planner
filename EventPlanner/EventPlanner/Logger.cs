using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace EventPlanner
{
    public static class Logger
    {
        /// <summary>
        /// Writes to the console and the Visual Studio debug window
        /// </summary>
        /// <param name="message">Message to write</param>
        public static void Write(string message)
        {
            Console.Write(message);
            Debug.Write(message);
        }

        /// <summary>
        /// Writes to the console and the Visual Studio debug window and appends a new line.
        /// </summary>
        /// <param name="message">Message to write</param>
        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Reads an input from the user and writes it out to Visual Studio Debug console.
        /// </summary>
        public static string ReadLine()
        {
            string input = Console.ReadLine();
            Debug.WriteLine(input);
            return input;
        }

        /// <summary>
        /// Reads a masked password from the console.
        /// </summary>
        public static string ReadPassword()
        {
            StringBuilder stringBuffer = new StringBuilder();
            while (true)
            {
                // check if the user has pressed any key
                if (Console.KeyAvailable)
                {
                    // read the key from the console input stream
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        // user pressed backspace
                        if (stringBuffer.Length != 0)
                        {
                            // if we had some input in stringbuffer we remove one character
                            stringBuffer.Remove(stringBuffer.Length - 1, 1);

                            // remove one character from the console
                            Console.Write("\b \b");
                        }
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        // user pressed enter

                        string input = stringBuffer.ToString();
                        Console.WriteLine();

                        // write out the password masked by * to the Visual Studio debug console
                        Debug.WriteLine("".PadRight(input.Length, '*'));
                        return input;
                    }
                    else
                    {
                        // add the char the user pressed to the console and the string buffer
                        stringBuffer.Append(key.KeyChar);
                        Console.Write("*");
                    }
                }

                Thread.Sleep(50);
            }
        }
    }
}
