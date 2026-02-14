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
    public class Friendship : BaseEntity
    {
        public FriendshipStatus Status { get; set; }

        #region Navigation Properties
        
        [ForeignKey(nameof(Requester))]
        public string RequesterId { get; set; }
        public virtual User Requester { get; set; } //User that sends the request

        [ForeignKey(nameof(Recipient))]
        public string RecipientId { get; set; }
        public virtual User Recipient { get; set; } //User that receives the request

        #endregion
    }
}
