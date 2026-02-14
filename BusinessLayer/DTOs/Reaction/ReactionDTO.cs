using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs.Reaction
{
    public class ReactionDTO
    {

    }

    public class ReactionGetDTO
    {
        public string UserId { get; set; }
        public int CommentId { get; set; }
        public ReactionType? Type { get; set; }



        //[ForeignKey(nameof(User))]
        //public string UserId { get; set; }
        //public virtual User User { get; set; }

        //[ForeignKey(nameof(Comment))]
        //public int CommentId { get; set; }
        //public virtual Comment Comment { get; set; }
    }
}
