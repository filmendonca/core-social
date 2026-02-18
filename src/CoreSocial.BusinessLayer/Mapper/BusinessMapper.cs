using AutoMapper;
using BusinessLayer.DTOs.Attachment;
using BusinessLayer.DTOs.Comment;
using BusinessLayer.DTOs.Friendship;
using BusinessLayer.DTOs.Post;
using BusinessLayer.DTOs.Profile;
using BusinessLayer.DTOs.Reaction;
using BusinessLayer.DTOs.User;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Mapper
{
    public class BusinessMapper : Profile
    {
        public BusinessMapper()
        {
            #region User

            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<UserCreateDTO, User>().ReverseMap();

            CreateMap<User, UserEditDTO>().ReverseMap();

            CreateMap<User, UserGetDTO>().ReverseMap();

            CreateMap<User, ProfileGetDTO>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar.FileName))
                .ForMember(dest => dest.NumPosts, opt => opt.MapFrom(src => src.Posts.Count))
                .ForMember(dest => dest.NumComments, opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<User, ProfileEditDTO>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Avatar.FileName))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.Avatar.FilePath))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ProfileEditDTO, User>()
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore());

            #endregion


            #region Post

            CreateMap<Post, PostDTO>().ReverseMap();

            CreateMap<Post, PostCreateDTO>().ReverseMap();
            //.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            //CreateMap<Post, PostGetDTO>().ReverseMap();

            CreateMap<Post, PostGetDTO>()
                .ForMember(
                    dest => dest.Comments,
                    opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image.FileName))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<Post, PostListDTO>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image.FileName))
                .ForMember(dest => dest.NumOfComments, opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<PostEditDTO, Post>()
                //Ignore null fields
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //CreateMap<PostEditDTO, PostCreateDTO>().ReverseMap();

            #endregion


            #region Attachment

            CreateMap<Attachment, AttachmentCreateDTO>().ReverseMap();

            CreateMap<Attachment, AttachmentEditDTO>().ReverseMap();

            CreateMap<Attachment, AttachmentGetDTO>().ReverseMap();

            #endregion


            #region Comment

            CreateMap<Comment, CommentGetDTO>()
                //Get the sum of all reactions of a comment
                .ForMember(dest => dest.Score, opt =>
                    opt.MapFrom(src => src.Reactions.Sum(r => (int)(r.Type ?? 0))))
                .ForMember(
                    d => d.UserReaction,
                    o => o.Ignore())
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.Avatar.FileName));

            CreateMap<Comment, CommentCreateDTO>().ReverseMap();

            CreateMap<Comment, CommentEditDTO>().ReverseMap();

            #endregion


            CreateMap<Reaction, ReactionGetDTO>().ReverseMap();


            #region Friendship

            CreateMap<Friendship, FriendshipStateDTO>()
                .ForMember(dest => dest.FriendshipId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.Requester.UserName))
                .ReverseMap();

            CreateMap<Friendship, FriendDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(
                    (src, _, _, ctx) =>
                        src.RequesterId == (string)ctx.Items["UserId"]
                            ? src.Recipient.Id
                            : src.Requester.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(
                    (src, _, _, ctx) =>
                        src.RequesterId == (string)ctx.Items["UserId"]
                            ? src.Recipient.UserName
                            : src.Requester.UserName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(
                    (src, _, _, ctx) =>
                        src.RequesterId == (string)ctx.Items["UserId"]
                            ? src.Recipient.Bio
                            : src.Requester.Bio))
                .ForMember(dest => dest.Reputation, opt => opt.MapFrom(
                    (src, _, _, ctx) =>
                        src.RequesterId == (string)ctx.Items["UserId"]
                            ? src.Recipient.Reputation
                            : src.Requester.Reputation))
                .ForMember(dest => dest.AvatarId, opt => opt.MapFrom(
                    (src, _, _, ctx) =>
                        src.RequesterId == (string)ctx.Items["UserId"]
                            ? src.Recipient.AvatarId
                            : src.Requester.AvatarId));

            #endregion

            //CreateMap<PostVM, PostDTO>()
            //.ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.ImageFileName))
            //.ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.Image.ContentType))
            //.ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.Image.Length))
            //.ForMember(dest => dest.FileData, opt => opt.Ignore()); // we’ll fill manually
        }
    }
}
