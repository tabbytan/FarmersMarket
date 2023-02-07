using FarmersMarket.Model;
using FarmersMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FarmersMarket.Pages
{
    public class LogoutModel : PageModel
    {

        private readonly IHttpContextAccessor contxt;

        [BindProperty]
		public Login LModel { get; set; }

        private readonly AuthDbContext authDbContext;

        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly UserManager<ApplicationUser> userManager;
		public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, AuthDbContext authDbContext, UserManager<ApplicationUser> userManager)
		{
			this.signInManager = signInManager;
            contxt = httpContextAccessor;
            this.authDbContext = authDbContext;
            this.userManager = userManager;
        }
		public void OnGet() { }
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
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
