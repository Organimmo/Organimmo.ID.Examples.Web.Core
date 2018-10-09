// --------------------------------------------------
// Cleanup date: 09/10/2018 14:55
// Cleanup user: Michael Roef
// --------------------------------------------------

#region NAMESPACES

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

#endregion

namespace Organimmo.ID.Examples.Web.Core
{
    public class Program
    {
        #region METHODS

        #region PUBLIC

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        #endregion

        #endregion
    }
}