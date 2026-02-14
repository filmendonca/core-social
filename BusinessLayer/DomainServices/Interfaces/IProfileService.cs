using BusinessLayer.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DomainServices.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileGetDTO> GetUserByIdAsync(string id);
        Task<ProfileGetDTO> GetUserByNameAsync(string username);
        Task<bool> EditAsync(ProfileEditDTO dto);
    }
}
