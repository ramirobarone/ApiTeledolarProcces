using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Application.Common.Interfaces.BankAccount;
using Application.Services;
using Domain.Entities.Teledolar.Account;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddTransient<IDomiciliacionesService, DomiciliacionesService>();
            services.AddTransient<IAdaService, AdaService>();
            services.AddTransient<IServiceBankAccount, ServiceAccountBank>();
            services.AddTransient<IHealthService, ServicesHealthDataBase>();

            return services;
        }
    }
}
