using System;
using Akka.Actor;
using RemoteMeasure.Common.Messages;

namespace RemoteMeasure.CalculationService.Actors
{
    public class MeasureReceiveActor : ReceiveActor
    {
        private IActorRef _calcActor;

        public MeasureReceiveActor(IActorRef calcActor)
        {
            _calcActor = calcActor;
            Ready();
        }

        private void Ready()
        {
            Receive<MeasureData>(data =>
            {
                Console.WriteLine("Received Measure Data is {0}", data.Value);
                Sender.Tell(new SendSuccess());
                _calcActor.Tell(data);
            });
        }
    }
}
