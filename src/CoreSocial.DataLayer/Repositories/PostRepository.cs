using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utils.Models;
using X.PagedList;

namespace DataLayer.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(AppDbContext context) : base(context) { }

        //Get queryable post
        public async Task<IQueryable<Post>> QueryAsync(int id)
        {
            return _db.Where(p => p.Id == id);

            //                    postDTO = await postQuery
            //        //.Where(p => p.Id == id)
            //        .ProjectTo<PostGetDTO>(
            //            _mapper.ConfigurationProvider,
            //            new { currentUserId = userId })
            //        .FirstOrDefaultAsync();

            //IQueryable<Post> postQuery = (IQueryable<Post>)post;

            //postQuery.Select(p => new PostGetDto
            //{
            //    Id = p.Id,
            //    Title = p.Title,
            //    Comments = p.Comments.Select(c => new CommentGetDTO
            //    {
            //        Id = c.Id,
            //        Content = c.Content,
            //        Score = c.Reactions.Sum(r => (int)r.Type)
            //    }).ToList()
            //})
            //    .FirstOrDefaultAsync();
        }

        //public async Task<IPagedList<Post>> GetAllAsync(
        //    PagingParams pagingParams = null, 
        //    Expression<Func<Post, bool>> expression = null, 
        //    Func<IQueryable<Post>, IOrderedQueryable<Post>> orderBy = null, 
        //    List<string> includes = null)
        //{
        //    //Run base method
        //    var totalPosts = await base.GetAllAsync(expression, orderBy, includes);

        //    //If no paging params were passed, use default values
        //    if (pagingParams == null)
        //        pagingParams = new();

        //    var totalPages = totalPosts.Count() / pagingParams.PageSize;
        //    totalPages = (int)Math.Ceiling((double)totalPages);

        //    //Convert entries to paged list
        //    return await totalPosts.ToPagedListAsync(pagingParams.PageNumber, pagingParams.PageSize);
        //}

        ////Get paged list
        //public async Task<IPagedList<Post>> GetPagedListAsync(IEnumerable<Post> posts, PagingParams pagingParams)
        //{
        //    ////If no paging params were passed, use default values
        //    //if (pagingParams == null)
        //    //    pagingParams = new();

        //    //var totalPages = posts.Count() / pagingParams.PageSize;
        //    //totalPages = (int)Math.Ceiling((double)totalPages);

        //    //Convert entries to paged list
        //    return await posts.ToPagedListAsync(pagingParams.PageNumber, pagingParams.PageSize);
        //}
    }
}
