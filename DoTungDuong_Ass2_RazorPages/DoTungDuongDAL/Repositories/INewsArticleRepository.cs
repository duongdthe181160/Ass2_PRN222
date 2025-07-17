using DoTungDuongDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoTungDuongDAL.Repositories
{
    public interface INewsArticleRepository : IRepository<NewsArticle>
    {
        IEnumerable<NewsArticle> GetNewsWithTags();
        void AddNewsWithTags(NewsArticle news, List<int> tagIds);
        void UpdateNewsWithTags(NewsArticle news, List<int> tagIds);
    }
}
