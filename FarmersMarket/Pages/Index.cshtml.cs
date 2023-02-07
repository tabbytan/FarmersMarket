using FarmersMarket.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace FarmersMarket.Pages
{
    [Authorize]
    public class IndexModel : PageModel




    {
        private readonly ILogger<IndexModel> _logger;
		private readonly IHttpContextAccessor contxt;
        private readonly SignInManager<ApplicationUser> signInManager;
		private readonly AuthDbContext authDbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager, AuthDbContext authDbContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            contxt= httpContextAccessor;
            this.signInManager = signInManager;
			this.authDbContext = authDbContext;
            this.userManager = userManager;
        }

        public List<string> Name { get; set; } = new();



        public void OnGet()
        {


            if (contxt.HttpContext.Session.GetString("Email") != null) { 
                var UserName = contxt.HttpContext.Session.GetString("FullName");
                var Password = contxt.HttpContext.Session.GetString("PasswordHash");
                var Email = contxt.HttpContext.Session.GetString("Email");
                var Gender = contxt.HttpContext.Session.GetString("Gender");
                var PhoneNumber = contxt.HttpContext.Session.GetString("PhoneNumber");
                var dataProtectionProvider = DataProtectionProvider.Create(Email);
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var EncryptedCreditCard = contxt.HttpContext.Session.GetString("CreditCard");
                var CreditCard = protector.Unprotect(EncryptedCreditCard);
                var aboutme = contxt.HttpContext.Session.GetString("AboutMe");
                var location = contxt.HttpContext.Session.GetString("Location");

                Name.Add(UserName);
                Name.Add(Password);
                Name.Add(Email);
                Name.Add(PhoneNumber);
                Name.Add(Gender);
                Name.Add(CreditCard);
                Name.Add(aboutme);
                Name.Add(location);
            }
            else
            {
                OnPostLogoutAsync();
			}

            


		}
		public async Task<IActionResult> OnPostLogoutAsync()
		{

			if (contxt.HttpContext.Session.GetString("Email") != null)
			{
                var identity = userManager.FindByEmailAsync(contxt.HttpContext.Session.GetString("Email")).Result;
                identity.LoginCheck = false;
                IdentityResult result = userManager.UpdateAsync(identity).Result;


                var context = authDbContext;
                var infomation = new AuditLog { Email = contxt.HttpContext.Session.GetString("Email"), Action = "Logout at " + DateTime.Now };
                context.AuditLog.Add(infomation);
                context.SaveChanges();

                contxt?.HttpContext?.Session.Remove(contxt.HttpContext.Session.GetString("Email"));
				contxt.HttpContext.Session.Remove("LoginCheck");
				contxt.HttpContext.Session.Remove("FullName");
				contxt.HttpContext.Session.Remove("PasswordHash");
				contxt.HttpContext.Session.Remove("Email");
				contxt.HttpContext.Session.Remove("Gender");
				contxt.HttpContext.Session.Remove("PhoneNumber");
				contxt.HttpContext.Session.Remove("CreditCard");
			}


			await signInManager.SignOutAsync();
			return RedirectToPage("Login");
		}
	}
}