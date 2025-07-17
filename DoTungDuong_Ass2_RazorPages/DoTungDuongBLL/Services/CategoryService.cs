using DoTungDuongDAL.Models;
using DoTungDuongDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DoTungDuongBLL.Services
{
    public class CategoryService
    {
        private readonly IRepository<Category> _repository;

        public CategoryService(IRepository<Category> repository)
        {
            _repository = repository;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _repository.GetAll();
        }

        public Category GetCategoryById(short id)
        {
            return _repository.GetById(id);
        }

        public void AddCategory(Category category)
        {
            _repository.Add(category);
        }

        public void UpdateCategory(Category category)
        {
            _repository.Update(category);
        }

        public bool CanDeleteCategory(short id)
        {
            return !_repository.Search(c => c.NewsArticles.Any(n => n.CategoryId == id)).Any();
        }

        public void DeleteCategory(short id)
        {
            var category = GetCategoryById(id);
            if (CanDeleteCategory(id))
            {
                _repository.Delete(category);
            }
        }

        public IEnumerable<Category> SearchCategories(string keyword)
        {
            return _repository.Search(c => c.CategoryName.Contains(keyword));
        }
    }
}