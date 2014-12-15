namespace Nebula.TimedList
{
    public struct TimedElement<T>
    {
        public TimedElement(T value, float duration)
        {
            Value = value;
            Duration = duration;
        }
        public T Value;
        public float Duration;

        public override string ToString()
        {
            return string.Format("Duration: {0}, Val: {1}", Duration, Value);
        }
    }
}