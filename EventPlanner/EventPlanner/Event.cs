using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    class Event
    {
        public int Id;
        public User Creator;
        public string Name;
        public string Description;
        public int MaxParticipant;
        public DateTime Date;
        public string Location;
        List<User> Participant = new List<User>();
        List<ChatMessage> ChatMessages = new List<ChatMessage>();
    }
}
