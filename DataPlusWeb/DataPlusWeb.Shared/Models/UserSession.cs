using System;

namespace DataPlusWeb.Shared.Models
{
    public class UserSession
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
