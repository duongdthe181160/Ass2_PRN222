using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoTungDuongDAL.Models;
using DoTungDuongBLL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DoTungDuong_Ass2_RazorPages.Pages.ProfilePage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SystemAccountService _accountService;

        public IndexModel(SystemAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public SystemAccount Account { get; set; }

        public IActionResult OnGet()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            Account = _accountService.GetByEmail(email);
            if (Account == null)
                return NotFound();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                _accountService.Update(Account);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage();
        }
    }
}
