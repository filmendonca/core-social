using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class UserGetVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        //public string Avatar { get; set; }
        public GenderOption Gender { get; set; }
        public string Bio { get; set; }
    }
}
