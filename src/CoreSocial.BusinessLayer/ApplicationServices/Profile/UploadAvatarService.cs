using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.DomainServices.Interfaces;
using BusinessLayer.DTOs.Attachment;
using DataLayer.Interfaces;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Enums;

namespace BusinessLayer.ApplicationServices.Profile
{
    public class UploadAvatarService
    {
        #region Dependency Injection

        private readonly IAttachmentService _attachmentService;
        private readonly UserManager<User> _userManager;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UploadAvatarService> _logger;

        public UploadAvatarService(
            IAttachmentService attachmentService,
            UserManager<User> userManager,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UploadAvatarService> logger
            )
        {
            _attachmentService = attachmentService;
            _userManager = userManager;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        public async Task<bool> AddAvatarAsync(AttachmentCreateDTO dto, IFormFile avatar)
        {
            //Guard.Against.Null(dto, nameof(dto));
            //Guard.Against.Null(avatar, nameof(avatar));

            var flag = false;

            try
            {
                dto.FilePath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                var avatarEntity = _mapper.Map<Attachment>(dto);
                await _attachmentService.AddAsync(avatarEntity);

                flag = await _fileStorage.SaveFileToDiskAsync(avatar.OpenReadStream(), dto.FileName, FileStorageDirectory.Upload);

                //Save changes
                await _unitOfWork.SaveAsync();

                var userEntity = await _userManager.FindByIdAsync(dto.UserId);

                //Save the auto-generated id of the avatar in the user
                userEntity.AvatarId = avatarEntity.Id;
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                //Quick check to see if the avatar was added
                if (flag) _fileStorage.DeleteFile(dto.FileName);
                _logger.LogWarning(ex, "An error ocurred while adding a new post to the DB");
                return false;
                throw;
            }

            return true;
        }
    }
}
