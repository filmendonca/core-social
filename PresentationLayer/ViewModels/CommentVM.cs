using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class CommentVM
    {

    }
    public class CommentEditVM
    {
        public int Id { get; set; }
        [StringLength(500, MinimumLength = 20, ErrorMessage = "Content must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public bool IsEdited { get; set; } = true;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    }


    public class CommentCreateVM
    {
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Content must be between {2} and {1} chars")]
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public int PostId { get; set; }

        public string UserId { get; set; }
    }

    public class CommentGetVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public bool IsEdited { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public UserGetVM User { get; set; } = new();
        public string AvatarUrl { get; set; }
        public ReactionVM Reaction { get; set; } = new();

        public int Score { get; set; } //Likes - Dislikes
        public ReactionType? UserReaction { get; set; } //Reaction of logged in user
    }
}
