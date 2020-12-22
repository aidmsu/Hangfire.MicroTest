using Hangfire.Common;
using Hangfire.MicroTest.NewsletterService;
using Hangfire.MicroTest.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hangfire.MicroTest.OrdersService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config => config.UseApplicationConfiguration());
            services.AddHangfireServer(config => config.Queues = new [] { "orders", "default" });
            
            services.AddSingleton<IBackgroundJobClient>(provider => new CustomBackgroundJobClient(new BackgroundJobClient()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient client)
        {
            client.Enqueue(() => NewsletterSender.Execute(67890));
            client.Enqueue<NewsletterSender>(x => x.ExecuteInstance(67890));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World from Orders Service!"); });
            });
        }
    }
}