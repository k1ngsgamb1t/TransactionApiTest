using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.Swagger;
using TransactionApi.Server.Data;
using TransactionApi.Server.Services;
using TransactionApi.Server.Services.Interfaces;

namespace TransactionApi.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TransactionDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DbConnection"));
            });
            services.AddSwaggerGen(sw =>
            {
                sw.SwaggerDoc("v1", new OpenApiInfo { Title = "TransactionAPI", Version = "v1" });
                sw.CustomSchemaIds(x => x.FullName);
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xmlFile => sw.IncludeXmlComments(xmlFile));
            });
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ITransactionParser, CsvTransactionParser>();
            services.AddScoped<ITransactionParser, XmlTransactionParser>();
            services.AddScoped<ITransactionProcessor, TransactionProcessor>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "TransactionAPI v1");
                c.RoutePrefix = "api/swagger";
                c.DocumentTitle = "Transaction API Documentation";
                c.DocExpansion(DocExpansion.None);
            });


            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            
            RunMigrations(app);
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

        private static void RunMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<TransactionDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
