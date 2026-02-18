using BusinessLayer.DTOs.Reaction;
using BusinessLayer.DTOs.User;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs.Comment
{
    public class CommentDTO
    {

    }

    public class CommentEditDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public bool IsEdited { get; set; }
        public DateTime DateUpdated { get; set; }

        public DataCreation CreatedBy { get; set; } = DataCreation.User;
        public DataCreation UpdatedBy { get; set; } = DataCreation.User;
    }

    public class CommentCreateDTO
    {
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public DataCreation CreatedBy { get; set; } = DataCreation.User;
        public DataCreation UpdatedBy { get; set; } = DataCreation.User;
    }

    public class CommentGetDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public bool IsEdited { get; set; }
        public UserGetDTO User { get; set; } = new();
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public string AvatarUrl { get; set; }

        public int Score { get; set; } //Likes - Dislikes

        //public ICollection<ReactionGetDTO> Reactions { get; set; } = new List<ReactionGetDTO>();

        public ReactionType? UserReaction { get; set; } //Reaction of logged in user
    }
}
