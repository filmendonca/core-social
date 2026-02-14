using DataLayer.Base;
using DataLayer.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Post : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Popularity { get; set; } = 0;
        public TopicName Topic { get; set; }
        public VisibilityType Visibility { get; set; } = VisibilityType.Public;
        public bool IsEdited { get; set; } = false;
        public DateTime? LastInteractionDate { get; set; } = null;

        #region Navigation Properties

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(Image))]
        public int? ImageId { get; set; } //Id of attachment
        public virtual Attachment Image { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        #endregion
    }
}
