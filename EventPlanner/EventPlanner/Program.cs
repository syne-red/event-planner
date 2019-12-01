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
            eventManager.Start();
        }
    }
}
