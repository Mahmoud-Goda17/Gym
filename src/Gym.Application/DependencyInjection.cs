
using FluentValidation;
using FluentValidation.AspNetCore;
using Gym.Application.Interfaces;
using Gym.Application.Services;
using Gym.Application.Services.Gym.Application.Services;
using Gym.Application.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AssemblyRef>();


            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClassBookingService, ClassBookingService>();
            services.AddScoped<IGymClassService, GymClassService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddHttpClient("Fawaterk", (sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                client.BaseAddress = new Uri(
                    config["Fawaterk:BaseUrl"]!);

                client.DefaultRequestHeaders.Add(
                    "Authorization",
                    $"Bearer {config["Fawaterk:ApiKey"]}"
                );
            });

            services.AddMemoryCache(options=>options.SizeLimit=100);


            return services;

        }
        
    }
}
