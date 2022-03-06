using Application.Common.Interfaces;
using Infrastructure.Persistence.EfectivoYaDo;
using Infrastructure.Persistence.Fenix;
using Infrastructure.Proxies;
using Infrastructure.Proxies.Teledolar;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IEncryptService, EncryptService>();
            services.AddTransient<ProxiesLoggingHandler>((s) =>
            {
                return new ProxiesLoggingHandler(s.GetService<ILogger<ProxiesLoggingHandler>>());
            });

            services.AddDbContext<FenixDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("FenixConnection")));

            services.AddDbContext<EfectivoYaDoDbContext>(options =>
               options.UseMySQL(configuration.GetConnectionString("EfectivoYaDoConnection")));

            services.AddScoped<IFenixDbContext>(provider => provider.GetService<FenixDbContext>());
            services.AddScoped<IEfectivoYaDoDbContext>(provider => provider.GetService<EfectivoYaDoDbContext>());
            services.AddScoped<IAdaProxy, AdaTeledolarProxie>();
            services.AddScoped<IServiceProxyGet, AccountVerificationTeledolar>();
            services.AddScoped<IServiceSingleDomiciliation, SingleDomiciliationProxy>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();

            services.AddHttpClient<ITeledolarProxy, DomiciliacionesTeledolarProxy>().AddHttpMessageHandler<ProxiesLoggingHandler>();

            services.Configure<TeledolarSettings>(configuration.GetSection(TeledolarSettings.AppSettingsSection));
            //services.Configure<GetConfigurations>(configuration.GetSection(TeledolarSettings.AppSettingsSection));

            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                        ValidAudience = configuration.GetSection("Jwt:Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Value)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddAuthorization(options =>
            {

            });

            return services;
        }
    }
}
