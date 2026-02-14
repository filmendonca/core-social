using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.DomainServices.Interfaces;
using BusinessLayer.DTOs.Comment;
using DataLayer.Interfaces;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DomainServices
{
    public class CommentService : ICommentService
    {
        #region Dependency Injection

        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(ICommentRepository commentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public async Task<bool> AddAsync(CommentCreateDTO dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            var comment = _mapper.Map<Comment>(dto);
            await _commentRepository.AddAsync(comment);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task EditAsync(CommentEditDTO dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            //var comment = _mapper.Map<Comment>(dto);
            var comment = await _commentRepository.GetAsync(c => c.Id == dto.Id);

            _mapper.Map(dto, comment);

            await _commentRepository.UpdateAsync(comment);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Guard.Against.NegativeOrZero(id, nameof(id));
            await _commentRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        //Soft delete all comments from a post
        public async Task DeleteRangeAsync(int postId)
        {
            Guard.Against.NegativeOrZero(postId, nameof(postId));
            var comments = await _commentRepository.GetAllAsync(c => c.PostId == postId);
            Guard.Against.Null(comments, nameof(comments));

            foreach (var comment in comments)
                await _commentRepository.SoftDeleteAsync(comment.Id);

            //await _unitOfWork.SaveAsync();
        }
    }
}
