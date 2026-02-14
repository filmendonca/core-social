using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utils.Models;
using X.PagedList;

namespace DataLayer.Interfaces
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<IQueryable<Post>> QueryAsync(int id);
        //Task<IPagedList<Post>> GetAllAsync(
        //    PagingParams pagingParams = null,
        //    Expression<Func<Post, bool>> expression = null,
        //    Func<IQueryable<Post>, IOrderedQueryable<Post>> orderBy = null,
        //    List<string> includes = null);
        //Task<IPagedList<Post>> GetPagedListAsync(IEnumerable<Post> posts, PagingParams pagingParams);
    }
}
