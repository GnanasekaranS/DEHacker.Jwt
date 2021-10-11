using DEHacker.Businesslogic;
using DEHacker.Jwt.Helpers;
using DEHacker.Jwt.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;

namespace DEHacker.Jwt
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
            services.AddControllers();
            
            services.AddSwaggerGen(gen =>
            {
                gen.ResolveConflictingActions(api => api.First());
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                gen.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                gen.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            services.AddAzureClients(azureClientFactoryBuilder =>
            {
               // azureClientFactoryBuilder.AddSecretClient(Configuration.GetSection("KeyVault"));                
            });

          //  services.AddSingleton<IKeyVaultManager, KeyVaultManager>();

            services.AddTransient<IBusinessLayer, BusinessLayer>();
            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddApplicationInsightsTelemetry(Configuration);
        }       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApplicationInsightsRequestTelemetry();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(ep => { ep.MapControllers(); });
            app.UseSwagger();
            app.UseSwaggerUI(sw =>
            {
                sw.SwaggerEndpoint("/swagger/v1/swagger.json", "DE Hackers API service1");
                sw.DefaultModelsExpandDepth(-1);
                sw.DocExpansion(DocExpansion.None);
                sw.RoutePrefix = string.Empty;
            });
            app.UseApplicationInsightsExceptionTelemetry();
        }
    }
}
