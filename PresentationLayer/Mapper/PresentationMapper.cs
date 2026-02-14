using AutoMapper;
using BusinessLayer.DTOs.Attachment;
using BusinessLayer.DTOs.Comment;
using BusinessLayer.DTOs.Friendship;
using BusinessLayer.DTOs.Post;
using BusinessLayer.DTOs.Profile;
using BusinessLayer.DTOs.User;
using PresentationLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.Mapper
{
    public class PresentationMapper : Profile
    {
        public PresentationMapper()
        {
            #region User

            CreateMap<RegisterVM, UserCreateDTO>();
            CreateMap<LoginVM, UserLoginDTO>();

            CreateMap<UserGetVM, UserGetDTO>().ReverseMap();

            CreateMap<ProfileGetVM, ProfileGetDTO>().ReverseMap();

            CreateMap<ProfileEditVM, ProfileGetDTO>().ReverseMap();

            CreateMap<ProfileEditVM, ProfileEditDTO>().ReverseMap();

            #endregion


            #region Post

            CreateMap<PostVM, PostCreateDTO>();

            CreateMap<PostShowVM, PostGetDTO>();
            CreateMap<PostGetDTO, PostShowVM>()
                //.ForMember(vm => vm.ImageUrl, opt => opt.MapFrom(src => src.Image.FileName))
                //.ForMember(vm => vm.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(vm => vm.Comments, opt => opt.MapFrom(src => src.Comments));

            CreateMap<PostListDTO, PostListVM>()
                //.ForMember(vm => vm.ImageUrl, opt => opt.MapFrom(src => src.Image.FileName))
                .ForMember(vm => vm.FilePath, opt => opt.MapFrom(src => src.Image.FilePath));

            CreateMap<PostEditVM, PostGetDTO>();
            CreateMap<PostGetDTO, PostEditVM>()
                .ForMember(vm => vm.Image, opt => opt.Ignore());

            CreateMap<PostEditVM, PostEditDTO>()
                .ForMember(dto => dto.Image, opt => opt.Ignore());

            CreateMap<PostVM, AttachmentCreateDTO>()
                .ForMember(dto => dto.FileName, opt => opt.MapFrom(src => src.ImageFileName))
                .ForMember(dto => dto.FileType, opt => opt.MapFrom(src => src.ImageFileType))
                .ForMember(dto => dto.FileSize, opt => opt.MapFrom(src => src.ImageFileSize))
                .ForMember(dto => dto.FileData, opt => opt.Ignore());

            CreateMap<PostEditVM, AttachmentEditDTO>()
                .ForMember(dto => dto.FileName, opt => opt.MapFrom(src => src.ImageFileName))
                .ForMember(dto => dto.FileType, opt => opt.MapFrom(src => src.Image.ContentType))
                .ForMember(dto => dto.FileSize, opt => opt.MapFrom(src => src.Image.Length))
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.ImageId))
                .ForMember(dto => dto.FileData, opt => opt.Ignore());

            CreateMap<PostQueryVM, PostQueryDTO>().ReverseMap();

            #endregion


            #region Comment

            CreateMap<CommentCreateVM, CommentCreateDTO>().ReverseMap();

            CreateMap<CommentGetVM, CommentGetDTO>();

            CreateMap<CommentGetDTO, CommentGetVM>();

            CreateMap<CommentEditVM, CommentEditDTO>().ReverseMap();

            #endregion

            #region Friendship

            CreateMap<FriendshipStatusVM, FriendshipStatusDTO>().ReverseMap();

            #endregion

        }
    }
}
