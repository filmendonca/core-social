using AutoMapper;
using BusinessLayer.DomainServices.Interfaces;
using BusinessLayer.DTOs.Attachment;
using BusinessLayer.DTOs.Post;
using Utils.Enums;
using DataLayer.Interfaces;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.ApplicationServices.Post
{
    //This class manages the workflow and transaction related to the creation of a post
    public class CrudPostService
    {
        #region Dependency Injection

        private readonly IPostService _postService;
        private readonly IAttachmentService _attachmentService;
        private readonly ICommentService _commentService;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CrudPostService> _logger;

        public CrudPostService(
            IPostService postService,
            IAttachmentService attachmentService,
            ICommentService commentService,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CrudPostService> logger
            )
        {
            _postService = postService;
            _attachmentService = attachmentService;
            _commentService = commentService;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        public async Task<bool> CreateAsync(PostCreateDTO postDTO, AttachmentCreateDTO imageDTO, IFormFile? image)
        {
            var imageEntity = _mapper.Map<Attachment>(imageDTO);
            var postEntity = _mapper.Map<DataLayer.Models.Post>(postDTO);

            var flag = false;

            try
            {
                imageEntity.FilePath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);

                //Add changes to the DB Context
                await _attachmentService.AddAsync(imageEntity);
                await _postService.AddAsync(postEntity);

                if (!postDTO.Redirected)
                {
                    //Save new file in the file system
                    flag = await _fileStorage.SaveFileToDiskAsync(image.OpenReadStream(), imageDTO.FileName, FileStorageDirectory.Upload);
                }
                else flag = _fileStorage.MoveFile(imageDTO.FileName, FileStorageDirectory.Upload);

                //Save all the changes in a single transaction
                await _unitOfWork.SaveAsync();

                //Save the auto-generated id of the image in the post
                postEntity.ImageId = imageEntity.Id;
                await _unitOfWork.SaveAsync();
            }
            //If there are errors:
            catch(Exception ex)
            {
                //Quick check to see if the image was added, in order to bypass a redundant method call
                if (flag) _fileStorage.DeleteFile(imageDTO.FileName);
                _logger.LogWarning(ex, "An error ocurred while adding a new post to the DB");
                return false;
                throw;
            }

            return true;
        }

        public async Task<bool> EditAsync(PostEditDTO postDTO, AttachmentEditDTO imageDTO, IFormFile? image)
        {
            //Check if image was saved successfully
            var flag = false;

            //Get file name of old image
            var oldFileName = string.Empty;

            try
            {
                oldFileName = await _attachmentService.GetFileNameAsync(imageDTO.Id);
                imageDTO.FilePath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);

                //Add changes to the DB Context
                await _attachmentService.EditAsync(imageDTO);
                await _postService.EditAsync(postDTO, false);

                //Save new file in the file system
                flag = await _fileStorage.SaveFileToDiskAsync(image.OpenReadStream(), imageDTO.FileName, FileStorageDirectory.Upload);

                if (flag)
                {
                    //Move old file to temp folder, then wait for it to be deleted automatically
                    _fileStorage.MoveFile(oldFileName, FileStorageDirectory.Temp);
                }

                //Save all the changes in a single transaction
                await _unitOfWork.SaveAsync();
            }
            //If there are errors:
            catch (Exception ex)
            {
                //Quick check to see if the image was added, in order to bypass a redundant method call
                if (flag)
                {
                    _fileStorage.DeleteFile(imageDTO.FileName);
                    _fileStorage.MoveFile(oldFileName, FileStorageDirectory.Upload);
                }
                _logger.LogWarning(ex, "An error ocurred while adding a new post to the DB");
                return false;
                throw;
            }

            return true;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var flag = false;

            //Get file name of old image
            var oldFileName = string.Empty;

            try
            {
                var postDTO = await _postService.GetByIdAsync(id);
                oldFileName = await _attachmentService.GetFileNameAsync(postDTO.ImageId);

                //Soft delete the post and everything else related to it
                await _postService.DeleteAsync(id);
                await _attachmentService.DeleteAsync(postDTO.ImageId);
                await _commentService.DeleteRangeAsync(id);

                //Delete image
                flag = _fileStorage.MoveFile(oldFileName, FileStorageDirectory.Temp);

                await _unitOfWork.SaveAsync();
            }
            catch (Exception)
            {
                if (flag)
                    _fileStorage.MoveFile(oldFileName, FileStorageDirectory.Upload);
                return false;
                throw;
            }

            return true;
        }
    }
}
