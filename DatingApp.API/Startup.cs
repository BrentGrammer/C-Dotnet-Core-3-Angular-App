using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // NOTE: The order does not matter in here - (does in Configure method) - only order to ake it easier to read
        //  first few tutorial videos by Mike Taulty on youtube. He is from Microsoft and excellently explains how it works and why it would be used in an application. 
        public void ConfigureServices(IServiceCollection services)
        {
            // add DataContext service and pass in the database engine being used and a connection string
            // connection string comes from appsettings.json
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            // add CORS as a service to prevent corss origin errors in browser on front end
            services.AddCors();

            // adding auth repository to be available and injecfted into app
            // NOTE: AddScoped is used for the vast majority of services - AddTRansient and AddSingleton are only used if specifically needed
            // add scope uses same instance of service for all associated calls within a web request, but one per http web request.
            // rare to use the AddSingleton scope which will keep a service alive for the duration of the app - the only  things we typically use this for are for logging and caching.
            //transient objects are always different; a new instance is provided to every controller and every service.
            // see https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences

            //specify intrace and concrete implementation
            services.AddScoped<IAuthRepository, AuthRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            // order is important here - place sooner/later if doesn't work at first
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
