using System;
using System.Configuration;
using Akka.Actor;
using RemoteMeasure.Common.Messages;

namespace RemoteMeasure.MeasureService.Actors
{
    public class SendActor : ReceiveActor
    {
        private ActorSelection _calcSelection;

        public SendActor()
        {
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
            Receive<MeasureData>(data =>
            {
                Console.WriteLine("Measure Data is {0}", data.Value);
                _calcSelection.Tell(data);
            });
        }
    }
}
