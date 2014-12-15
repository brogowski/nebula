using NFluent;
using NUnit.Framework;
using Nebula.TimedList;

namespace Tests.Nebula.TimedList
{
    [TestFixture]
    public class TimedListTests
    {
        private TimedList<string> _timedList;
        private const float _duration = 0.1f;
        private const string _value = "val";

        [SetUp]
        public void SetUp()
        {
            _timedList = new TimedList<string>();
        }
        private void AddElement(string value = _value, float duration = _duration)
        {
            _timedList.Add(value, duration);
        }

        [Test]
        public void AfterAddingElement_CountIsUpdated()
        {
            AddElement();

            Check.That(_timedList.Count).IsEqualTo(1);
        }
        [Test]
        public void AfterTakingElement_CountIsUpdated()
        {
            AddElement();
            AddElement();

            _timedList.Take(_duration);

            Check.That(_timedList.Count).IsEqualTo(1);
        }
        [Test]
        public void AddingElementWithDuration0_IsIgnored()
        {
            AddElement(duration: 0);

            Check.That(_timedList.Count).IsEqualTo(0);
        }
        [Test]
        public void TakeWithDuration0_ReturnsEmptyList()
        {
            AddElement();

            Check.That(_timedList.Take(0f)).IsEmpty();
        }
        [Test]
        public void TakeWithDurationEqualToFirstElement_ReturnsFirstElement()
        {
            AddElement();

            Check.That(_timedList.Take(_duration)).ContainsExactly(new TimedElement<string>(_value, _duration));
        }
        [Test]
        public void AfterAddingElement_CumulativeDurationIsUpdated()
        {
            AddElement();

            Check.That(_timedList.CumulativeDuration).IsEqualTo(_duration);
        }
        [Test]
        public void AfterTakingElement_CumulativeDurationIsUpdated()
        {
            AddElement();
            AddElement();

            _timedList.Take(_duration);

            Check.That(_timedList.CumulativeDuration).IsEqualTo(_duration);
        }
        [Test]
        public void TakeWithDurationBiggerThenCumulativeDuration_ReturnsAllElements()
        {
            AddElement();
            AddElement();

            Check.That(_timedList.Take(_duration*5)).HasSize(2);
        }
        [Test]
        public void AfterTakingElement_ElementIsRemovedFromList()
        {
            AddElement();

            _timedList.Take(_duration);

            Check.That(_timedList.Take(_duration)).IsEmpty();
        }
        [Test]
        public void TakingWithDuration3WithElements2_2_ReturnsElementsWithModifiedDuration()
        {
            AddElement(duration: 2);
            AddElement(duration: 2);

            Check.That(_timedList.Take(3f))
                .ContainsExactly(new TimedElement<string>(_value, 2f), new TimedElement<string>(_value, 1f));
        }
        [Test]
        public void AfterTakeWhichAlteredDuration_DurationRemainsAltered()
        {
            AddElement(duration: 2);
            AddElement(duration: 2);

            _timedList.Take(3f);

            Check.That(_timedList.Take(2f))
                .ContainsExactly(new TimedElement<string>(_value, 1f));
        }
        [Test]
        public void SmallFloatTest()
        {
            AddElement(duration: 7);
            const float duration = 6.999999f;

            Check.That(_timedList.Take(duration))
                .ContainsExactly(new TimedElement<string>(_value, duration));
            Check.That(_timedList.Take(7 - duration))
                .ContainsExactly(new TimedElement<string>(_value, 7 - duration));
        }
    }
}
