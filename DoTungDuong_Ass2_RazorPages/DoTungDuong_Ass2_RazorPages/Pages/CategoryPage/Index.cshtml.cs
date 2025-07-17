using DoTungDuongBLL.Services;
using DoTungDuongDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Staff")]
public class CategoryIndexModel : PageModel
{
    private readonly CategoryService _categoryService;

    public CategoryIndexModel(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public IEnumerable<Category> Categories { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchKeyword { get; set; }

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
        return new JsonResult(new { success = false, errors = ModelState.Errors });
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