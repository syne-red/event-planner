using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    class User
    {
        public int Id;
        public string Email;
        public string PasswordHash;
        List<string> Roles = new List<string>();
    }
}
