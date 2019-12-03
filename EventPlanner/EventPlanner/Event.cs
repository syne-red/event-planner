using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    class Event
    {
        public int Id;
        public string Name;
        public string Description;
        public int MaxParticipant;
        public DateTime Date;
        public string Location;
        public List<User> Participant = new List<User>();
        public List<ChatMessage> ChatMessages = new List<ChatMessage>();
    }
}
