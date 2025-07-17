using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using System.Collections.Generic;

namespace DoTungDuong_Ass2_RazorPages.Pages.ViewNewsPage
{
    [Authorize(Roles = "Lecturer,Staff,Admin")] // Or remove [Authorize] if no auth needed for view
    public class IndexModel : PageModel
    {
        private readonly NewsArticleService _service;

        public IndexModel(NewsArticleService service) { _service = service; }

        public IEnumerable<NewsArticle> ActiveNews { get; set; }

        public void OnGet()
        {
            ActiveNews = _service.GetActiveNews();
        }
    }
}