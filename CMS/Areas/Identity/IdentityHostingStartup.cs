using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CMS.Areas.Identity.IdentityHostingStartup))]
namespace CMS.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}