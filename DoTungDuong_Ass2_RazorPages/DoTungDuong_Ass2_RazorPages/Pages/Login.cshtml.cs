using DoTungDuongBLL.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace DoTungDuong_Ass2_RazorPages.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SystemAccountService _accountService;

        public LoginModel(SystemAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            var principal = _accountService.Authenticate(Email, Password);
            if (principal != null)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                return role switch
                {
                    "Admin" => RedirectToPage("/Account/Index"),
                    "Staff" => RedirectToPage("/Category/Index"),
                    "Lecturer" => RedirectToPage("/News/ActiveList"),
                    _ => RedirectToPage("/Index")
                };
            }

            ErrorMessage = "Invalid login credentials.";
            return Page();
        }
    }
}
