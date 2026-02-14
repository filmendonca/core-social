using BusinessLayer.DTOs.Attachment;
using DataLayer.Interfaces;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DomainServices.Interfaces
{
    public interface IAttachmentService
    {
        Task AddAsync(AttachmentCreateDTO dto);
        Task AddAsync(Attachment attachment);
        Task EditAsync(AttachmentEditDTO dto);
        Task<int> GetIdAsync(string userId);
        Task<string> GetFileNameAsync(int id);
        Task DeleteAsync(int id);
    }
}
