using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EventPlanner
{
    class Program
    {
        static void Main(string[] args)
        {
            EventManager eventManager = new EventManager();
            string hash = Hasher.Hash("1234");
            Logger.WriteLine(hash);
            eventManager.Start();
        }
    }
}
