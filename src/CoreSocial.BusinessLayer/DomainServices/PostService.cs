using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.DTOs.Post;
using BusinessLayer.DomainServices.Interfaces;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Helpers;
using BusinessLayer.DTOs.Attachment;
using DataLayer.Enums;
using System.Linq;
using System.Linq.Expressions;
using Utils.Enums;
using Utils.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.DTOs.Comment;

namespace BusinessLayer.DomainServices
{
    public class PostService : IPostService
    {
        //Navigation entities for inclusion when getting repository data
        private readonly string[] navEntities = { "Image", "Comments" };

        #region Dependency Injection

        //Either "private readonly IGenericRepository<Post> _postRepository;" //Or "private readonly IPostRepository _postRepository;"

        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;

        public PostService(IPostRepository postRepository, IMapper mapper, IUnitOfWork unitOfWork, IFileStorage fileStorage)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
        }

        #endregion

        //public async Task<int> EditAsync(PostEditDTO dto, int? attachmentId)
        //{
        //    Guard.Against.Null(dto, nameof(dto));
        //    //Guard.Against.Null(attachmentId, nameof(attachmentId));
        //    await _postRepository.UpdateAsync(new Post(), );
        //}

        public async Task AddAsync(Post post)
        {
            Guard.Against.Null(post, nameof(post));
            await _postRepository.AddAsync(post);
        }

        public async Task AddAsync(PostCreateDTO dto)
        {
            Guard.Against.Null(dto, nameof(dto));
            var post = _mapper.Map<Post>(dto);
            await _postRepository.AddAsync(post);
        }

        public async Task<PostGetDTO> GetByIdAsync(int id, string userId = null)
        {
            var postDTO = new PostGetDTO();

            if (userId != null)
            {
                //Get everything from post, including num of reactions of comments and the reaction of the current user
                var postQuery = await _postRepository.QueryAsync(id);
                postDTO = await postQuery
                    .ProjectTo<PostGetDTO>(
                        _mapper.ConfigurationProvider,
                        new { currentUserId = userId })
                    .FirstOrDefaultAsync();

                //postDTO = _postRepository.Query(id)
                //    .Select(p => new PostGetDTO
                //    {
                //        Id = p.Id,
                //        Title = p.Title,
                //        Comments = p.Comments.Select(c => new CommentGetDTO
                //        {
                //            Id = c.Id,
                //            Content = c.Content,
                //            Score = c.Reactions.Sum(r => (int)(r.Type ?? 0)),
                //            UserReaction = c.Reactions
                //                .Where(r => r.UserId == userId)
                //                .Select(r => r.Type)
                //                .FirstOrDefault()
                //        }).ToList()
                //    })
                //    .FirstOrDefault();
            }
            else
            {
                var post = await _postRepository.GetAsync(p => p.Id == id, navEntities.ToList());
                postDTO = _mapper.Map<PostGetDTO>(post);
            }               

            var imgPath = string.Empty;

            switch (postDTO.Image.FilePath)
            {
                case "img\\uploads":
                    imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                    break;
                case null:
                    imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Default);
                    postDTO.ImageUrl = "post_pic.jpg";
                    break;
                default:
                    throw new Exception("Something went wrong");
            }
            postDTO.ImageUrl = $"{imgPath}\\{postDTO.ImageUrl}";

            foreach (var commentDTO in postDTO.Comments)
            {
                if (commentDTO.User.Avatar != null)
                {
                    switch (commentDTO.User.Avatar.FilePath)
                    {
                        case "img\\uploads":
                            imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                            break;
                        //case null:
                        //    imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Default);
                        //    comment.AvatarUrl = "profile_pic.jpg";
                        //    break;
                        default:
                            throw new Exception("Something went wrong");
                    }
                }
                else
                {
                    imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Default);
                    commentDTO.AvatarUrl = "profile_pic.jpg";
                }

                commentDTO.AvatarUrl = $"{imgPath}\\{commentDTO.AvatarUrl }";
            }

            return postDTO;
        }

        public async Task<IEnumerable<PostListDTO>> GetAllAsync(PostQueryDTO queryDTO)
        {
            #region Expression Setup

            //Set default expression (no extra filters)
            Expression<Func<Post, bool>> defaultExp =
                p => p.CreatedBy == DataCreation.User;

            Expression<Func<Post, bool>> combinedExp = null;

            if (!string.IsNullOrWhiteSpace(queryDTO.SearchText))
            {
                //Set text search expression
                Expression<Func<Post, bool>> searchExp =
                    p => p.Title.Contains(queryDTO.SearchText) || p.Content.Contains(queryDTO.SearchText);

                //Get "p" (Post)
                var param = defaultExp.Parameters[0];

                var combinedBody = Expression.AndAlso(
                    defaultExp.Body,
                    Expression.Invoke(searchExp, param)
                );

                //Combined expression
                combinedExp = Expression.Lambda<Func<Post, bool>>(combinedBody, param);
            }

            #endregion

            //Get posts that were not soft deleted and created by users
            //By default, just get posts ordered by last creation
            var posts = await _postRepository.GetAllAsync(
                expression: (string.IsNullOrWhiteSpace(queryDTO.SearchText)) ? defaultExp : combinedExp,
                //orderBy: p => p.OrderByDescending(e => e.DateCreated),

                orderBy: (queryDTO.Sort == "name_desc") ? p => p.OrderByDescending(e => e.Title) :
                (queryDTO.Sort == "name_asc") ? p => p.OrderBy(e => e.Title) :
                (queryDTO.Sort == "date_asc") ? p => p.OrderBy(e => e.DateUpdated) :
                (queryDTO.Sort == "date_desc") ? p => p.OrderByDescending(e => e.DateUpdated) :
                (queryDTO.Sort == "pop_desc") ? p => p.OrderByDescending(e => e.Popularity) :
                (queryDTO.Sort == "pop_asc") ? p => p.OrderBy(e => e.Popularity) : null,

                //orderBy: OrderPostBy(queryDTO.Sort),

                includes: navEntities.ToList()
                );

            Guard.Against.Null(posts, nameof(posts));

            var postDTOs = new List<PostListDTO>();
            foreach (var post in posts)
                postDTOs.Add(_mapper.Map<PostListDTO>(post));

            var imgPath = string.Empty;

            foreach (var postDTO in postDTOs)
            {
                switch (postDTO.Image.FilePath)
                {
                    case "img\\uploads":
                        imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                        break;
                    case null:
                        imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Default);
                        postDTO.ImageUrl = "post_pic.jpg";
                        break;
                    default:
                        throw new Exception("Something went wrong");
                }

                postDTO.ImageUrl = $"{imgPath}\\{postDTO.ImageUrl}";
            }

            return postDTOs;
        }

        public async Task EditAsync(PostEditDTO dto, bool saveChanges = true)
        {
            Guard.Against.Null(dto, nameof(dto));
            var post = await _postRepository.GetAsync(p => p.Id == dto.Id);
            _mapper.Map(dto, post);
            await _postRepository.UpdateAsync(post);
            if (saveChanges)
                await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Guard.Against.NegativeOrZero(id, nameof(id));
            await _postRepository.SoftDeleteAsync(id);
            //await _unitOfWork.SaveAsync();
        }

        public async Task<bool> ChangeVisibility(int id, VisibilityType visibility)
        {
            Guard.Against.NegativeOrZero(id, nameof(id));
            Guard.Against.EnumOutOfRange(visibility, nameof(visibility));
            var post = await _postRepository.GetAsync(p => p.Id == id);
            if (post.Visibility != visibility)
            {
                post.Visibility = visibility;
                await _postRepository.UpdateAsync(post);
                await _unitOfWork.SaveAsync();
            }
            return true;
        }

        //private Func<IQueryable<Post>, IOrderedQueryable<Post>> OrderPostBy(string? sort)
        //{
        //    IQueryable<Post> posts1 = null;
        //    switch (sort)
        //    {
        //        case "name_desc":
        //            return posts => posts1.OrderByDescending(e => e.Title);
        //        case "name":
        //            return posts => posts1.OrderBy(e => e.Title);
        //        case "date":
        //            return posts => posts1.OrderBy(e => e.DateUpdated);
        //        case "date_desc":
        //            return posts => posts1.OrderByDescending(e => e.DateUpdated);
        //        case "pop_desc":
        //            return posts => posts1.OrderByDescending(e => e.Popularity);
        //        case "pop_asc":
        //            return posts => posts1.OrderBy(e => e.Popularity);
        //        default:
        //            return null;
        //    }
        //}
    }
}
