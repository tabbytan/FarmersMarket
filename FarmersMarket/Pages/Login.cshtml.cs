namespace FarmersMarket.Pages

{
	public class LoginModel : PageModel


	{

		[BindProperty]
		public Login LModel { get; set; }

		private readonly AuthDbContext authDbContext;
		private readonly GoogleCaptchaService _captchaservice;

		private readonly IHttpContextAccessor contxt;

		private readonly SignInManager<ApplicationUser> signInManager;

		private readonly UserManager<ApplicationUser> userManager;
		public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, AuthDbContext authDbContext, GoogleCaptchaService captchaservice)
		public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, AuthDbContext authDbContext, GoogleCaptchaService captchaservice)
		{
			this.signInManager = signInManager;
			contxt = httpContextAccessor;
			this.userManager = userManager;
			this.authDbContext = authDbContext;
			_captchaservice = captchaservice;
			this.authDbContext = authDbContext;
			_captchaservice = captchaservice;
		}

		public void OnGet()
		{
		}
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
		{

			var captchaResult = await _captchaservice.VerifyToken(LModel.Token);
			if (!captchaResult)
			{
				return Page();
			}
			if (ModelState.IsValid)
			{

				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
				LModel.RememberMe, true);
				if (identityResult.Succeeded)

				{
					var user = await userManager.FindByEmailAsync(LModel.Email);
					var context = authDbContext;
					if (user.LoginCheck == true)
					{
						var context2 = authDbContext;
						var infomation2 = new AuditLog { Email = LModel.Email, Action = "Tried to log in another place at " + DateTime.Now };
						context2.AuditLog.Add(infomation2);
						context2.SaveChanges();


						OnKickOutAsync();
					}
					else
					{
						contxt?.HttpContext?.Session.SetString("FullName", user.FullName);
						contxt?.HttpContext?.Session.SetString("PasswordHash", user.PasswordHash);
						contxt?.HttpContext?.Session.SetString("Email", user.Email);
						contxt?.HttpContext?.Session.SetString("Gender", user.Gender);
						contxt?.HttpContext?.Session.SetString("PhoneNumber", user.PhoneNumber);
						contxt?.HttpContext?.Session.SetString("CreditCard", user.CreditCard);
						contxt?.HttpContext?.Session.SetString("AboutMe", user.AboutMe);
						contxt?.HttpContext?.Session.SetString("Location", user.Location);
						contxt?.HttpContext?.Session.SetString("photo", user.ImageURL);

						var identity = userManager.FindByIdAsync(user.Id).Result;
						identity.LoginCheck = true;
						IdentityResult result = userManager.UpdateAsync(identity).Result;

						var infomation = new AuditLog { Email = LModel.Email, Action = "Login at " + DateTime.Now };
						context.AuditLog.Add(infomation);

						context.SaveChanges();

						return RedirectToPage("Index");
					}
				}
				ModelState.AddModelError("", "Username or Password incorrect");

			}
			return Page();
		}
		public async Task<IActionResult> OnKickOutAsync()
		{
			await signInManager.SignOutAsync();
			return RedirectToPage("Login");
		}
	}
}
