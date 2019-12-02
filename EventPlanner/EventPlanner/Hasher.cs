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
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder result = new StringBuilder();

                foreach (byte b in bytes)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
        }
    }
}
