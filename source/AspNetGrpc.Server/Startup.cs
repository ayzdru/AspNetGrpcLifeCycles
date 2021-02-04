using AspNetGrpc.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetGrpc.Server
{
    public class Startup
    {
        public const string AllowOriginPolicyName = "AllowOrigin";
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            services.AddGrpc();            
            services.AddCors(options =>
            {
                options.AddPolicy(AllowOriginPolicyName,
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(AllowOriginPolicyName);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapGrpcService<UnaryService>();
                endpoints.MapGrpcService<ClientStreamingService>();
                endpoints.MapGrpcService<ServerStreamingService>();
                endpoints.MapGrpcService<BidirectionalStreamingService>();
            });
        }
    }
}
