using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FarmersMarket.Model
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; }
		public string CreditCard { get; set; }
		public string Gender { get; set; }
		//public string PhotoLocation { get; set; }
		public string AboutMe { get; set; }

		public string Location { get; set; }

		public bool LoginCheck { get; set; }

		public string? ImageURL { get; set; }
	}
}
