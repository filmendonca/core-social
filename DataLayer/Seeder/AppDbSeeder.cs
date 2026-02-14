using DataLayer.Enums;
using DataLayer.Models;
using Utils.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DataLayer.Context;

namespace DataLayer.Seeder
{
    public static class AppDbSeeder
    {
        private static RoleManager<IdentityRole> _roleManager;
        private static UserManager<User> _userManager;

        static AppDbSeeder() { }

        public static async Task SeedRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

            _roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Moderator))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Moderator));
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
        }

        public static async Task SeedUsersAsync(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

            _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var adminEmail = "admin@mail.com";
            var moderatorEmail = "moderator@mail.com";
            var userEmail = "user@mail.com";
            var password = Environment.GetEnvironmentVariable("SOC_MED_PASS");

            if (password == null)
                throw new Exception("SOC_MED_PASS environment var not found.");

            //create admin
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User()
                {
                    Email = adminEmail,
                    UserName = "Admin",
                    BirthDate = DateTime.UtcNow,
                    AvatarId = null,
                    Gender = GenderOption.Male,
                    CreatedBy = DataCreation.System
                };

                await _userManager.CreateAsync(admin, password);
                await _userManager.AddToRoleAsync(admin, UserRoles.Admin);
            }

            //create moderator
            if (await _userManager.FindByEmailAsync(moderatorEmail) == null)
            {
                var moderator = new User()
                {
                    Email = moderatorEmail,
                    UserName = "Moderator",
                    BirthDate = DateTime.UtcNow,
                    AvatarId = null,
                    Gender = GenderOption.Male,
                    CreatedBy = DataCreation.System
                };

                await _userManager.CreateAsync(moderator, password);
                await _userManager.AddToRoleAsync(moderator, UserRoles.Moderator);
            }

            //create user
            if (await _userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new User()
                {
                    Email = userEmail,
                    UserName = "User",
                    BirthDate = DateTime.UtcNow,
                    AvatarId = null,
                    Gender = GenderOption.Male,
                    CreatedBy = DataCreation.System
                };

                await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
        }

        public static async Task SeedEntitiesAsync(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            if (context.Posts.Any() && context.Comments.Any()) { return; }

            await context.Database.EnsureCreatedAsync();

            var seedUser = context.Set<User>().FirstOrDefault(u => u.Email ==  "admin@mail.com");

            var post = new Post()
            {
                Title = "Seed Post",
                Content = "Seed Content",
                Image = null,
                Popularity = 4,
                Topic = TopicName.Technology,
                UserId = seedUser.Id,
                CreatedBy = DataCreation.System
            };

            if (!context.Posts.Any())
            {
                await context.AddAsync(post);
                await context.SaveChangesAsync();
            }

            var seedPost = context.Set<Post>().FirstOrDefault(p => p.CreatedBy != DataCreation.System);

            //Comment[] comments = { 
            //    new Comment()
            //    {
            //        Content = "Seed Content",
            //        PostId = seedPost.Id,
            //        UserId = seedUser.Id,
            //        CreatedBy = DataCreation.System
            //    },
            //    new Comment()
            //    {
            //        Content = "Seed Content",
            //        PostId = seedPost.Id,
            //        UserId = seedUser.Id,
            //        CreatedBy = DataCreation.System
            //    },
            //    new Comment()
            //    {
            //        Content = "Seed Content",
            //        PostId = seedPost.Id,
            //        UserId = seedUser.Id,
            //        CreatedBy = DataCreation.System
            //    }
            //};

            //if (!context.Comments.Any())
            //{
            //    await context.AddRangeAsync(comments);
            //    await context.SaveChangesAsync();
            //}

            //if (!context.Friendships.Any())
            //{
            //    await context.AddAsync(new Friendship()
            //    {
            //        Status = FriendshipStatus.Accepted,
            //        UserId1 = "13155f45-8582-4920-b761-37e6c522d812",
            //        UserId2 = "4b7e6e32-0033-4fd6-97b7-ca621dd48092"
            //    }
            //    );
            //    await context.SaveChangesAsync();
            //}

            //if (!context.Warnings.Any())
            //{
            //    await context.AddAsync(new Warning()
            //    {
            //        Reason = BanReasons.Spam,
            //        Description = "Seed Description",
            //        ActiveUntil = DateTime.Now.AddDays(10),
            //        UserId = "13155f45-8582-4920-b761-37e6c522d812",
            //        ModeratorId = "4b7e6e32-0033-4fd6-97b7-ca621dd48092"
            //    }
            //    );
            //    await context.SaveChangesAsync();
            //}

            //await context.SaveChangesAsync();
        }
    }
}
