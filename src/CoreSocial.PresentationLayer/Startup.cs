using AutoMapper;
using BusinessLayer.Mapper;
using BusinessLayer.DomainServices;
using BusinessLayer.DomainServices.Interfaces;
using DataLayer.Context;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repositories;
using DataLayer.Seeder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PresentationLayer.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BusinessLayer.ApplicationServices.Post;
using DataLayer.Storage;
using Utils.Extensions;
using BusinessLayer.ApplicationServices.Profile;
using BusinessLayer.ApplicationServices.Reaction;
using Microsoft.Extensions.Logging;

namespace PresentationLayer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region DbContext

            services.AddDbContext<AppDbContext>(options =>
                options
                //Add connection string
                .UseSqlServer(Configuration.GetConnectionString("MainConnectionString"),
                //Get migration assembly stored in appsettings
                b => b.MigrationsAssembly(Configuration.GetSection("MigrationAssembly").Get<string>()))

                //.LogTo(Console.WriteLine, LogLevel.Information)

                //Enable/Disable lazy loading
                .UseLazyLoadingProxies(true)
            );

            #endregion

            #region Identity - Authentication and Authorization

            services.AddIdentity<User, IdentityRole>(setup =>
            {
                setup.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>();
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                //Session timeout
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            #endregion

            #region Services
            
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IFriendshipService, FriendshipService>();
            services.AddScoped<CrudPostService>();
            services.AddScoped<UploadAvatarService>();
            services.AddScoped<ReactionService>();

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReactionRepository, ReactionRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IFileStorage, FileStorage>();

            services.AddHostedService<FileDeletionService>();

            #endregion

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddAutoMapper(typeof(PresentationMapper), typeof(BusinessMapper));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                AppDbSeeder.SeedUsersAsync(app).Wait();
                AppDbSeeder.SeedEntitiesAsync(app).Wait();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            AppDbSeeder.SeedRolesAsync(app).Wait();

            //If there is an error somewhere, the flow will be redirected to a specific error page
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            //Extension method
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
