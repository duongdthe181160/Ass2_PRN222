using DoTungDuong_Ass2_RazorPages.Hubs;
using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DoTungDuong_Ass2_RazorPages.Pages.NewsArticlePage
{
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        private readonly NewsArticleService _service;
        private readonly TagService _tagService;
        private readonly CategoryService _categoryService;
        private readonly IHubContext<NewsHub> _hubContext;

        public IndexModel(NewsArticleService service, TagService tagService, CategoryService categoryService, IHubContext<NewsHub> hubContext)
        {
            _service = service;
            _tagService = tagService;
            _categoryService = categoryService;
            _hubContext = hubContext;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        [BindProperty] public NewsArticle NewsArticle { get; set; } = new NewsArticle();
        public IEnumerable<Tag> Tags { get; set; } = new List<Tag>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        [BindProperty] public List<int> SelectedTags { get; set; } = new List<int>();

        public void OnGet(string search = null)
        {
            NewsArticles = string.IsNullOrEmpty(search) 
                ? _service.GetAll() 
                : _service.Search(n => n.Headline != null && n.Headline.Contains(search));
            Tags = _tagService.GetAllTags();
            Categories = _categoryService.GetAllCategories();
        }

        public async Task<IActionResult> OnPostAdd()
        {
            try
            {
                if (string.IsNullOrEmpty(NewsArticle.NewsArticleId))
                {
                    NewsArticle.NewsArticleId = Guid.NewGuid().ToString();
                }
                
                NewsArticle.CreatedById = short.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                _service.Add(NewsArticle, SelectedTags ?? new List<int>());
                
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "New news article added");
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            try
            {
                NewsArticle.UpdatedById = short.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                _service.Update(NewsArticle, SelectedTags ?? new List<int>());
                
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "News article updated");
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDelete(string id)
        {
            try
            {
                _service.Delete(id);
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "News article deleted");
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
        }
    }
}