using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EventPlanner
{
    class Program
    {
        static void Main(string[] args)
        {
            // using (...) {} will call eventManager.Dispose() on the ending }
            using (EventManager eventManager = new EventManager())
            {
                eventManager.Start();
            }
        }
    }
}
