using DataLayer.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PresentationLayer.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils.Models;

namespace PresentationLayer.ViewModels
{
    public class PostEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Topic is required")]
        public TopicName Topic { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [StringLength(500, MinimumLength = 20, ErrorMessage = "Content must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Visibility is required")]
        public VisibilityType Visibility { get; set; }

        public int Popularity { get; set; }
        public bool IsEdited { get; set; } = true;
        public DateTime? LastInteractionDate { get; set; }
        public int? ImageId { get; set; }
        public string UserId { get; set; }
        //public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;

        public string ImageUrl { get; set; }
        public string FilePath { get; set; }


        //[Required(ErrorMessage = "Image is required")]
        //[ImageFile(3 * 1024 * 1024, ErrorMessage = "File size must not exceed 3 MB")]
        [ImageFile(3 * 1024 * 1024)]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        [BindNever]
        public string ImageFileName { get; set; }
        [BindNever]
        public string ImageFileType { get; set; }
        [BindNever]
        public long ImageFileSize { get; set; }
    }

    //This View Model will get data from the query params
    public class PostQueryVM
    {
        [FromQuery(Name = "search")]
        public string SearchText { get; set; }
        public string CurrentFilter { get; set; }

        [FromQuery(Name = "page")]
        public int PageNumber { get; set; } = 1;

        [Range(2, 10, ErrorMessage = "Page size must be between {1} and {2}")]
        public int PageSize { get; set; } = 10;

        public string Sort { get; set; }

        public TopicName? Topic { get; set; } = null;
    }

    public class PostListVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Topic is required")]
        public TopicName Topic { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, MinimumLength = 20, ErrorMessage = "Content must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Visibility is required")]
        public VisibilityType Visibility { get; set; } = VisibilityType.Public;

        public string ImageUrl { get; set; }
        public string FilePath { get; set; }

        public int Popularity { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? LastInteractionDate { get; set; }
        public int? ImageId { get; set; }
        public string UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Username { get; set; }
        public int NumOfComments { get; set; }

        public TopicName Topics { get; set; }

        //public PostQueryVM PostQueryVM { get; set; }
    }

    //Class that shows post and image data in a view
    public class PostShowVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Topic is required")]
        public TopicName Topic { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, MinimumLength = 20, ErrorMessage = nameof(Content) + " must be between {2} and {1} chars")]
        [Required(ErrorMessage = nameof(Content) + " is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Visibility is required")]
        public VisibilityType Visibility { get; set; } = VisibilityType.Public;

        public string ImageUrl { get; set; }
        public string FilePath { get; set; }

        public int Popularity { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? LastInteractionDate { get; set; }
        public int? ImageId { get; set; }
        public string UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Username { get; set; }

        //public class CommentGetVM
        //{
        //    public string Content { get; set; }
        //    public bool IsEdited { get; set; }
        //}

        public CommentCreateVM Comment { get; set; } = new();

        public ReactionVM Reaction { get; set; } = new();

        public ICollection<CommentGetVM> Comments { get; set; } = new List<CommentGetVM>();
    }

    public class PostVM
    {
        [Required(ErrorMessage = "Topic is required")]
        public TopicName Topic { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, MinimumLength = 20, ErrorMessage = "Content must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Visibility is required")]
        public VisibilityType Visibility { get; set; } = VisibilityType.Public;

        [Required(ErrorMessage = "Image is required")]
        //[ImageFile(3 * 1024 * 1024, ErrorMessage = "File size must not exceed 3 MB")]
        [ImageFile(3 * 1024 * 1024)]
        [DataType(DataType.Upload)] 
        public IFormFile Image { get; set; }

        [BindNever]
        public string ImageFileName { get; set; }
        [BindNever]
        public string ImageFileType { get; set; }
        [BindNever]
        public long ImageFileSize { get; set; }
        [BindNever]
        public bool Redirected { get; set; } = false;


        //public PostImageVM? ImageMetadata { get; set; }

        //private class PostImageVM
        //{
        //    public string FileName { get; set; } = string.Empty;
        //    public string FileType { get; set; } = string.Empty; //Content-Type
        //    public int FileSize { get; set; }
        //}

        //[Required(ErrorMessage = "Popularity is required")]
        //public int Popularity { get; set; } = 0;
    }
}
