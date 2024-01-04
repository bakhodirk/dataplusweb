using System;

namespace DataPlusWeb.Shared.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Role { get; set; }
    }
}
