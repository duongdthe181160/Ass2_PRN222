using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace DoTungDuong_Ass2_RazorPages.Pages.HistoryPage
{
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        private readonly NewsArticleService _service;

        public IndexModel(NewsArticleService service) { _service = service; }

        public IEnumerable<NewsArticle> NewsHistory { get; set; }

        public void OnGet()
        {
            var userId = short.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            NewsHistory = _service.GetHistoryByUser(userId);
        }
    }
}