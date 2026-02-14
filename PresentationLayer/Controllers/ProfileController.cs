using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.ApplicationServices.Profile;
using BusinessLayer.DomainServices.Interfaces;
using BusinessLayer.DTOs.Attachment;
using BusinessLayer.DTOs.Profile;
using DataLayer.Enums;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PresentationLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Utils.Constants;
using Utils.Enums;
using Utils.Helpers;
using ImageResult = Utils.Enums.FileResult;

namespace PresentationLayer.Controllers
{
    [Authorize]
    [Route("profile/")]
    public class ProfileController : Controller
    {
        #region Dependency Injection

        private readonly IMapper _mapper;
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileService _profileService;
        private readonly IAuthService _authService;
        private readonly IFriendshipService _friendshipService;
        private readonly UploadAvatarService _avatarService;

        public ProfileController(
            IMapper mapper,
            ILogger<ProfileController> logger,
            IProfileService profileService,
            IAuthService authService,
            IFriendshipService friendshipService,
            UploadAvatarService avatarService
            )
        {
            _mapper = mapper;
            _logger = logger;
            _profileService = profileService;
            _authService = authService;
            _friendshipService = friendshipService;
            _avatarService = avatarService;
        }

        #endregion


        //Public profile
        [HttpGet("{username}")]
        public async Task<IActionResult> Other(string username)
        {
            try
            {
                Guard.Against.NullOrWhiteSpace(username, nameof(username));

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var profileDTO = await _profileService.GetUserByNameAsync(username);
                if (profileDTO != null && currentUserId != profileDTO.UserId)
                {
                    var profileVM = _mapper.Map<ProfileGetVM>(profileDTO);

                    var friendshipDTO = await _friendshipService.GetFriendshipBetweenUsersAsync(currentUserId, profileDTO.UserId);

                    profileVM.Friendship = new FriendshipStatusVM();

                    if (friendshipDTO != null)
                    {
                        profileVM.Friendship.FriendshipId = friendshipDTO.FriendshipId;
                        profileVM.Friendship.IsFriend =
                            friendshipDTO.Status == FriendshipStatus.Accepted;
                        profileVM.Friendship.IsPending =
                            friendshipDTO.Status == FriendshipStatus.Pending;
                        profileVM.Friendship.IsIncomingRequest =
                            friendshipDTO.Status == FriendshipStatus.Pending &&
                            friendshipDTO.RecipientId == currentUserId;
                        profileVM.Friendship.IsOutgoingRequest =
                            friendshipDTO.Status == FriendshipStatus.Pending &&
                            friendshipDTO.RequesterId == currentUserId;
                    }

                    return View(nameof(Index), profileVM);
                }
                else return NotFound();
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Other));
                throw;
            }
        }

        //Private profile (own user)
        [HttpGet("u")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var profileDTO = await _profileService.GetUserByIdAsync(currentUserId);
                var profileVM = _mapper.Map<ProfileGetVM>(profileDTO);

                var incomingRequests = await _friendshipService.GetIncomingRequestsAsync(currentUserId);

                profileVM.IncomingRequests = incomingRequests
                    .Select(f => new FriendshipStatusVM
                    {
                        FriendshipId = f.FriendshipId,
                        RequesterId = f.RequesterId,
                        RequesterUsername = f.RequesterName
                    })
                    .ToList();



                profileVM.Friendship = null;

                //Get user's friends
                var friendsDTO = await _friendshipService.GetFriendsAsync(currentUserId);

                var friendsVM = friendsDTO.Select(f => new FriendVM
                {
                    UserId = f.RequesterId == currentUserId ? f.RecipientId : f.RequesterId,
                    //UserName = f.RequesterId == currentUserId ? f.RecipientName : f.UserName,
                    UserName = f.UserName,
                    //AvatarUrl = f.RequesterId == currentUserId ? f.RecipientAvatarUrl : f.RequesterAvatarUrl
                    AvatarUrl = f.AvatarUrl
                }).ToList();

                profileVM.Friends = friendsVM;



                return View(profileVM);
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        //Get Edit View
        [HttpGet("u/edit")]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var profileDTO = await _profileService.GetUserByIdAsync(id);
                var profileVM = _mapper.Map<ProfileEditVM>(profileDTO);

                //profileVM.AvatarUrl = $"{imgPath}\\{profileVM.AvatarUrl}";


                //var imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                //profileVM.ImageUrl = $"{imgPath}\\{profileVM.ImageUrl}";

                //if (commentText != null)
                //{
                //    profileVM.Comment.Content = commentText;
                //    //HttpContext.Request.Query = null;
                //}

                profileVM.CurrentUserName = profileVM.UserName;

                return View(profileVM);
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost("u/edit")]
        public async Task<IActionResult> Edit([FromForm] ProfileEditVM profileVM)
        {
            try
            {
                #region Password Validation

                //Check if the inputted password is correct for this user
                if (!string.IsNullOrWhiteSpace(profileVM.CurrentPassword))
                {
                    if (!await _authService.CheckPasswordAsync(profileVM.Email, profileVM.CurrentPassword))
                        ModelState.AddModelError("CurrentPassword", "Password is wrong");
                }

                if (!ModelState.IsValid)
                    return View(profileVM);

                //Validate new password
                var passwordErrors = await _authService.ValidatePasswordAsync(profileVM.Password);
                if (passwordErrors.Count > 0)
                {
                    //If the password doesn't have an uppercase, lowercase or a special char
                    TempData["Error"] = "Password has extra errors:<br />";

                    foreach (var error in passwordErrors)
                        TempData["Error"] += "- " + HtmlEncoder.Default.Encode(error) + "<br />";

                    return View(profileVM);
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(profileVM.Bio))
                {
                    //Validate Bio; Username is validated by default
                    profileVM.Bio = FilterTextInputHelper.ValidateText(profileVM.Bio);
                }

                profileVM.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var profileDTO = _mapper.Map<ProfileEditDTO>(profileVM);             

                var success = await _profileService.EditAsync(profileDTO);
                if (success)
                {
                    TempData["PopupMessage"] = "Profile updated successfully!";
                    TempData["PopupType"] = "success";
                }
                else
                {
                    TempData["PopupMessage"] = "Error while updating profile!";
                    TempData["PopupType"] = "error";
                }

                return View(nameof(Edit));
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Edit));
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost("u/edit/avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarVM avatarVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    //return View(nameof(Edit));
                    return RedirectToAction(nameof(Edit));

                AttachmentCreateDTO avatarDTO = null;
                var imageResult = FileValidationHelper.ValidateFile(avatarVM.Image, FileCategory.Avatar);
                switch (imageResult)
                {
                    case ImageResult.Success:
                        avatarDTO = new AttachmentCreateDTO
                        {
                            FileName = FileValidationHelper.ValidateFileName(avatarVM.Image.FileName, FileStorageDirectory.Upload),
                            FileType = avatarVM.Image.ContentType,
                            FileSize = avatarVM.Image.Length,
                            FileStorageType = FileStorageType.FileSystem,
                            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            CreatedBy = DataCreation.User,
                            UpdatedBy = DataCreation.User
                        };
                        break;
                    case ImageResult.InvalidDimensions:
                        ModelState.AddModelError("Image", $"Image must be between {ImageDimensions.POST_IMAGE_MIN_WIDTH}x{ImageDimensions.POST_IMAGE_MIN_HEIGHT} and " +
                            $" {ImageDimensions.POST_IMAGE_MAX_WIDTH}x{ImageDimensions.POST_IMAGE_MAX_HEIGHT} pixels.");
                        break;
                    case ImageResult.InvalidFormat:
                        ModelState.AddModelError("Image", "Uploaded file is not a valid image");
                        break;
                    default:
                        Guard.Against.EnumOutOfRange(imageResult, nameof(imageResult));
                        break;
                }

                if (!ModelState.IsValid)
                    return View(nameof(Edit));


                var successful = await _avatarService.AddAvatarAsync(avatarDTO, avatarVM.Image);
                if (successful)
                {
                    TempData["PopupMessage"] = "Avatar uploaded successfully!";
                    TempData["PopupType"] = "success";
                }
                else
                {
                    TempData["PopupMessage"] = "An error ocurred when uploading the avatar!";
                    TempData["PopupType"] = "error";
                }

                return View(nameof(Edit));
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Edit));
                throw;
            }
        }

        #region Friendship Actions

        public async Task<IActionResult> GetFriends()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var friends = await _friendshipService.GetFriendsAsync(userId);

            return View(friends);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("{username}/remove-friend")]
        public async Task<IActionResult> RemoveFriend(string userId, string username)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _friendshipService.RemoveFriendAsync(currentUserId, userId);
            }
            else return NotFound();

            return RedirectToAction(nameof(Other), new { username });
        }

        [ValidateAntiForgeryToken]
        [HttpPost("{username}/accept-request")]
        public async Task<IActionResult> AcceptRequest(int friendshipId, string username)
        {
            if (friendshipId > 0)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _friendshipService.AcceptRequestAsync(friendshipId, currentUserId);
            }
            else return NotFound();

            //return RedirectToAction(nameof(Other), new { username });
            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("{username}/decline-request")]
        public async Task<IActionResult> DeclineRequest(int friendshipId, string username)
        {
            if (friendshipId > 0)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _friendshipService.DeclineRequestAsync(friendshipId, currentUserId);
            }
            else return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("{username}/send-request")]
        public async Task<IActionResult> SendRequest(string recipientId, string username)
        {
            if (!string.IsNullOrWhiteSpace(recipientId))
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _friendshipService.SendRequestAsync(currentUserId, recipientId);
            }
            else return NotFound();

            return RedirectToAction(nameof(Other), new { username });
        }

        #endregion
    }
}
