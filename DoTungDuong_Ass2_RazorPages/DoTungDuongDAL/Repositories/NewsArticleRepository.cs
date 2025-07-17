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
            return _context.NewsArticles
                .Include(n => n.Tags)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .ToList();
        }

        public void AddNewsWithTags(NewsArticle news, List<int> tagIds)
        {
            // Load and attach tags
            var tags = _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
            news.Tags = tags;
            
            _dbSet.Add(news);
            _context.SaveChanges();
        }

        public void UpdateNewsWithTags(NewsArticle news, List<int> tagIds)
        {
            var existing = _context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefault(n => n.NewsArticleId == news.NewsArticleId);
                
            if (existing != null)
            {
                // Update properties
                _context.Entry(existing).CurrentValues.SetValues(news);
                
                // Update tags
                existing.Tags.Clear();
                var tags = _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                foreach (var tag in tags)
                {
                    existing.Tags.Add(tag);
                }
                
                _context.SaveChanges();
            }
        }
    }
}
