using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class ReactionVM
    {
        public string UserId { get; set; }
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}
