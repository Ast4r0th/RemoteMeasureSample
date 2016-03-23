using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteMeasure.Common.Messages
{
    public class MeasureData
    {
        public int Value { get; private set; }

        public MeasureData(int value)
        {
            Value = value;
        }
    }
}
