using BusinessLayer.DTOs.User;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DomainServices.Interfaces
{
    public interface IAuthService
    {
        Task<bool> FindByEmailAsync(string email);
        Task<bool> CreateAsync(UserCreateDTO dto);
        Task<ICollection<string>> ValidatePasswordAsync(string password);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<bool> PasswordSignInAsync(string email, string password);
        Task SignOutAsync();
    }
}
