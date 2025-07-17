using DoTungDuongDAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTungDuongDAL.Repositories
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
    {
        public NewsArticleRepository(FunewsManagementContext context) : base(context) { }

        public IEnumerable<NewsArticle> GetNewsWithTags()
        {
            return _context.NewsArticles.Include(n => n.Tags).ToList();
        }

        public void AddNewsWithTags(NewsArticle news, List<int> tagIds)
        {
            news.Tags = _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
            Add(news);
        }

        public void UpdateNewsWithTags(NewsArticle news, List<int> tagIds)
        {
            var existing = GetById(news.NewsArticleId);
            if (existing != null)
            {
                _context.Entry(existing).Collection(e => e.Tags).Load();
                existing.Tags.Clear();
                existing.Tags = _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                Update(existing);
            }
        }
    }
}
