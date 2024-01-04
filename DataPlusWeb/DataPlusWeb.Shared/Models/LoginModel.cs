using System.ComponentModel.DataAnnotations;

namespace DataPlusWeb.Shared.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}
