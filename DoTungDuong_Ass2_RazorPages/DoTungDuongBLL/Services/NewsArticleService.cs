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
        private Func<string, Task>? _signalRNotifier;

        public NewsArticleService(
            INewsArticleRepository repository,
            IRepository<Tag> tagRepo,
            IRepository<Category> categoryRepo)
        {
            _repository = repository;
            _tagRepo = tagRepo;
            _categoryRepo = categoryRepo;
        }

        public void SetSignalRNotifier(Func<string, Task> notifier)
        {
            _signalRNotifier = notifier;
        }

        public IEnumerable<NewsArticle> GetAll() => _repository.GetNewsWithTags();

        public NewsArticle? GetById(string id) => _repository.GetById(id);

        public async Task AddAsync(NewsArticle article, List<int> tagIds)
        {
            if (string.IsNullOrEmpty(article.Headline)) throw new ArgumentException("Headline required");
            article.CreatedDate = DateTime.Now;
            _repository.Add(article);
            foreach (var tagId in tagIds)
            {
                var tag = _tagRepo.GetById(tagId);
                if (tag != null) article.Tags.Add(tag);
            }
            _repository.Update(article);

            if (_signalRNotifier != null)
                await _signalRNotifier("New article created");
        }

        public void Add(NewsArticle article, List<int> tagIds)
        {
            AddAsync(article, tagIds).Wait();
        }

        public async Task UpdateAsync(NewsArticle article, List<int> tagIds)
        {
            if (string.IsNullOrEmpty(article.Headline)) throw new ArgumentException("Headline required");
            article.ModifiedDate = DateTime.Now;
            var existing = _repository.GetById(article.NewsArticleId);
            if (existing == null) throw new ArgumentException("Article not found");

            existing.NewsTitle = article.NewsTitle;
            existing.Headline = article.Headline;
            existing.NewsContent = article.NewsContent;
            existing.NewsSource = article.NewsSource;
            existing.CategoryId = article.CategoryId;
            existing.NewsStatus = article.NewsStatus;
            existing.UpdatedById = article.UpdatedById;
            existing.ModifiedDate = article.ModifiedDate;

            existing.Tags.Clear();
            foreach (var tagId in tagIds)
            {
                var tag = _tagRepo.GetById(tagId);
                if (tag != null) existing.Tags.Add(tag);
            }

            _repository.Update(existing);

            if (_signalRNotifier != null)
                await _signalRNotifier($"Article {article.NewsArticleId} updated");
        }

        public void Update(NewsArticle article, List<int> tagIds)
        {
            UpdateAsync(article, tagIds).Wait();
        }

        public async Task DeleteAsync(string id)
        {
            var article = _repository.GetById(id);
            if (article != null)
            {
                _repository.Delete(article);
                if (_signalRNotifier != null)
                    await _signalRNotifier($"Article {id} deleted");
            }
        }

        public void Delete(string id)
        {
            DeleteAsync(id).Wait();
        }

        public IEnumerable<NewsArticle> Search(Expression<Func<NewsArticle, bool>> predicate) => _repository.Search(predicate);

        public IEnumerable<NewsArticle> GetActiveNews() => Search(n => n.NewsStatus == true).AsEnumerable();

        public IEnumerable<NewsArticle> GetHistoryByUser(short userId) => Search(n => n.CreatedById == userId);

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
