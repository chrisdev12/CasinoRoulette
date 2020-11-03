using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Roulette.Core.Entities.User
{
    public class User
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
        public int Cash { get; set; }
    }
}
