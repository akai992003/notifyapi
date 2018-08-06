using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using notifyApi.Services;
using notifyApi.Data;

namespace notifyApi {
    public class Startup {
        private string _connectionStr;
      
    
        public IConfiguration Configuration { get; }
        public Startup (IHostingEnvironment env) {

            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true)
                .AddJsonFile ($"appsettings.{env.EnvironmentName}.json", optional : true)
                .AddEnvironmentVariables ();
            Configuration = builder.Build ();

            _connectionStr = UStore.GetUStore (Configuration["ConnectionStrings:Dev"]);

        }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices (IServiceCollection services) {
            var connectionStr = _connectionStr;

            services.AddDbContext<NotifyContext> (options => options.UseSqlServer (connectionStr));
            services.AddScoped<IMemberService, MemberService> ();
            services.AddScoped<IhnNotifyService, hnNotifyService> ();
            services.AddScoped<IhnNotifyItemService, hnNotifyItemService> ();
            // services.AddScoped<IEnergyCaseStringService, EnergyCaseStringService> ();
            // services.AddScoped<IEnergyCaseMicroService, EnergyCaseMicroService> ();;
            // services.AddScoped<IInfoSettingsService, InfoSettingsService> ();
            // services.AddScoped<ICityInfoService, CityInfoService> ();
            // services.AddScoped<IMsgLogService, MsgLogService> ();
            // services.AddScoped<IUserRoleService, UserRoleService> ();

            //加入 appsettings.json 自訂的參數取得
            services.AddSingleton<IConfiguration> (Configuration);

            // CorsDomain
            services.AddCors (options => options.AddPolicy ("CorsDomain",
                p => p.WithOrigins ("http://61.220.206.97",
                    "http://www.enluxsolar.com",
                    "http://localhost",
                    "http://localhost:4200",
                    "http://localhost:4201",
                    "http://localhost:4202",
                    "http://localhost:4203",
                    "http://localhost:4207",
                    "http://localhost:5000",
                    "http://127.0.0.1:8887",
                    "http://127.0.0.1"
                ).AllowAnyMethod ().AllowAnyHeader ()));

            // JWT
            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Tokens:ValidIssuer"],
                    ValidAudience = Configuration["Tokens:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (Configuration["Tokens:IssuerSigningKey"])),
                    RequireExpirationTime = true,
                    };
                    options.Events = new JwtBearerEvents () {
                        OnAuthenticationFailed = context => {
                                context.NoResult ();

                                context.Response.StatusCode = 401;
                                context.Response.HttpContext.Features.Get<IHttpResponseFeature> ().ReasonPhrase = context.Exception.Message;
                                Debug.WriteLine ("OnAuthenticationFailed: " + context.Exception.Message);
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context => {
                                Console.WriteLine ("OnTokenValidated: " +
                                    context.SecurityToken);
                                return Task.CompletedTask;
                            }

                    };
                });

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseHsts ();
            }

            app.UseStaticFiles ();
            /// jwt 啟用驗證 
            app.UseAuthentication ();

            // app.UseHttpsRedirection ();

            // for CorsDomain
            app.UseCors ("CorsDomain");

            app.UseMvc ();

        }
    }
}