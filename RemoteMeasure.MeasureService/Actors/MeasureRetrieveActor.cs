using System;
using Akka.Actor;
using RemoteMeasure.MeasureService.Messages;

namespace RemoteMeasure.MeasureService.Actors
{
    public class MeasureRetrieveActor : ReceiveActor
    {
        private IActorRef _sendActor;

        public MeasureRetrieveActor(IActorRef sendActor)
        {
            _sendActor = sendActor;
            Ready();
        }

        private void Ready()
        {
            var random = new Random(100);
            Receive<StartMonitoring>(start =>
            {
                Context.System.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(5),
                    Self,
                    new MeasureCheck(),
                    Self);
            });
            Receive<MeasureCheck>(start =>
            {
                var data = new InternalMeasureData(random.Next(-50, 50));
                _sendActor.Tell(data);
            });
        }
    }
}
