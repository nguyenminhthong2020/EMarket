using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using EMarket.Data;
using EMarket.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace EMarket
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
            services.AddControllersWithViews()
                    .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var strConnectDb = Configuration.GetConnectionString("EMarketContext");
            services.AddDbContext<dbMarketsContext>(options =>
                    options.UseSqlServer(strConnectDb));

            // Session sẽ được lưu vào bộ nhớ đệm phân tán
            // Distributed memory cache
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                //TimeSpan(7,0,0,0) : 7 ngày
                //options.IdleTimeout = new TimeSpan(0, 30, 0); tương đương với
                options.IdleTimeout = TimeSpan.FromMinutes(120);
                options.Cookie.Name = "EMarketSession";  // không bắt buộc
            });

            // Có xài không ?
            // https://stackoverflow.com/questions/47324129/no-authenticationscheme-was-specified-and-there-was-no-defaultchallengescheme-f
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(p =>
                    {
                        p.LoginPath = "/login.html";
                        p.AccessDeniedPath = "/";
                    });
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            //    options =>
            //    {
            //        options.LoginPath = new PathString("/auth/login");
            //        options.AccessDeniedPath = new PathString("/auth/denied");
            //    });



            // Để hiểu các ký tự tiếng Việt, unicode.
            // Còn nhớ lúc in value của form hồi xưa không ? 
            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

            services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.TopCenter; });
            services.AddToastify(config => { config.DurationInSeconds = 1000; config.Position = Position.Left; config.Gravity = Gravity.Bottom; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseNotyf();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();   // bắt buộc phải nằm sau useRouting & nằm trước useEndpoints
            app.UseMiddleware<MyAuthMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
