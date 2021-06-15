using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Facebook
{
    public class Program
    {
        public static void Main(string[] args)
        {
          

            var postOnWallTask2 = FaceBookController.PublishImages("Hello from C# .NET Core!");
            Task.WaitAll(postOnWallTask2);

            // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
