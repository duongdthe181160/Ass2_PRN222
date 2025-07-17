using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DoTungDuong_Ass2_RazorPages.Pages;

[AllowAnonymous]  // For public
public class IndexModel : PageModel
{
    private readonly NewsArticleService _newsService;

    public IndexModel(NewsArticleService newsService)
    {
        _newsService = newsService;
    }

    public IEnumerable<NewsArticle> NewsArticles { get; set; }

    public void OnGet()
    {
        NewsArticles = _newsService.GetAllNewsArticles(false);  // Active only
    }
}