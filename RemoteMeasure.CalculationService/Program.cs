using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace RemoteMeasure.CalculationService
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                x.SetServiceName("CalculationService");
                x.SetDisplayName("Calculation Service");
                x.SetDescription("Akka.NET Remoting Demo - Calculation Service");

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.Service<CalculationService>();
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
