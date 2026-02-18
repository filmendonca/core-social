using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Enums
{
    public enum VisibilityType
    {
        Public = 0,

        [Display(Name = "Friends Only")]
        FriendsOnly = 1,

        Restricted = 2
    }
}
