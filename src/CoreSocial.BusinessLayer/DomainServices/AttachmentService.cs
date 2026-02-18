using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.DTOs.Attachment;
using BusinessLayer.DomainServices.Interfaces;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Constants;
using Utils.Helpers;

namespace BusinessLayer.DomainServices
{
    public class AttachmentService : IAttachmentService
    {
        #region Dependency Injection

        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IMapper _mapper;

        public AttachmentService(IAttachmentRepository attachmentRepository, IMapper mapper)
        {
            _attachmentRepository = attachmentRepository;
            _mapper = mapper;
        }

        #endregion

        public async Task AddAsync(Attachment attachment)
        {
            Guard.Against.Null(attachment, nameof(attachment));
            await _attachmentRepository.AddAsync(attachment);
        }

        public async Task AddAsync(AttachmentCreateDTO dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            var attachment = _mapper.Map<Attachment>(dto);
            await _attachmentRepository.AddAsync(attachment);
        }

        public async Task EditAsync(AttachmentEditDTO dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            var attachment = await _attachmentRepository.GetAsync(att => att.Id == dto.Id);
            _mapper.Map(dto, attachment);
            await _attachmentRepository.UpdateAsync(attachment);
        }

        public async Task<int> GetIdAsync(string userId)
        {
            Guard.Against.NullOrWhiteSpace(userId, nameof(userId));
            var attachment = await _attachmentRepository.GetAsync(att => att.UserId == userId);
            return attachment.Id;
        }

        public async Task<string> GetFileNameAsync(int id)
        {
            Guard.Against.NegativeOrZero(id, nameof(id));
            var attachment = await _attachmentRepository.GetAsync(att => att.Id == id);
            return attachment.FileName;
        }

        public async Task DeleteAsync(int id)
        {
            Guard.Against.NegativeOrZero(id, nameof(id));
            await _attachmentRepository.SoftDeleteAsync(id);
        }        
    }
}
