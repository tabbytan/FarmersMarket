using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace FarmersMarket.ViewModels
{
    public class Register
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
		[MinLength(12, ErrorMessage = "Enter at least a 12 characters password")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$", ErrorMessage = "Passwords must be at least 12 characters long and contain at least an upper case letter, lower case letter, digit and a symbol")]
		public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
		[DataType(DataType.CreditCard)]
        [CreditCard]
		public string CreditCard { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Location { get; set; }
        [Required]
        public string AboutMe { get; set; }

        public string? ImageURL { get; set; }
      





    }
}
