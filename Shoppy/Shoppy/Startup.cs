using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Shoppy.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shoppy
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
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseMySql(
                 Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                UserManager<IdentityUser> userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                CreateRoles(services).Wait();
                CreateAdminUser(userManager).Wait();
                CreateSellerUser(userManager).Wait();
                CreateBuyerUser(userManager).Wait();
            });
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //if changing roles change register.cshtm and register.cshtml.cs
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();            
            string[] roles = { "Admin", "Seller", "Buyer" };
            IdentityResult roleResult;

            foreach(var role in roles)
            {
                var roleCheck = await RoleManager.RoleExistsAsync(role);
                if(!roleCheck)
                {
                    //here in this line we are creating a role and seed it to the database
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(role));
                }
            }           
        }

        private async Task CreateAdminUser(UserManager<IdentityUser> userManager)
        {            
            IdentityUser user = await userManager.FindByEmailAsync(Configuration.GetSection("AdminSettings")["AdminEmail"]);
            if(user == null)
            {
                var adminUser = new IdentityUser {
                    UserName = Configuration.GetSection("AdminSettings")["AdminUserName"],
                    Email = Configuration.GetSection("AdminSettings")["AdminEmail"],
                    EmailConfirmed = true,
                    PhoneNumber = Configuration.GetSection("AdminSettings")["AdminPhoneNumber"],
                    PhoneNumberConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, Configuration.GetSection("AdminSettings")["AdminPassword"]);

                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        private async Task CreateSellerUser(UserManager<IdentityUser> userManager)
        {
            IdentityUser user = await userManager.FindByEmailAsync(Configuration.GetSection("SellerSettings")["SellerEmail"]);
            if(user == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = Configuration.GetSection("SellerSettings")["SellerUserName"],
                    Email = Configuration.GetSection("SellerSettings")["SellerEmail"],
                    EmailConfirmed = true,
                    PhoneNumber = Configuration.GetSection("SellerSettings")["SellerPhoneNumber"],
                    PhoneNumberConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, Configuration.GetSection("SellerSettings")["SellerPassword"]);

                await userManager.AddToRoleAsync(adminUser, "Seller");
            }
        }

        private async Task CreateBuyerUser(UserManager<IdentityUser> userManager)
        {
            IdentityUser user = await userManager.FindByEmailAsync(Configuration.GetSection("BuyerSettings")["BuyerEmail"]);
            if(user == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = Configuration.GetSection("BuyerSettings")["BuyerUserName"],
                    Email = Configuration.GetSection("BuyerSettings")["BuyerEmail"],
                    EmailConfirmed = true,
                    PhoneNumber = Configuration.GetSection("BuyerSettings")["BuyerPhoneNumber"],
                    PhoneNumberConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, Configuration.GetSection("BuyerSettings")["BuyerPassword"]);

                await userManager.AddToRoleAsync(adminUser, "Buyer");
            }
        }
    }
}
 