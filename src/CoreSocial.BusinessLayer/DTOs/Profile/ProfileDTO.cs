using BusinessLayer.DTOs.Attachment;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs.Profile
{
    public class ProfileDTO
    {

    }

    public class ProfileEditDTO
    {
        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public GenderOption Gender { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; } //New password
        //public string ConfirmPassword { get; set; }
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Bio { get; set; }
        public int Reputation { get; set; }
        public DateTime DateCreated { get; set; }

        //public int AvatarId { get; set; }
        public string AvatarUrl { get; set; }
        public string FilePath { get; set; }
    }

    public class ProfileGetDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public GenderOption Gender { get; set; }
        public string Bio { get; set; }
        public int Reputation { get; set; }
        public DateTime DateCreated { get; set; }

        public string Role { get; set; }

        public int NumPosts { get; set; }
        public int NumComments { get; set; }

        public string FilePath { get; set; }
        public string AvatarUrl { get; set; }
        //public AttachmentGetDTO Avatar { get; set; }

        //public virtual Attachment Avatar { get; set; }
        //public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        //public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        //public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        //public virtual ICollection<Friendship> FriendshipsOfRequester { get; set; } = new List<Friendship>(); //old user1
        //public virtual ICollection<Friendship> FriendshipsOfRecipient { get; set; } = new List<Friendship>(); //old user2
    }
}
