using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FurApp.Web.Entry
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // ����Ǩ���� FurApp.Web.Core/WebConfigureStartup.cs
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ����Ǩ���� FurApp.Web.Core/WebConfigureStartup.cs
        }
    }
}
