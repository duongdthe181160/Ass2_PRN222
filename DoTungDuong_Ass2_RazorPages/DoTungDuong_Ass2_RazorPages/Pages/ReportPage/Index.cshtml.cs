using DoTungDuongBLL.Services;
using DoTungDuongDAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace DoTungDuong_Ass2_RazorPages.Pages.ReportPage
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly NewsArticleService _service;

        public IndexModel(NewsArticleService service) { _service = service; }

        public IEnumerable<ReportDTO> ReportData { get; set; } = new List<ReportDTO>();
        [BindProperty] public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
        [BindProperty] public DateTime EndDate { get; set; } = DateTime.Now;

        public void OnGet() { }

        public void OnPost()
        {
            ReportData = _service.GetReport(StartDate, EndDate);
        }
    }
}