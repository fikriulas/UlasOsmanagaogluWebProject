using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UlasBlog.Data.Abstract;
using UlasBlog.Data.Concrete.EntityFramework;
using UlasBlog.WebUI.IdentityCore;
using UlasBlog.WebUI.IdentityCore.CustomValidation;
using UlasBlog.WebUI.Middleware;
using Microsoft.Extensions.Logging;
using UlasBlog.WebUI.Logs;

namespace UlasBlog.WebUI
{
    public partial class Startup
    {
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment { get; set; }
        // public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("UlasBlog.WebUI")));
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"), b => b.MigrationsAssembly("UlasBlog.WebUI")));

            services.AddIdentity<AppUser, AppRole>(opts =>
            {

                //user default validation
                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._şığüç";
                //password default validation
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireDigit = true;
            }).AddPasswordValidator<CustomPasswordValidator>()
            .AddUserValidator<CustomUserValidator>()
            .AddRoleValidator<CustomRoleValidator>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppIdentityDbContext>();
            //Cookie bazlı kimlik doğrulama
            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "MyBlog";
            cookieBuilder.HttpOnly = true; // sadece http isteği üzerinden cookie bilgisi okur. Client cookie bilgisini göremez.            

            //cookieBuilder.Expiration = System.TimeSpan.FromMinutes(60); //cookie 60 dk kalır.
            cookieBuilder.SameSite = SameSiteMode.Strict; // sadece ilgili site üzerinden bu cookie erişilir. Başka site üzerinden bu cookie erişilmez. (lax bu özelliği kapatır.) Strict ile csrf açığı kapatılır.
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;   //always seçilirse cookie bilgisini sadece https üzerinden gönderir./sameasRequest ile http ve https üzerinden gönderir. İsteğe bağlı.
            services.ConfigureApplicationCookie(opts =>
            {
                opts.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
                opts.LoginPath = new PathString("/Home/Login"); // yetkisi olmayan kullanıcıların yönlendirileceği path.
                opts.AccessDeniedPath = new PathString("/Admin/AccessDenied"); // rolü dışında giriş yapılırsa bu path'e yönlendirilir.
                opts.Cookie = cookieBuilder;
                opts.SlidingExpiration = true; // cookiebuilder.expiration'da belirlenen sürenin yarısında kullanıcı giriş yaparsa bu expiration süresi ikiye katlanır.  

            });
            //Cokie end 
            services.AddControllersWithViews().AddRazorRuntimeCompilation().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); // runtime comp.
            services.AddControllersWithViews();
            services.AddTransient<IUnitOfWork, EfUnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new LoggerProvider(_hostingEnvironment));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<IPControlMiddleware>();            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //Identity için middware eklenir.
            app.UseAuthentication();
            app.UseRouting();
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
