using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using System.Collections.Generic;
using System;

namespace DoTungDuong_Ass2_RazorPages.Pages.AccountPage
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly SystemAccountService _service;

        public IndexModel(SystemAccountService service) { _service = service; }

        public IEnumerable<SystemAccount> Accounts { get; set; } = new List<SystemAccount>();
        [BindProperty] public SystemAccount Account { get; set; } = new SystemAccount();

        public void OnGet(string search = null)
        {
            Accounts = string.IsNullOrEmpty(search) ? _service.GetAllAccounts() : _service.SearchAccounts(search);
        }

        public IActionResult OnPostAdd()
        {
            try
            {
                _service.AddAccount(Account);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostUpdate()
        {
            try
            {
                _service.UpdateAccount(Account);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(short id)
        {
            try
            {
                _service.DeleteAccount(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
            return RedirectToPage();
        }
    }
}