using Arcora.Payments.Abstractions;
using Arcora.Payments.Configuration;
using Arcora.Payments.Stripe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcora.Payments.DependencyInjection
{
    /// <summary>
    /// Registration entry point for the Arcora payments module.
    /// </summary>
    public static class PaymentsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Stripe-backed payment provider and its configuration.
        /// </summary>
        public static IServiceCollection AddArcoraPayments(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<StripeOptions>()
                .Bind(configuration.GetSection(StripeOptions.SectionName));

            services.AddSingleton<IPaymentProvider, StripePaymentProvider>();

            return services;
        }
    }
}
