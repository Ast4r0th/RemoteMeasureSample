using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace RemoteMeasure.MeasureService
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                x.SetServiceName("MeasureService");
                x.SetDisplayName("Measure Service");
                x.SetDescription("Akka.NET Remoting Demo - Measure Service");

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.Service<MeasureService>();
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
