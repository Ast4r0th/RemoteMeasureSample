namespace RemoteMeasure.MeasureService.Messages
{
    internal class InternalMeasureData
    {
        public int Value { get; private set; }

        public InternalMeasureData(int value)
        {
            Value = value;
        }
    }
}
