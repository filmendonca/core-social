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
    public class Warning : BaseEntity
    {
        public ModerationReason Reason { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset ActiveUntil { get; set; } = DateTimeOffset.MinValue;

        #region Navigation Properties
        
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(Moderator))]
        public string ModeratorId { get; set; }
        public virtual User Moderator { get; set; }

        #endregion
    }
}
