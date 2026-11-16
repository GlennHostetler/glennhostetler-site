using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GlennSite
{
    // This app is a small web server whose only job is to serve the static site in wwwroot/.
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // No services are needed to serve static files.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // HTTPS is handled by Azure Container Apps' ingress in front of this container,
            // which also redirects http to https. The container itself only speaks plain HTTP on
            // port 8080, so there is deliberately no UseHttpsRedirection() here.

            // "/" serves wwwroot/index.html; everything else is served as the file it names.
            // Any other URL gets a normal 404.
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
