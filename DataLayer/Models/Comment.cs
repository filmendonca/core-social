using DataLayer.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public bool IsEdited { get; set; } = false;

        #region Navigation Properties

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public virtual Post Post { get; set; }

        public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

        #endregion
    }
}
