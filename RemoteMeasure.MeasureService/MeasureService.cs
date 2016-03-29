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

            IActorRef unrecvdMsgActor = System.ActorOf(Props.Create<UnreceivedMessagesActor>(), "UnreceivedMessages");
            IActorRef sendActor = System.ActorOf(Props.Create(() => new SendActor(unrecvdMsgActor)), "Send");
            IActorRef measureRetrieveActor = System.ActorOf(Props.Create(() => new MeasureRetrieveActor(sendActor)), "MeasureRetrieve");

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
