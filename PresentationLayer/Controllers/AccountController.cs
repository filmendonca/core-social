using AutoMapper;
using BusinessLayer.DTOs.User;
using BusinessLayer.DomainServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text.Json;
using System.IO;
using DataLayer.Enums;
using Utils.Helpers;
using DataLayer.Interfaces;
using Utils.Enums;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        #region Dependency Injection

        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        private readonly IFileStorage _fileStorage;

        public AccountController(IAuthService authService, IMapper mapper, ILogger<AccountController> logger, IFileStorage fileStorage)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;

            _fileStorage = fileStorage;
        }

        #endregion

        public IActionResult Register() => (User.Identity?.IsAuthenticated == true) ? RedirectToAction("Index", "Home") : View(new RegisterVM());

        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            else
            {
                //ViewData["ReturnUrl"] = returnUrl;
                returnUrl ??= Url.Content("~/Home/Index"); //Go to the home page if there is no return Url
                return View(new LoginVM { ReturnUrl = returnUrl });
            }
        }

        //public IActionResult Login(string returnUrl = null)
        //{
        //    returnUrl = returnUrl ?? Url.Content("~/"); // Default to home page if no return URL
        //    return View(new LoginVM { ReturnUrl = returnUrl });
        //}


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) 
            {
                _logger.LogWarning($"Invalid ViewModel input in {nameof(Register)} method of {nameof(AccountController)} class.");
                return View(registerVM); 
            }

            var userDTO = _mapper.Map<UserCreateDTO>(registerVM);
            var userExists = await _authService.FindByEmailAsync(userDTO.Email);
            if (userExists)
            {
                TempData["Error"] = "This email address is already in use!";
                return View(registerVM);
            }

            var passwordErrors = await _authService.ValidatePasswordAsync(userDTO.Password);
            if (passwordErrors.Count > 0)
            {
                //If the password doesn't have an uppercase, lowercase or a special char
                TempData["Error"] = "Password has extra errors:<br />";

                foreach (var error in passwordErrors)
                {
                    TempData["Error"] += "- " + HtmlEncoder.Default.Encode(error) + "<br />";
                }

                return View(registerVM);
            }

            var userCreated = await _authService.CreateAsync(userDTO);
            if (userCreated)
            {
                _logger.LogInformation("New user created successfully.");
                return RedirectToAction(nameof(RegisterRedirect));
            }
            else
            {
                _logger.LogError($"An error ocurred in {nameof(Register)} method of {nameof(AccountController)} class.");
                throw new Exception("Something went wrong.");
            }
        }

        [HttpGet]
        public IActionResult RegisterRedirect() => View("RegisterSuccessful");

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) { return View(loginVM); }

            var userDTO = _mapper.Map<UserLoginDTO>(loginVM);
            var userExists = await _authService.FindByEmailAsync(userDTO.Email);
            if (userExists)
            {
                var passwordCheck = await _authService.CheckPasswordAsync(userDTO.Email, userDTO.Password);
                if (passwordCheck)
                {
                    var success = await _authService.PasswordSignInAsync(userDTO.Email, userDTO.Password);
                    if (success)
                    {
                        //Get controller and action names from the return Url
                        string[] splitUrl = loginVM.ReturnUrl?.Split("/", StringSplitOptions.RemoveEmptyEntries);
                        var controller = splitUrl[0];
                        var action = splitUrl[1];

                        //var returnUrl = string.Empty;

                        //ViewData["ReturnUrl"] = returnUrl;

                        //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        //{
                        //    return Redirect(returnUrl);
                        //}

                        //return RedirectToAction("Index", "Home");


                        //if (controller != "Home")
                        //{
                        //    //return RedirectToAction(action, controller, new PostVM());
                        //    //return RedirectToAction("CreatePostRedirect", "Post", new PostVM());

                        //    var postJson = HttpContext.Session.GetString("PostVM");
                        //    //var imageFilePath = HttpContext.Session.GetString("PostVM_ImagePath");

                        //    if (!string.IsNullOrWhiteSpace(postJson) /*&& !string.IsNullOrWhiteSpace(imageFilePath)*/)
                        //    {
                        //        var dict = ParseJsonHelper.Parse(postJson);

                        //        var postVM = new PostVM
                        //        {
                        //            Title = dict["Title"],
                        //            Content = dict["Content"],
                        //            Topic = (TopicName)Enum.Parse(typeof(TopicName), dict["Topic"]),
                        //            Visibility = (VisibilityType)Enum.Parse(typeof(VisibilityType), dict["Visibility"]),
                        //            ImageFileName = dict["FileName"],
                        //            ImageFileSize = long.Parse(dict["FileSize"]),
                        //            ImageFileType = dict["FileType"]
                        //        };

                        //        HttpContext.Session.Remove("PostVM");
                        //        //HttpContext.Session.Remove("PostVM_ImagePath");

                        //        postVM.Redirected = true;

                        //        return RedirectToAction(action, controller /*new { postVM }*/);
                        //    }
                        //    else
                        //    {
                        //        return RedirectToAction(action, controller);
                        //        throw new Exception("Something went wrong.");
                        //    }
                        //}

                        //else return RedirectToAction(action, controller);

                        TempData["PopupMessage"] = "Login successful!";
                        TempData["PopupType"] = "success";

                        return RedirectToAction(action, controller);
                    }
                }
                else 
                {
                    //Wrong password
                    TempData["Error"] = "Wrong credentials. Please, try again!";
                    return View(loginVM);
                }
            }
            else
            {
                //User does not exist
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(loginVM);
            }

            throw new Exception("Something went wrong.");
        }

        //[HttpGet]
        //public IActionResult LoginRedirect() => View("Home/Index");

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true) 
                await _authService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
