using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanner
{
    class ChatMessage
    {
        public int Id;
        public User User;
        public Event Event;
        public DateTime Date;
        public string Message;
    }
}
