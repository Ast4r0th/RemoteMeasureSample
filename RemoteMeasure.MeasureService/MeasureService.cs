using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using RemoteMeasure.MeasureService.Actors;
using Topshelf;

namespace RemoteMeasure.MeasureService
{
    public class MeasureService : ServiceControl
    {
        protected ActorSystem System { get; private set; }

        public bool Start(HostControl hostControl)
        {
            System = ActorSystem.Create("measurement");
            var sendActor = System.ActorOf(Props.Create<SendActor>(), "Send");
            var measureRetrieveActor = System.ActorOf(Props.Create(() => new MeasureRetrieveActor(sendActor)), "MeasureRetrieve");
            measureRetrieveActor.Tell(new Messages.StartMonitoring());
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            System.Terminate().Wait();
            return true;
        }
    }
}
