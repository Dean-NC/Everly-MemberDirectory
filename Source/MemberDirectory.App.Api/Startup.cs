using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MemberDirectory.Data;
using MemberDirectory.Data.Interfaces;
using MemberDirectory.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MemberDirectory.App.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // ConfigureServices() is called by the runtime. Use it to add services to the dependency injection container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //--------------
            // Swagger documentation
            //--------------
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MemberDirectory.App.Api", Version = "v1" });

                // Provide Swagger with the path to the comments file.
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //--------------
            // Add things to the dependency-injection container
            //--------------
            // DbConfig holds the connection string. Add as a singleton to be given to each repository.
            services.AddSingleton(_ => new DbConfig(
                Configuration.GetConnectionString("main")
            ));

            // Data repositories
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();

            // Anonymous URL shortener service is singleton
            services.AddSingleton<Interfaces.IUrlShortener, Services.AnonymousUrlShortener>();

            // HtmlParser
            services.AddScoped<Interfaces.IHtmlParser, Services.HtmlParser>();

            // Member service
            services.AddScoped<Services.MemberService, Services.MemberService>();

            // Friendship service
            services.AddScoped<Services.FriendshipService, Services.FriendshipService>();
        }

        // Configure() is called by the runtime. Use it to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MemberDirectory.App.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
