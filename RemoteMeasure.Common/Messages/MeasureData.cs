using System;

namespace RemoteMeasure.Common.Messages
{
    public class MeasureData
    {
        public MeasureData(int value)
        {
            Value = value;
            SendDate = DateTime.Now;
        }

        public int Value { get; private set; }

        public DateTime SendDate { get; private set; }
    }
}
