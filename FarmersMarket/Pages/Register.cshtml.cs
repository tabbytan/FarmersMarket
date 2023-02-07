using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FarmersMarket.ViewModels;
using FarmersMarket.Model;
using Microsoft.AspNetCore.DataProtection;
using System.Runtime.Intrinsics.X86;

namespace FarmersMarket.Pages
{
    public class RegisterModel : PageModel
    {

        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }
		public string[] Genders { get; set; } = new[] { "Male", "Female", "Unspecified" };

		public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }



        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)

            {

                var dataProtectionProvider = DataProtectionProvider.Create(RModel.Email);
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    CreditCard = protector.Protect(RModel.CreditCard),
                    FullName = RModel.FullName,
                    PhoneNumber = RModel.PhoneNumber,
                    Gender = RModel.Gender,
                    AboutMe = RModel.AboutMe,
                    Location= RModel.Location,

					// Note : to display the encrypted data, create the protector instance with the same secret string and use the unprotect method.


				};
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }







    }
}
