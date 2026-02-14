using BusinessLayer.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DomainServices.Interfaces
{
    public interface ICommentService
    {
        Task<bool> AddAsync(CommentCreateDTO dto);
        Task EditAsync(CommentEditDTO dto);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(int postId);
    }
}
