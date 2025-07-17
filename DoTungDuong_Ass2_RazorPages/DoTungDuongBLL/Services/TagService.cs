using DoTungDuongDAL.Models;
using DoTungDuongDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DoTungDuongBLL.Services
{
    public class TagService
    {
        private readonly IRepository<Tag> _repository;

        public TagService(IRepository<Tag> repository)
        {
            _repository = repository;
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return _repository.GetAll();
        }

        public Tag GetTagById(int id)
        {
            return _repository.GetById(id);
        }

        public void AddTag(Tag tag)
        {
            if (string.IsNullOrEmpty(tag.TagName))
                throw new ArgumentException("Tag name is required.");

            _repository.Add(tag);
        }

        public void UpdateTag(Tag tag)
        {
            var existing = _repository.GetById(tag.TagId);
            if (existing == null)
                throw new ArgumentException("Tag not found.");

            existing.TagName = tag.TagName;
            existing.Note = tag.Note;

            _repository.Update(existing);
        }

        public void DeleteTag(int id)
        {
            var tag = _repository.GetById(id);
            if (tag == null)
                throw new ArgumentException("Tag not found.");

            _repository.Delete(tag);
        }

        public IEnumerable<Tag> SearchTags(string keyword)
        {
            return _repository.Search(t => t.TagName.Contains(keyword) || t.Note.Contains(keyword));
        }
    }
}
