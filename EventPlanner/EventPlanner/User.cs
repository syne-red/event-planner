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
        public List<string> Roles = new List<string>();

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}
