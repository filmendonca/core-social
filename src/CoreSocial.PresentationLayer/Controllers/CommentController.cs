using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.ApplicationServices.Reaction;
using BusinessLayer.DomainServices.Interfaces;
using BusinessLayer.DTOs.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PresentationLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils.Helpers;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        #region Dependency Injection

        private readonly IMapper _mapper;
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentService _commentService;
        private readonly ReactionService _reactionService;

        public CommentController(ICommentService commentService, ReactionService reactionService, ILogger<CommentController> logger, IMapper mapper)
        {
            _commentService = commentService;
            _reactionService = reactionService;
            _logger = logger;
            _mapper = mapper;
        }

        #endregion

        public IActionResult Index() => View();

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> React([FromForm] ReactionVM reactionVM)
        {
            if (reactionVM.CommentId > 0 && reactionVM.PostId > 0)
            {
                reactionVM.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var successful = await _reactionService.ReactToCommentAsync(reactionVM.CommentId, reactionVM.UserId, reactionVM.Type);
                if (successful)
                {
                    TempData["PopupMessage"] = "Reaction added successfully!";
                    TempData["PopupType"] = "success";
                }
                else
                {
                    TempData["PopupMessage"] = "An error ocurred when adding the reaction!";
                    TempData["PopupType"] = "error";
                    return RedirectToAction("View", "Post", new
                    {
                        id = reactionVM.PostId
                    });
                }
            }
            else return NotFound();

            return RedirectToAction("View", "Post", new
            {
                id = reactionVM.PostId
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CommentCreateVM commentVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("View", "Post", new
                    {
                        id = commentVM.PostId,
                        commentText = commentVM.Content
                    });

                    //return View("View", commentVM);
                }

                commentVM.Content = FilterTextInputHelper.ValidateText(commentVM.Content);
                commentVM.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var commentDTO = _mapper.Map<CommentCreateDTO>(commentVM);
                var successful = await _commentService.AddAsync(commentDTO);
                if (successful)
                {
                    TempData["PopupMessage"] = "Comment added successfully!";
                    TempData["PopupType"] = "success";
                }
                else
                {
                    TempData["PopupMessage"] = "An error ocurred when adding the comment!";
                    TempData["PopupType"] = "error";
                    return RedirectToAction("View", "Post", new
                    {
                        id = commentVM.PostId
                    });
                }

                return RedirectToAction("View", "Post", new
                {
                    id = commentVM.PostId
                });
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] CommentEditVM commentVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("View", "Post", new
                    {
                        id = commentVM.PostId,
                        commentText = commentVM.Content
                    });

                    //return View("View", commentVM);
                }

                commentVM.Content = FilterTextInputHelper.ValidateText(commentVM.Content);
                commentVM.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var commentDTO = _mapper.Map<CommentEditDTO>(commentVM);
                await _commentService.EditAsync(commentDTO);

                return RedirectToAction("View", "Post", new
                {
                    id = commentVM.PostId
                });
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int postId)
        {
            try
            {
                if (id <= 0 || postId <= 0)
                {
                    TempData["PopupMessage"] = "An error ocurred when deleting the post!";
                    TempData["PopupType"] = "error";
                    return RedirectToAction("Index", "Post");
                }

                await _commentService.DeleteAsync(id);
                return RedirectToAction("View", "Post", new
                {
                    id = postId
                });
            }
            catch (ArgumentException ex)
            {
                throw;
            }
        }
    }
}
