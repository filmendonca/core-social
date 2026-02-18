using BusinessLayer.DTOs.Attachment;
using BusinessLayer.DTOs.Comment;
using BusinessLayer.DTOs.User;
using DataLayer.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Utils.Models;

namespace BusinessLayer.DTOs.Post
{
    public class PostDTO
    {

    }

    public class PostQueryDTO
    {
        public string SearchText { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
    }

    public class PostEditDTO
    {
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public string UserId { get; set; }
        public TopicName Topic { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public VisibilityType Visibility { get; set; }
        public int Popularity { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? LastInteractionDate { get; set; }
        public AttachmentGetDTO Image { get; set; }
        public DateTime DateUpdated { get; set; }
        //public DateTime DateCreated { get; set; }
    }

    //List of DTOs
    public class PostListDTO
    {
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public string UserId { get; set; }
        public TopicName Topic { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public VisibilityType Visibility { get; set; }
        public int Popularity { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? LastInteractionDate { get; set; }
        public AttachmentGetDTO Image { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumOfComments { get; set; }
        public string Username { get; set; }

        public string ImageUrl { get; set; }

        public static int TotalPages { get; set; }
    }

    //Single DTO
    public class PostGetDTO
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public string UserId { get; set; }
        public TopicName Topic { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public VisibilityType Visibility { get; set; }
        public int Popularity { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? LastInteractionDate { get; set; }
        public AttachmentGetDTO Image { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Username { get; set; }

        public string ImageUrl { get; set; }
        public string FilePath { get; set; }

        public ICollection<CommentGetDTO> Comments { get; set; } = new List<CommentGetDTO>();

        //public bool Redirected { get; set; }
        //public DataCreation CreatedBy { get; set; } = DataCreation.User;
        //public DataCreation UpdatedBy { get; set; } = DataCreation.User;
    }

    public class PostCreateDTO
    {
        public string UserId { get; set; }
        public TopicName Topic { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public VisibilityType Visibility { get; set; }
        public bool Redirected { get; set; }
        public DataCreation CreatedBy { get; set; } = DataCreation.User;
        public DataCreation UpdatedBy { get; set; } = DataCreation.User;

        //public int? ImageId { get; set; }
        //public virtual AttachmentCreateDTO Image { get; set; }

        //// For the attachment
        //public string FileName { get; set; }
        //public string ContentType { get; set; }
        //public long FileSize { get; set; }
        //public byte[] FileData { get; set; }  // or file path if you're using FS
    }

}
