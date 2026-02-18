using BusinessLayer.DTOs.Attachment;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs.User
{
    public class UserDTO : UserCreateDTO
    {
        public string Id { get; set; }

        //[Required]
        //[StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "Username Is Either Too Short Or Too Long")]
        //public string UserName { get; set; }

        //[Required]
        //[StringLength(25, ErrorMessage = "Your Password is limited to {2} and {1} characters", MinimumLength = 5)]
        //public string Password { get; set; }

        //[Required]
        //public string Email { get; set; }

        //[Required]
        //public DateTime? BirthDate { get; set; }

        //[Required]
        //public GenderOption Gender { get; set; }

        [Required]
        public string Avatar { get; set; }

        [Required]
        public string Bio { get; set; }

        [Required]
        public bool Banned { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }

    public class UserGetDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public AttachmentGetDTO Avatar { get; set; }
        public GenderOption Gender { get; set; }
        public string Bio { get; set; }
    }

    public class UserCreateDTO
    {
        [Required]
        //[StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "Username Is Either Too Short Or Too Long")]
        public string UserName { get; set; }

        [Required]
        public DateTime? BirthDate { get; set; }

        [Required]
        public GenderOption Gender { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        //[StringLength(25, ErrorMessage = "Your Password is limited to {2} and {1} characters", MinimumLength = 5)]
        public string Password { get; set; }

        public DataCreation CreatedBy { get; set; } = DataCreation.User;
        public DataCreation UpdatedBy { get; set; } = DataCreation.User;
    }

    public class UserLoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserEditDTO
    {
        [Required]
        //[StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "Username Is Either Too Short Or Too Long")]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Avatar { get; set; }

        [Required]
        public GenderOption Gender { get; set; }

        [Required]
        public string Bio { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
