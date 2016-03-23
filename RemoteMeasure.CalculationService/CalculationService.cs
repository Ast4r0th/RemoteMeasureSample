using Akka.Actor;
using RemoteMeasure.CalculationService.Actors;
using Topshelf;

namespace RemoteMeasure.CalculationService
{
    public class CalculationService : ServiceControl
    {
        protected ActorSystem System { get; private set; }

        public bool Start(HostControl hostControl)
        {
            System = ActorSystem.Create("measurement");
            IActorRef calcActor = System.ActorOf(Props.Create<CalculationActor>(), "Calculation");
            System.ActorOf(Props.Create(() => new MeasureReceiveActor(calcActor)), "Receive");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            System.Terminate().Wait();
            return true;
        }
    }
}
