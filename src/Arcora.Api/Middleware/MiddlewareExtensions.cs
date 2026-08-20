// ===================================THIS FILE WAS AUTO GENERATED===================================
namespace Arcora.Api.Middleware
{
    /// <summary>
    /// Extension methods for registering custom middleware in the application pipeline.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds the global exception handling middleware to the application pipeline.
        /// This should be registered early in the pipeline to catch all unhandled exceptions.
        /// </summary>
        /// <param name = "app">The application builder.</param>
        /// <returns>The application builder for chaining.</returns>
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}