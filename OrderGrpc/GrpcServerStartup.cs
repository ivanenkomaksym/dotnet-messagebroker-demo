using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using OrderAPI.Services;

namespace OrderGrpc
{
    public class GrpcServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GrpcOrderService>();
                endpoints.MapGrpcHealthChecksService();
            });
        }
    }
}