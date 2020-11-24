using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public override string ToString()
        {
            return $"UserId: {Id} Username: {Username}";
        }
    }
}
