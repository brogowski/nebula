using System;
using System.Linq;
using Nebula.TimedList;
using NFluent;
using NUnit.Framework;

namespace Tests.Nebula.TimedList
{
    [TestFixture]
    public class TimedListTests
    {
        private const float Duration = 0.1f;
        private const string Value = "value";

        private TimedList<string> _timedList;
        private Func<string, float, string[]> _splitFunc; 

        [SetUp]
        public void SetUp()
        {
            _splitFunc = (input, duration) =>
            {
                var trimAmount = (int)(input.Length * duration);
                return new[] {input.Substring(0, trimAmount), input.Substring(trimAmount, input.Length - trimAmount)};
            };
            _timedList = new TimedList<string>(_splitFunc);
        }
        private void AddElement(string value = Value, float duration = Duration)
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

            _timedList.Take(Duration);

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

            Check.That(_timedList.Take(Duration)).ContainsExactly(new TimedElement<string>(Value, Duration));
        }
        [Test]
        public void AfterAddingElement_CumulativeDurationIsUpdated()
        {
            AddElement();

            Check.That(_timedList.CumulativeDuration).IsEqualTo(Duration);
        }
        [Test]
        public void AfterTakingElement_CumulativeDurationIsUpdated()
        {
            AddElement();
            AddElement();

            _timedList.Take(Duration);

            Check.That(_timedList.CumulativeDuration).IsEqualTo(Duration);
        }
        [Test]
        public void TakeWithDurationBiggerThenCumulativeDuration_ReturnsAllElements()
        {
            AddElement();
            AddElement();

            Check.That(_timedList.Take(Duration*5)).HasSize(2);
        }
        [Test]
        public void AfterTakingElement_ElementIsRemovedFromList()
        {
            AddElement();

            _timedList.Take(Duration);

            Check.That(_timedList.Take(Duration)).IsEmpty();
        }
        [Test]
        public void TakingWithDuration3WithElements2_2_ReturnsElementsWithModifiedDuration()
        {
            AddElement(duration: 2);
            AddElement(duration: 2);

            Check.That(_timedList.Take(3f))
                .ContainsExactly(
                new TimedElement<string>(Value, 2f),
                new TimedElement<string>(Value.Substring(0,2), 1f));
        }

        [Test]
        public void AfterTakeWhichAlteredDuration_DurationRemainsAltered()
        {
            AddElement(duration: 2);
            AddElement(duration: 2);

            _timedList.Take(3f);

            var leftElement = _timedList.Take(2f).Single();
            Check.That(leftElement.Duration).IsEqualTo(1f);
        }

        [Test]
        public void AfterTakeWhichAlteredDuration_SplitIsCorrectlyInvoked()
        {
            AddElement(duration: 2);

            var result = _timedList.Take(1f);

            Check.That(result).ContainsExactly(new TimedElement<string>(Value.Substring(0, 2), 1f));
            Check.That(_timedList.Take(1f)).ContainsExactly(new TimedElement<string>(Value.Substring(2, 3), 1f));
        }

        [Test]
        public void SmallFloatTest()
        {
            AddElement(duration: 7);
            const float duration = 6.999999f;

            var firstElement = _timedList.Take(duration).Single();
            var secondElement = _timedList.Take(duration).Single();
            Check.That(firstElement.Duration).IsEqualTo(duration);
            Check.That(secondElement.Duration).IsEqualTo(7 - duration);
        }
    }
}
