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
            NewsArticles = string.IsNullOrEmpty(search) ? _service.GetAll() : _service.Search(n => n.Headline.Contains(search));
            Tags = _tagService.GetAll();
            Categories = _categoryService.GetAll();
        }

        public async Task<IActionResult> OnPostAdd()
        {
            try
            {
                NewsArticle.CreatedById = short.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _service.Add(NewsArticle, SelectedTags);
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "New news article added");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            try
            {
                NewsArticle.UpdatedById = short.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _service.Update(NewsArticle, SelectedTags);
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "News article updated");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OnGet();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(string id)
        {
            try
            {
                _service.Delete(id);
                await _hubContext.Clients.All.SendAsync("ReceiveNewsUpdate", "News article deleted");
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