using DoTungDuongDAL;
using DoTungDuongDAL.Models;
using DoTungDuongDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DoTungDuongBLL.Services
{
    public class NewsArticleService
    {
        private readonly INewsArticleRepository _repository;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Tag> _tagRepo;

        public NewsArticleService(
            INewsArticleRepository repository,
            IRepository<Tag> tagRepo,
            IRepository<Category> categoryRepo)
        {
            _repository = repository;
            _tagRepo = tagRepo;
            _categoryRepo = categoryRepo;
        }

        public IEnumerable<NewsArticle> GetAll() => _repository.GetNewsWithTags();

        public NewsArticle? GetById(string id) => _repository.GetById(id);

        public void Add(NewsArticle article, List<int> tagIds)
        {
            if (string.IsNullOrEmpty(article.Headline)) 
                throw new ArgumentException("Headline is required");
            
            if (string.IsNullOrEmpty(article.NewsArticleId))
                throw new ArgumentException("NewsArticleId is required");
            
            article.CreatedDate = DateTime.Now;
            article.NewsStatus = article.NewsStatus ?? true; // Default to active
            
            // Use the repository method that handles tags
            _repository.AddNewsWithTags(article, tagIds);
        }

        public void Update(NewsArticle article, List<int> tagIds)
        {
            if (string.IsNullOrEmpty(article.Headline)) 
                throw new ArgumentException("Headline is required");
            
            var existing = _repository.GetById(article.NewsArticleId);
            if (existing == null) 
                throw new ArgumentException("Article not found");

            // Update properties
            existing.NewsTitle = article.NewsTitle;
            existing.Headline = article.Headline;
            existing.NewsContent = article.NewsContent;
            existing.NewsSource = article.NewsSource;
            existing.CategoryId = article.CategoryId;
            existing.NewsStatus = article.NewsStatus;
            existing.UpdatedById = article.UpdatedById;
            existing.ModifiedDate = DateTime.Now;

            // Use the repository method that handles tags
            _repository.UpdateNewsWithTags(existing, tagIds);
        }

        public void Delete(string id)
        {
            var article = _repository.GetById(id);
            if (article != null)
            {
                _repository.Delete(article);
            }
        }

        public IEnumerable<NewsArticle> Search(Expression<Func<NewsArticle, bool>> predicate) 
            => _repository.Search(predicate);

        public IEnumerable<NewsArticle> GetActiveNews() 
            => _repository.Search(n => n.NewsStatus == true);

        public IEnumerable<NewsArticle> GetHistoryByUser(short userId) 
            => _repository.Search(n => n.CreatedById == userId);

        public IEnumerable<ReportDTO> GetReport(DateTime start, DateTime end)
        {
            var news = _repository.Search(n => n.CreatedDate >= start && n.CreatedDate <= end);
            var categories = _categoryRepo.GetAll();

            var query = from c in categories
                        join n in news on c.CategoryId equals n.CategoryId into newsGroup
                        select new ReportDTO
                        {
                            CategoryName = c.CategoryName,
                            ActiveCount = newsGroup.Count(n => n.NewsStatus == true),
                            InactiveCount = newsGroup.Count(n => n.NewsStatus == false),
                            Total = newsGroup.Count()
                        };

            return query.OrderByDescending(r => r.Total).ToList();
        }
    }
}
