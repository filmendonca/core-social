using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.DomainServices.Interfaces;
using BusinessLayer.DTOs.Profile;
using DataLayer.Interfaces;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Enums;
using System.Security.Claims;
using BusinessLayer.DTOs.Attachment;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DomainServices
{
    public class ProfileService : IProfileService
    {
        #region Dependency Injection

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfileService(
            IUserRepository userRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IFileStorage fileStorage,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        public async Task<ProfileGetDTO> GetUserByIdAsync(string id)
        {
            Guard.Against.NullOrWhiteSpace(id, nameof(id));

            //var navEntities = new List<string> { "Avatar", "Comments" };
            //var entity = await _userRepository.GetAsync(user => user.Id == id, navEntities);

            var user = await _userManager.FindByIdAsync(id);

            //Get role of user
            var roleNames = await _userManager.GetRolesAsync(user);
            user.Role = await _roleManager.FindByNameAsync(roleNames.FirstOrDefault());

            Guard.Against.Null(user, nameof(user));
            var postDTO = _mapper.Map<ProfileGetDTO>(user);

            var imgPath = string.Empty;
            switch (postDTO.FilePath)
            {
                case "img\\uploads":
                    imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                    break;
                //Set default image
                case null:
                    imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Default);
                    postDTO.AvatarUrl = "profile_pic.jpg";
                    break;
                default:
                    throw new Exception("Something went wrong");
            }

            postDTO.AvatarUrl = $"{imgPath}\\{postDTO.AvatarUrl}";

            return postDTO;
        }

        public async Task<ProfileGetDTO> GetUserByNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
                return _mapper.Map<ProfileGetDTO>(user);
            else return null;
        }

        public async Task<bool> EditAsync(ProfileEditDTO dto)
        {
            try
            {
                Guard.Against.Null(dto, nameof(dto));

                //In order to not overwrite the existing fields, the user needs to be loaded first
                var user = await _userManager.FindByIdAsync(dto.UserId) ?? throw new InvalidOperationException("User not found.");

                var oldEmail = user.Email;
                var oldUserName = user.UserName;
                var oldPhoneNumber = user.PhoneNumber;

                int numResults = 0;

                _mapper.Map(dto, user);

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    var pwdResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.Password);
                    if (!pwdResult.Succeeded)
                        throw new Exception("Error when updating password. Result is: " + pwdResult);
                    numResults++;
                }
                if (oldEmail != dto.Email)
                {
                    var emailResult = await _userManager.SetEmailAsync(user, dto.Email);
                    if (!emailResult.Succeeded)
                        throw new Exception("Error when updating email. Result is: " + emailResult);
                    numResults++;
                }
                if (oldUserName != dto.UserName)
                {
                    var userNameResult = await _userManager.SetUserNameAsync(user, dto.UserName);
                    if (!userNameResult.Succeeded)
                        throw new Exception("Error when updating username. Result is: " + userNameResult);
                    numResults++;
                }
                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && oldPhoneNumber != dto.PhoneNumber)
                {
                    var phoneNumResult = await _userManager.SetPhoneNumberAsync(user, dto.PhoneNumber);
                    if (!phoneNumResult.Succeeded)
                        throw new Exception("Error when updating phone number. Result is: " + phoneNumResult);
                    numResults++;
                }

                //Save custom properties, if necessary
                if (numResults == 0)
                    await _userManager.UpdateAsync(user);
            }
            catch(Exception ex)
            {
                throw;
                return false;
            }

            return true;
        }
    }
}
