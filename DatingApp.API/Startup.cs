using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Helpers;
using AutoMapper;

namespace DatingApp.API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This is created to run SQLite in development mode
    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
      // add DataContext service and pass in the database engine being used and a connection string
      // connection string comes from appsettings.json
      services.AddDbContext<DataContext>(x =>
      {
        // LAZY LOADING to address warnings about unnecessary include usage with navprops from the CountAsync() call in PagedList.cs (because at that time it executes the IQueryable before the included Photos are used or returned)
        // Now in the dating repository, you can remove all include statements! EF will know what it needs to use and include automatically
        x.UseLazyLoadingProxies();
        x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
      });

      // now that we use SQLite in development mode, call the rest of the services
      ConfigureServices(services);
    }

    // This is created to run SQLite in development mode
    public void ConfigureProductionServices(IServiceCollection services)
    {
      // add DataContext service and pass in the database engine being used and a connection string
      // connection string comes from appsettings.json
      services.AddDbContext<DataContext>(x =>
      {
        // LAZY LOADING to address warnings about unnecessary include usage with navprops from the CountAsync() call in PagedList.cs (because at that time it executes the IQueryable before the included Photos are used or returned)
        // Now in the dating repository, you can remove all include statements! EF will know what it needs to use and include automatically
        x.UseLazyLoadingProxies();
        x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

      });

      // now that we use SQLite in development mode, call the rest of the services
      ConfigureServices(services);
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // NOTE: The order does not matter in here - (does in Configure method) - only order to ake it easier to read
    //  first few tutorial videos by Mike Taulty on youtube. He is from Microsoft and excellently explains how it works and why it would be used in an application. 
    public void ConfigureServices(IServiceCollection services)
    {
      //using Newtonsoft serializer over 3.0 System.Text for more features-install as Nuget Package - Microsoft.AspNetCore.Mvc.NewtonSoftJson
      services.AddControllers().AddNewtonsoftJson(opt =>
      {
        //this needs to be done to prevent reference loop error when you have navigation props (Photos in User) - ignore the error.
        opt.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      });
      // add CORS as a service to prevent corss origin errors in browser on front end
      services.AddCors();

      //Cloudinary configuration mapping valuies in appsettings.json to the settings class created:
      services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

      //AutoMapper needs to know which assembly the profiles for mapping to use - give it the name of the class
      // using AutoMapper to import
      services.AddAutoMapper(typeof(DatingRepository).Assembly);

      // adding auth repository to be available and injecfted into app
      // NOTE: AddScoped is used for the vast majority of services - AddTRansient and AddSingleton are only used if specifically needed
      // add scope uses same instance of service for all associated calls within a web request, but one per http web request.
      // rare to use the AddSingleton scope which will keep a service alive for the duration of the app - the only  things we typically use this for are for logging and caching.
      //transient objects are always different; a new instance is provided to every controller and every service.
      // see https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences

      //specify intrace and concrete implementation
      services.AddScoped<IAuthRepository, AuthRepository>();
      services.AddScoped<IDatingRepository, DatingRepository>();

      // Configure auth middleware - used for Authorizing calls in routes to tell app how to authenticate
      // after done configuring you need to add and register the middleware in the Configure block.
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // override the default schema with jet auth
          .AddJwtBearer(options =>
          {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
              // validate the signing key (from app settings used as the salt) and tell it where th key is
              ValidateIssuerSigningKey = true,
              // Requires bytes as the argument for the key, so need to encode it
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
              ValidateIssuer = false,  // both issuer and audience is localhost at this point, so don't validate here yet
              ValidateAudience = false
            };
          });

      // add the action filter to update the last active field when a user hits the user controller routes (LogUserActivity.cs)
      // use add scoped since you want to create a new instance of the class for every request
      services.AddScoped<LogUserActivity>();
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
        // if not in development setup a global exception handler - builtin to dotnet
        // this catchwes exceptions and handles them in requests - returns inernal error to client by default
        app.UseExceptionHandler(builder =>
        {
          // with the builder you can control status codes returned for errors
          // this will run a different request pipeline when an error occurs
          builder.Run(async httpContext =>
          {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //write eror message into http response:
            var error = httpContext.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
              //this ectension method made in helpers folder adds a header to the respomnse to contain the error
              httpContext.Response.AddApplicationError(error.Error.Message);
              // NOTE: make sure you import     using Microsoft.AspNetCore.Http;
              await httpContext.Response.WriteAsync(error.Error.Message);
            }
          });
        });
      }

      // app.UseHttpsRedirection();

      app.UseRouting();
      // order is important here - place sooner/later if doesn't work at first
      // allowing all these cors props is not a security risk **You definitley don't want to add AllowCredentials() as that would be a securty risk
      app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      // register jwt auth middleware configured above here
      app.UseAuthentication();
      app.UseAuthorization();


      app.UseAuthorization();

      /*
        Setup for serving static files (i.e. from your SPA client)
        -For this to work, you need to have your SPA build script output the build files to a folder in the root 
        of the dotnet project - in this case wwwroot folder in DatingApp.API
      */
      // look for default asset files like index.html etc., if it finds the file, it serves it
      app.UseDefaultFiles();
      // add ability for web server to use static files
      app.UseStaticFiles();



      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        // tell .NET to use the Fallback controller (which serves index.html) for any routes it does not recognize
        // this is done to defer handling of Angular SPA routing to the client on angular routes
        // the first arg is the name of the method in the controller to use and the second is the name of the controller
        endpoints.MapFallbackToController("Index", "Fallback");
      });

    }
  }
}
