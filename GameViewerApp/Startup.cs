using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;

namespace GameViewerApp
{
    public class Startup
    {
        private GameData gameData = new GameData();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var rb = new RouteBuilder(app);

            rb.MapGet("entry/{*name}", async ctx =>
            {
                var entryName = ctx.GetRouteValue("name").ToString();
                var entry = gameData.GetEntryAtPath(entryName);

                if (entry is IGameDataFileEntry fileEntry)
                {
                    await fileEntry.ExportAsync(ctx.Response.Body);
                }
                else if (entry is IGameDataDirectoryEntry dirEntry)
                {
                    var entries = dirEntry.GetEntries();

                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.WriteAsync(entries.Select(a => a.Name).Aggregate((a, b) => a + "\n" + b));
                }
            });

            app.UseRouter(rb.Build());

            app.Run(async (context) =>
            {

                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
