using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using RemoteMeasure.Common.Messages;

namespace RemoteMeasure.CalculationService.Actors
{
    public class CalculationActor : ReceiveActor
    {
        private IList<int> _valueList = new List<int>();

        public CalculationActor()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<MeasureData>(data =>
            {
                _valueList.Add(data.Value);
                Console.WriteLine(
                    "Average value is {0:0.##} from total {1} values",
                    _valueList.Average(x => x),
                    _valueList.Count);
            });
        }
    }
}
