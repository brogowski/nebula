using System.Collections.Generic;
using System.Linq;

namespace Nebula.TimedList
{
    public class TimedList<T>
    {
        private readonly IList<TimedElement<T>> _elements;

        public TimedList()
        {
            _elements = new List<TimedElement<T>>();
        }

        public float CumulativeDuration { get; private set; }
        public int Count { get; private set; }

        public void Add(T value, float duration)
        {
            if(duration <= 0f)
                return;

            AddElement(value, duration);

            Count++;
            CumulativeDuration += duration;
        }
        public IEnumerable<TimedElement<T>> Take(float duration)
        {
            if (duration <= 0f)
                return Enumerable.Empty<TimedElement<T>>();
            
            var toReturn = GetElements(duration);

            UpdateCounters();

            return toReturn;
        }

        private void AddElement(T value, float duration)
        {
            _elements.Add(new TimedElement<T>(value,duration));
        }
        private IEnumerable<TimedElement<T>> GetElements(float duration)
        {
            int amount = 0;
            while (duration > 0f && amount < _elements.Count)
            {
                duration -= _elements[amount].Duration;
                amount++;
            }

            var toReturn = _elements.Take(amount).ToArray();

            if (LastElementMustBeModified(duration))
            {
                var indexToModify = toReturn.Length - 1;
                toReturn[indexToModify].Duration += duration;
                _elements[indexToModify] = new TimedElement<T>(
                    _elements[indexToModify].Value, -duration);
                amount--;
            }

            for (int i = 0; i < amount; i++)
                _elements.RemoveAt(0);

            return toReturn;
        }
        private bool LastElementMustBeModified(float duration)
        {
            return duration < 0f;
        }
        private void UpdateCounters()
        {
            CumulativeDuration = _elements.Sum(q => q.Duration);
            Count = _elements.Count;
        }
    }
}