using System;
using System.Configuration;
using Akka.Actor;
using RemoteMeasure.Common.Messages;
using RemoteMeasure.MeasureService.Messages;

namespace RemoteMeasure.MeasureService.Actors
{
    public class SendActor : ReceiveActor
    {
        private ActorSelection _calcSelection;
        private IActorRef _unreceivedMsgActor;

        public SendActor(IActorRef unreceivedMsgActor)
        {
            _unreceivedMsgActor = unreceivedMsgActor;
            Ready();
        }

        protected override void PreStart()
        {
            string location = ConfigurationManager.AppSettings["ServerLocalion"];
            string actorPath = new Uri(new Uri(location), "/user/Receive").AbsoluteUri;
            _calcSelection = Context.ActorSelection(actorPath);
        }

        private void Ready()
        {
            Receive<InternalMeasureData>(data =>
            {
                var transferData = new MeasureData(data.Value);
                Console.WriteLine("Measure Data {0} ({1:T})",
                    transferData.Value,
                    transferData.SendDate);
                _calcSelection.Tell(transferData);
            });
            Receive<MeasureData>(data =>
            {
                Console.WriteLine("Resend Measure Data {0} ({1:T})",
                    data.Value,
                    data.SendDate);
                _calcSelection.Tell(data);
            });
            Receive<SendSuccess>(data =>
            {
                Console.WriteLine("Success");
                _unreceivedMsgActor.Tell(data);
            });
        }
    }
}
