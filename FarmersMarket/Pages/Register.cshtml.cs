using FarmersMarket.Model;
using FarmersMarket.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace FarmersMarket.Pages
{
    public class RegisterModel : PageModel
    {

        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager;
        private IWebHostEnvironment _environment;

        [BindProperty]
        public Register RModel { get; set; }
        [BindProperty]
        public IFormFile? Upload { get; set; }

        public string[] Genders { get; set; } = new[] { "Male", "Female", "Unspecified" };

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _environment = environment;
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

                if (Upload != null)
                {
                    var uploadsFolder = "uploads";
                    var imageFile = Guid.NewGuid() + Path.GetExtension(
                    Upload.FileName);
                    var imagePath = Path.Combine(_environment.ContentRootPath,
                    "wwwroot", uploadsFolder, imageFile);
                    using var fileStream = new FileStream(imagePath,
                    FileMode.Create);
                    await Upload.CopyToAsync(fileStream);

                    var encodestring = HttpUtility.HtmlEncode(RModel.AboutMe);



                    var user = new ApplicationUser()
                    {
                        UserName = RModel.Email,
                        Email = RModel.Email,
                        CreditCard = protector.Protect(RModel.CreditCard),
                        FullName = RModel.FullName,
                        PhoneNumber = RModel.PhoneNumber,
                        Gender = RModel.Gender,
                        AboutMe = encodestring,
                        Location = RModel.Location,
                        LoginCheck = false,
                        ImageURL = string.Format("/{0}/{1}", uploadsFolder, imageFile)



                        // Note : to display the encrypted data, create the protector instance with the same secret string and use the unprotect method.

                    };
                    //Create the Admin role if NOT exist
                    IdentityRole role = await roleManager.FindByIdAsync("Admin");
                    if (role == null)
                    {
                        IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                        if (!result2.Succeeded)
                        {
                            ModelState.AddModelError("", "Create role admin failed");
                        }
                    }
                    var result = await userManager.CreateAsync(user, RModel.Password);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddToRoleAsync(user, "Admin");
                        await signInManager.SignInAsync(user, false);
                        return RedirectToPage("Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }





            }
            return Page();
        }







    }
}
