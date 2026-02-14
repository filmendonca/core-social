using AutoMapper;
using BusinessLayer.DTOs.Post;
using DataLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using BusinessLayer.DomainServices.Interfaces;
using Microsoft.AspNetCore.Http;
using BusinessLayer.DTOs.Attachment;
using Microsoft.AspNetCore.Authorization;
using Utils.Constants;
using Newtonsoft.Json;
using System.Text.Json;
using Utils.Helpers;
using BusinessLayer.ApplicationServices.Post;
using Utils.Enums;
using ImageResult = Utils.Enums.FileResult;
using Microsoft.Extensions.Logging;
using DataLayer.Interfaces;
using Ardalis.GuardClauses;
using Utils.Models;

namespace PresentationLayer.Controllers
{
    public class PostController : Controller
    {
        #region Dependency Injection

        private readonly CrudPostService _crudPostService;
        private readonly IMapper _mapper;
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;
        private readonly IFileStorage _fileStorage;

        public PostController(
            CrudPostService crudPostService, 
            IMapper mapper, 
            ILogger<PostController> logger,
            IPostService postService,
            IFileStorage fileStorage
            )
        {
            _crudPostService = crudPostService;
            _mapper = mapper;
            _logger = logger;
            _postService = postService;
            _fileStorage = fileStorage;
        }

        #endregion

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ChangeVisibility(int id, VisibilityType visibility)
        {
            try
            {
                if (id > 0)
                {
                    var successful = await _postService.ChangeVisibility(id, visibility);
                    if (!successful)
                    {
                        TempData["PopupMessage"] = "An error ocurred when changing the visibility!";
                        TempData["PopupType"] = "error";
                        return RedirectToAction("View", routeValues: new { id });
                    }
                }
                else
                    return NotFound();

                TempData["PopupMessage"] = "Post visibility changed successfully!";
                TempData["PopupType"] = "success";
                return RedirectToAction("View", routeValues: new { id });
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        //Show all posts
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PostQueryVM queryVM)
        {
            try
            {
                //Set values on the View
                ViewData["Sort"] = queryVM.Sort;
                ViewData["NameAscSortParam"] = "name_asc";
                ViewData["NameDescSortParam"] = "name_desc";
                ViewData["DateAscSortParam"] = "date_asc";
                ViewData["DateDescSortParam"] = "date_desc";
                ViewData["PopAscSortParm"] = "pop_asc";
                ViewData["PopDescSortParm"] = "pop_desc";

                @ViewData["Topic"] = queryVM.Topic.ToString();

                if (!string.IsNullOrWhiteSpace(queryVM.SearchText))
                {
                    if (queryVM.SearchText.Length < 3)
                    {
                        TempData["Error"] = "The string inserted is too small.";
                        //return View(postVMs);
                    }

                    queryVM.SearchText = FilterTextInputHelper.ValidateText(queryVM.SearchText);
                }

                //Set popularity (desc) as the default sorting
                if (string.IsNullOrWhiteSpace(queryVM.Sort))
                    queryVM.Sort = "pop_desc";

                if (queryVM.SearchText != null)
                    queryVM.PageNumber = 1;
                else
                    queryVM.SearchText = queryVM.CurrentFilter;

                ViewData["CurrentFilter"] = queryVM.SearchText;

                if (!string.IsNullOrWhiteSpace(queryVM.SearchText) && queryVM.SearchText.Length < 3)
                    queryVM.SearchText = string.Empty;

                var postDTOs = await _postService.GetAllAsync(_mapper.Map<PostQueryDTO>(queryVM));
                var postVMs = new List<PostListVM>();

                //Filter posts by topic
                if (queryVM.Topic != null)
                    postDTOs = postDTOs.Where(p => p.Topic == queryVM.Topic);

                foreach (var postDTO in postDTOs)
                {
                    postVMs.Add(_mapper.Map<PostListVM>(postDTO));
                }

                if (!string.IsNullOrWhiteSpace(queryVM.SearchText))
                {
                    //Number of posts returned when searching
                    TempData["NumOfResults"] = postVMs.Count;
                }

                var pagedList = PaginatedList<PostListVM>.Create(postVMs.AsQueryable(), queryVM.PageNumber, queryVM.PageSize);

                return View(pagedList);
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }

            //return View();
        }

        [HttpGet]
        //[ActionName("Create")]
        public IActionResult Create() => View(new PostVM());

        //public IActionResult RedirectCreate([Bind("Title, Content, Topic, Visibility, Image")] PostVM postVM)
        //{
        //    if (User.Identity?.IsAuthenticated == false)
        //    {
        //        HttpContext.Session.SetString("PostVM", JsonConvert.SerializeObject(postVM));
        //    }

        //    return RedirectToAction(nameof(CreatePost), new { postVM });
        //}

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var successful = await _crudPostService.DeleteAsync(id);
                    if (!successful)
                    {
                        TempData["PopupMessage"] = "An error ocurred when deleting the post!";
                        TempData["PopupType"] = "error";
                        return RedirectToAction("View", routeValues: new { id });
                    }
                }
                else
                    return NotFound();

                TempData["PopupMessage"] = "Post deleted successfully!";
                TempData["PopupType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        //Get a single post
        [HttpGet]
        [ActionName("View")]
        public async Task<IActionResult> Get(int id, string commentText = null)
        {
            try
            {
                if (id > 0)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var postDTO = await _postService.GetByIdAsync(id, userId);

                    if (postDTO != null)
                    {
                        var postVM = _mapper.Map<PostShowVM>(postDTO);

                        //var imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                        //postVM.ImageUrl = $"{imgPath}\\{postVM.ImageUrl}";

                        if (commentText != null)
                        {
                            postVM.Comment.Content = commentText;
                            //HttpContext.Request.Query = null;
                        }

                        return View("View", postVM);
                    }
                    else return NotFound();
                }
                else return NotFound();
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        //View to edit post
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    if (id > 0)
                    {
                        var postDTO = await _postService.GetByIdAsync(id);
                        if (postDTO != null)
                        {
                            var postVM = _mapper.Map<PostEditVM>(postDTO);
                            return View(postVM);
                        }
                        else return NotFound();
                    }
                    else return NotFound();
                }
                catch (ArgumentException ex)
                {
                    return RedirectToAction("View", routeValues: new { id });
                    throw;
                }
            }
            else return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] PostEditVM postVM)
        {
            try
            {
                //Validate if an image was uploaded
                if (postVM.Image != null)
                {
                    var imageResult = FileValidationHelper.ValidateFile(postVM.Image, FileCategory.PostImage);
                    switch (imageResult)
                    {
                        case ImageResult.Success:
                            postVM.ImageFileName = FileValidationHelper.ValidateFileName(postVM.Image.FileName, FileStorageDirectory.Upload);
                            break;
                        case ImageResult.InvalidDimensions:
                            ModelState.AddModelError("Image", $"Image must be at least {ImageDimensions.POST_IMAGE_MIN_WIDTH}x{ImageDimensions.POST_IMAGE_MIN_HEIGHT} pixels.");
                            break;
                        case ImageResult.InvalidFormat:
                            ModelState.AddModelError("Image", "Uploaded file is not a valid image");
                            break;
                        default:
                            Guard.Against.EnumOutOfRange(imageResult, nameof(imageResult));
                            break;
                    }
                }

                if (!ModelState.IsValid)
                    return View(postVM);

                postVM.Content = FilterTextInputHelper.ValidateText(postVM.Content);
                postVM.Title = FilterTextInputHelper.ValidateText(postVM.Title);
                postVM.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var postDTO = _mapper.Map<PostEditDTO>(postVM);

                //If an image was uploaded, update the metadata in the DB and delete the current image
                if (postVM.Image != null)
                {
                    var imageDTO = _mapper.Map<AttachmentEditDTO>(postVM);
                    imageDTO.FileStorageType = FileStorageType.FileSystem;

                    var successful = await _crudPostService.EditAsync(postDTO, imageDTO, postVM.Image);
                    if (!successful)
                    {
                        TempData["PopupMessage"] = "An error ocurred when editing the post!";
                        TempData["PopupType"] = "error";
                        return RedirectToAction("View", routeValues: new { postVM.Id });
                    }
                }
                else
                    await _postService.EditAsync(postDTO);

                TempData["PopupMessage"] = "Post edited successfully!";
                TempData["PopupType"] = "success";
                return RedirectToAction("View", routeValues: new { postVM.Id });
            }
            catch (ArgumentException ex)
            {
                return View(nameof(Index));
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreatePostRedirect()
        {
            //return RedirectToAction("CreatePost", routeValues: postVM);

            //var postJson = HttpContext.Session.GetString("PostVM");
            var postJson = TempData["PostVM"].ToString();

            Guard.Against.NullOrWhiteSpace(postJson, nameof(postJson));

            var dict = ParseJsonHelper.Parse(postJson);

            var postVM = new PostVM
            {
                Title = dict["Title"],
                Content = dict["Content"],
                Topic = (TopicName)Enum.Parse(typeof(TopicName), dict["Topic"]),
                Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), dict["Visibility"]),
                ImageFileName = dict["FileName"],
                ImageFileSize = long.Parse(dict["FileSize"]),
                ImageFileType = dict["FileType"]
            };

            postVM.Redirected = true;
            //HttpContext.Session.Remove("PostVM");

            //return RedirectToAction(nameof(CreatePostRedirect1), routeValues: new { postVM } );

            await CreatePostRedirect1(postVM);

            TempData["PopupMessage"] = "Post created successfully!";
            TempData["PopupType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet(Name = "CreatePostRedirect1")]
        private async Task<IActionResult> CreatePostRedirect1(PostVM postVM)
        {
            try
            {
                if (!postVM.Redirected)
                {
                    var imageResult = FileValidationHelper.ValidateFile(postVM.Image, FileCategory.PostImage);
                    switch (imageResult)
                    {
                        case ImageResult.Success:
                            postVM.ImageFileName = FileValidationHelper.ValidateFileName(postVM.Image.FileName, FileStorageDirectory.Upload);
                            break;
                        case ImageResult.InvalidDimensions:
                            ModelState.AddModelError("Image", $"Image must be at least {ImageDimensions.POST_IMAGE_MIN_WIDTH}x{ImageDimensions.POST_IMAGE_MIN_HEIGHT} pixels.");
                            break;
                        case ImageResult.InvalidFormat:
                            ModelState.AddModelError("Image", "Uploaded file is not a valid image");
                            break;
                        default:
                            Guard.Against.EnumOutOfRange(imageResult, nameof(imageResult));
                            break;
                    }
                }

                if (!ModelState.IsValid)
                    return View(postVM);

                postVM.Content = FilterTextInputHelper.ValidateText(postVM.Content);
                postVM.Title = FilterTextInputHelper.ValidateText(postVM.Title);

                var postDTO = _mapper.Map<PostCreateDTO>(postVM);
                var imageDTO = _mapper.Map<AttachmentCreateDTO>(postVM);
                postDTO.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //Fill in the other properties
                imageDTO.FileStorageType = FileStorageType.FileSystem;
                imageDTO.CreatedBy = DataCreation.User;
                imageDTO.UpdatedBy = DataCreation.User;

                var successful = await _crudPostService.CreateAsync(postDTO, imageDTO, postVM.Image);
                if (successful)
                {
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction(nameof(CreatePostRedirect));
                }
                else
                {
                    TempData["PopupMessage"] = "An error ocurred when creating the post!";
                    TempData["PopupType"] = "error";
                    //TempData["Error"] = "There was an error when adding post data.";
                    return View(postVM);
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Image", "Uploaded file is not a valid image");
                _logger.LogWarning(ex, "Uploaded file is not a valid image");
                return View(postVM);
            }
        }

        [ValidateAntiForgeryToken]
        //[Authorize(Roles = nameof(UserRoles.User) + "," + nameof(UserRoles.Moderator) + "," + nameof(UserRoles.Admin))]
        //[Authorize]
        [HttpPost/*("CreatePost")*/]
        public async Task<IActionResult> Create([FromForm] PostVM postVM)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                try
                {
                    var imageResult = FileValidationHelper.ValidateFile(postVM.Image, FileCategory.PostImage);
                    switch (imageResult)
                    {
                        case ImageResult.Success:
                            postVM.ImageFileName = FileValidationHelper.ValidateFileName(postVM.Image.FileName, FileStorageDirectory.Upload);
                            break;
                        case ImageResult.InvalidDimensions:
                            ModelState.AddModelError("Image", $"Image must be at least {ImageDimensions.POST_IMAGE_MIN_WIDTH}x{ImageDimensions.POST_IMAGE_MIN_HEIGHT} pixels.");
                            break;
                        case ImageResult.InvalidFormat:
                            ModelState.AddModelError("Image", "Uploaded file is not a valid image");
                            break;
                        default:
                            Guard.Against.EnumOutOfRange(imageResult, nameof(imageResult));
                            break;
                    }

                    if (!ModelState.IsValid) return View(postVM);

                    //Save data inputted
                    var postVmData = new Dictionary<string, object>
                    {
                        ["Title"] = postVM.Title,
                        ["Content"] = postVM.Content,
                        ["Topic"] = (int)postVM.Topic,
                        ["Visibility"] = (int)postVM.Visibility,
                        //Image metadata
                        ["FileName"] = postVM.ImageFileName,
                        ["FileType"] = postVM.Image.ContentType,
                        ["FileSize"] = postVM.Image.Length
                    };

                    //Save image in the temp directory
                    await _fileStorage.SaveFileToDiskAsync(postVM.Image.OpenReadStream(), postVmData["FileName"].ToString(), FileStorageDirectory.Temp);

                    TempData["PostVM"] = JsonConvert.SerializeObject(postVmData);
                    //HttpContext.Session.SetString("PostVM", JsonConvert.SerializeObject(postVmData));
                    //HttpContext.Session.SetString("PostVM_ImagePath", filePath);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }

                return RedirectToAction("Login", "Account", routeValues: new { returnUrl = Url.Content("~/Post/CreatePostRedirect") });
                //return View("Login", new LoginVM { ReturnUrl = Url.Content("~/Post/Create") });

                //return RedirectToRoute(routeName: "userLogin", routeValues: new { returnUrl = Url.Content("~/Post/Create") });
            }

            else
                //return RedirectToAction(nameof(CreatePostRedirect1), routeValues: postVM);

                return RedirectToRoute(routeName: "CreatePostRedirect1", routeValues: postVM);

            //try
            //{
            //    if (!postVM.Redirected)
            //    {
            //        var validImage = FileValidationHelper.ValidateFile(postVM.Image);
            //        if (validImage)
            //        {
            //            postVM.ImageFileName = FileValidationHelper.ValidateFileName(postVM.Image.FileName, FileStorageDirectory.Upload);
            //        }
            //        else ModelState.AddModelError("Image", "Uploaded file is not a valid image");
            //    }

            //    if (!ModelState.IsValid)
            //    {
            //        return View(postVM);
            //    }

            //    var postDTO = _mapper.Map<PostCreateDTO>(postVM);
            //    var imageDTO = _mapper.Map<AttachmentCreateDTO>(postVM);

            //    //Fill in the other properties
            //    imageDTO.FileStorageType = FileStorageType.FileSystem;
            //    imageDTO.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //    imageDTO.CreatedBy = DataCreation.User;
            //    imageDTO.UpdatedBy = DataCreation.User;

            //    postDTO.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //    var successful = await _createPostService.ExecuteAsync(postDTO, imageDTO, postVM.Image);
            //    if (successful)
            //    {
            //        return RedirectToAction(nameof(Index));
            //    }
            //    else
            //    {
            //        TempData["Error"] = "There was an error when adding post data.";
            //        return View(postVM);
            //    }
            //}
            //catch (ArgumentException ex)
            //{
            //    ModelState.AddModelError("Image", "Uploaded file is not a valid image");
            //    _logger.LogWarning(ex, "Uploaded file is not a valid image");
            //    return View(postVM);
            //}
        }
    }
}
