using API.Middleware;
using API.Services;
using Application;
using Application.Common.Interfaces;
using Application.Common.Interfaces.BankAccount;
using Application.Services;
using Domain.Entities.Teledolar.Account;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Persistence.EfectivoYaDo;
using Infrastructure.Persistence.Fenix;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddInfrastructure(Configuration, Environment).AddApplication();
            services.AddHealthChecks().AddDbContextCheck<FenixDbContext>();
            services.AddHealthChecks().AddDbContextCheck<EfectivoYaDoDbContext>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddTransient<IServicePaymentExtra, ServicePaymentExtra>();
            services.AddTransient<IServicePayment, ServicePayment>();
            services.AddTransient<IServiceBankAccount, ServiceAccountBank>();

            services.AddHttpContextAccessor();
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<IFenixDbContext>())
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<IEfectivoYaDoDbContext>());
            services.AddApplicationInsightsTelemetry(f => { f.EnableDebugLogger = false; f.DeveloperMode = false; });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeledolarProcessAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Type into the textbox: Bearer {your JWT token}.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                },
                                Scheme = "Bearer",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                        },
                        new List<string> ()
                    }
                });
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment ()) {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeledolarProcessAPI v1"));
            //}
            app.UseHttpsRedirection();
            app.UseCustomExceptionHandler();
            app.UseRouting();
            app.UseAuthorization();
            app.UseRequestLoggingHandler();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
