using Hangfire.Common;
using Hangfire.MicroTest.NewsletterService;
using Hangfire.MicroTest.Shared;
using Hangfire.Server;
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

            services.AddMicroservices();

            services.AddHangfireServer(config =>
            {
                config.Queues = new[] {"orders", "default"};
                
            });
            
            services.AddSingleton<IBackgroundJobClient>(provider => new MicroserviceBackgroundJobClient(new BackgroundJobClient()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient client)
        {
            var jobId = client.Enqueue(() => NewsletterSender.ExecuteGeneric(67890));
            BackgroundJob.ContinueJobWith(jobId, () => NewsletterSender.Execute(67891, null));
            BackgroundJob.Enqueue<NewsletterSender>(x => x.ExecuteInstance(67892));

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