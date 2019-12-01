using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EventPlanner
{
    public static class Hasher
    {
        public static string Hash(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                return BitConverter.ToString(sha512.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
