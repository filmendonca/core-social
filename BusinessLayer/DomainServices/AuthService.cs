using AutoMapper;
using BusinessLayer.DTOs.User;
using BusinessLayer.DomainServices.Interfaces;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Constants;

namespace BusinessLayer.DomainServices
{
    //This service manages the sign-in workflow of an user
    public class AuthService : IAuthService
    {
        #region Dependency Injection

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IPasswordValidator<User> _passwordValidator;

        public AuthService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            IPasswordValidator<User> passwordValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _passwordValidator = passwordValidator;
        }

        #endregion

        public async Task<bool> CreateAsync(UserCreateDTO dto)
        {
            var newUser = _mapper.Map<User>(dto);
            var creationResult = await _userManager.CreateAsync(newUser, dto.Password);
            if (creationResult.Succeeded)
            {
                //Give default role to user
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                //Create claims later!
            }
            else return false;

            return true;
        }

        public async Task<ICollection<string>> ValidatePasswordAsync(string password)
        {
            var result = await _passwordValidator.ValidateAsync(_userManager, new(), password);

            var errorList = new List<string>();
            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    errorList.Add(error.Description);
                }
            }

            return errorList;
        }

        public async Task<bool> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> PasswordSignInAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            return result.Succeeded;
        }

        public async Task SignOutAsync() => await _signInManager.SignOutAsync();
    }
}
