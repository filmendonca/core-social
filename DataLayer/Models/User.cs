using DataLayer.Enums;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class User : IdentityUser, IBaseEntity
    {
        public DateTime? BirthDate { get; set; } = null;
        public GenderOption Gender { get; set; }
        public string Bio { get; set; } = "This user hasn't updated their bio yet";

        //Reputation = (Likes - Dislikes) - (Active Warnings Received^3)
        //Alternative: Reputation = (Likes - Dislikes) - (Math.Min(WarningsReceived, 5(multiplier)) * 5(max nº of warnings before ban))
        public int Reputation { get; set; } = 0;

        #region Navigation Properties

        [ForeignKey(nameof(Avatar))]
        public int? AvatarId { get; set; }
        public virtual Attachment Avatar { get; set; }
        public virtual IdentityRole Role { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Warning> WarningsReceived { get; set; } = new List<Warning>(); //If User.Role = User
        public virtual ICollection<Warning> WarningsGiven { get; set; } = new List<Warning>(); //If User.Role = Moderator
        public virtual ICollection<Ban> BansReceived { get; set; } = new List<Ban>(); //If User.Role = User
        public virtual ICollection<Ban> BansGiven { get; set; } = new List<Ban>(); //If User.Role = Moderator
        public virtual ICollection<Friendship> FriendshipsOfRequester { get; set; } = new List<Friendship>(); //old user1
        public virtual ICollection<Friendship> FriendshipsOfRecipient { get; set; } = new List<Friendship>(); //old user2
        public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

        //check later
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        #endregion

        #region Data Auditing Properties

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; } = null;
        public DataCreation CreatedBy { get; set; }
        public DataCreation UpdatedBy { get; set; }
        int IBaseEntity.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion
    }
}
