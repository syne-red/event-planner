using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EventPlanner
{
    class Validation
    {
        //^(\w| ){3,255}$

        public static bool IsValidEmail(string email)
        {

            Regex emailRegex = new Regex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");

            return emailRegex.IsMatch(email);
        }

        public static bool IsValidEventName(string name)
        {
            Regex nameRegex = new Regex("^(\\w| ){3,255}$");

            return nameRegex.IsMatch(name);
        }

        public static bool IsValidDescription(string description)
        {
            Regex descriptionRegex = new Regex("^(\\w| ){3,255}$");

            return descriptionRegex.IsMatch(description);
        }

        public static bool IsValidMaxParticipants(int number)
        {
            return number >= 1 && number <= 10000;
        }

        public static bool IsValidEventDate(DateTime date)
        {
            return date >= DateTime.Now;
        }

        public static bool IsValidLocation(string location)
        {
            Regex locationRegex = new Regex("^(\\w| ){3,255}$");

            return locationRegex.IsMatch(location);
        }
    }
}
