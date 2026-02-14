using DataLayer.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class ProfileVM
    {

    }

    public class ProfileEditVM
    {
        [Display(Name = "Your Name")]
        [StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "Username Is Either Too Short Or Too Long")]
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; } //New UserName

        public string CurrentUserName { get; set; }

        [Display(Name = "Date of Birth")]
        //[Required(ErrorMessage = "Date of Birth is required")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public GenderOption Gender { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Current Password")]
        //[Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        //[Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        //[Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [StringLength(500, MinimumLength = 20, ErrorMessage = nameof(Bio) + " must be between {2} and {1} chars")]
        [Required(ErrorMessage = nameof(Bio) + " is required")]
        public string Bio { get; set; }

        //[BindNever]
        //public int Reputation { get; set; }

        //[BindNever]
        //public DateTime DateCreated { get; set; }

        public UploadAvatarVM Avatar { get; set; }/* = new();*/

        [BindNever]
        public int AvatarId { get; set; }

        public string AvatarUrl { get; set; }

        [BindNever]
        public string FilePath { get; set; }
    }

    public class ProfileGetVM
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
        public string AvatarUrl { get; set; }

        public string Role { get; set; }

        public int NumPosts { get; set; }
        public int NumComments { get; set; }

        public FriendshipStatusVM Friendship { get; set; }

        public IEnumerable<FriendshipStatusVM> IncomingRequests { get; set; } = new List<FriendshipStatusVM>();

        public IEnumerable<FriendVM> Friends { get; set; } = new List<FriendVM>();
    }
}
