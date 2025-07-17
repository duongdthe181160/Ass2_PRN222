using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DoTungDuong_Ass2_RazorPages.Pages.CategoryPage
{
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        private readonly CategoryService _categoryService;

        public IndexModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;

        public void OnGet()
        {
            Categories = string.IsNullOrEmpty(SearchKeyword) ? _categoryService.GetAllCategories() : _categoryService.SearchCategories(SearchKeyword);
        }

        public IActionResult OnPostAdd(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.AddCategory(category);
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false, errors = ModelState });
        }

        // Similar for Update, Delete with confirm
        public IActionResult OnPostDelete(short id)
        {
            if (_categoryService.CanDeleteCategory(id))
            {
                _categoryService.DeleteCategory(id);
                return new JsonResult(new { success = true });
            }
            return new JsonResult(new { success = false, message = "Cannot delete" });
        }
    }
}