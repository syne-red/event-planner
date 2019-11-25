using System;

namespace EventPlanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var DB = new Database();
            DB.GetUserById(0);
        }
    }
}
