using DataLayer.Base;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Reaction : BaseEntity
    {
        public ReactionType? Type { get; set; } = null;

        #region Navigation Properties

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(Comment))]
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }

        #endregion
    }
}
