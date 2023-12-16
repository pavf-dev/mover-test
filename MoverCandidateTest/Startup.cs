using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoverCandidateTest.Application.Events;
using MoverCandidateTest.Application.InventoryItems;
using MoverCandidateTest.Application.WatchHands;
using MoverCandidateTest.Infrastructure;

namespace MoverCandidateTest
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
            services.AddSingleton<IWatchHandsAngleCalculator, WatchHandsAngleCalculator>();
            services.AddSingleton<WatchHandsAngleDifferenceCalculator>();
            services.AddSingleton<IInventoryDomainEventsRepository, InventoryDomainEventsRepository>();
            services.AddSingleton<IInventoryItemDtoValidator, InventoryItemDtoValidator>();
            services.AddSingleton<InventoryItemsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
