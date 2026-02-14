using Ardalis.GuardClauses;
using DataLayer.Enums;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.ApplicationServices.Reaction
{
    public class ReactionService
    {
        #region Dependency Injection

        private readonly ICommentRepository _commentRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReactionService(ICommentRepository commentRepository, IReactionRepository reactionRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public async Task<bool> ReactToCommentAsync(int commentId, string userId, ReactionType reactionType)
        {
            try
            {
                Guard.Against.EnumOutOfRange(reactionType, nameof(reactionType));
                var comment = await _commentRepository.GetAsync(c => c.Id == commentId);
                Guard.Against.Null(comment, nameof(comment));

                if (comment.UserId == userId)
                    throw new Exception("You cannot react to your own comment.");

                //var existingReaction = await _reactionRepository
                //    .GetByUserAndCommentAsync(userId, commentId);

                var existingReaction = await _reactionRepository.GetAsync(r => r.UserId == userId && r.CommentId == commentId);

                if (existingReaction == null)
                {
                    //If there is no reaction then create
                    var reaction = new DataLayer.Models.Reaction
                    {
                        UserId = userId,
                        CommentId = commentId,
                        Type = reactionType
                    };

                    await _reactionRepository.AddAsync(reaction);
                }
                else if (existingReaction.Type == reactionType)
                {
                    //If there is a same reaction then remove (turn off)

                    //await _reactionRepository.DeleteAsync(existingReaction);
                    await _reactionRepository.DeleteAsync(existingReaction.Id);
                }
                else
                {
                    //If there is a different reaction then switch
                    existingReaction.Type = reactionType;
                    await _reactionRepository.UpdateAsync(existingReaction);
                }

                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

            return true;
        }
    }
}
