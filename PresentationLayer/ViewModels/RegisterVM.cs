using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    //View Model for the registration of an user
    public class RegisterVM
    {
        [Display(Name = "Your Name")]
        [StringLength(maximumLength: 20, MinimumLength = 3, ErrorMessage = "Username Is Either Too Short Or Too Long")]
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [Display(Name = "Date of Birth")]
        //[Required(ErrorMessage = "Date of Birth is required")]
        public DateTime? BirthDate { get; set; } = null;

        [Required(ErrorMessage = "Gender is required")]
        public GenderOption Gender { get; set; } = GenderOption.Other;

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
