using BusinessLayer.DTOs.Post;
using DataLayer.Base;
using DataLayer.Enums;
using DataLayer.Interfaces;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DomainServices.Interfaces
{
    public interface IPostService
    {
        Task AddAsync(PostCreateDTO dto);
        Task AddAsync(Post post);
        Task<PostGetDTO> GetByIdAsync(int id, string userId = null);
        Task<IEnumerable<PostListDTO>> GetAllAsync(PostQueryDTO queryDTO);
        Task EditAsync(PostEditDTO dto, bool saveChanges = true);
        Task DeleteAsync(int id);
        Task<bool> ChangeVisibility(int id, VisibilityType visibility);
    }
}
