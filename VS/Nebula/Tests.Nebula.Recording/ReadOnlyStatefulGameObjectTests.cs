using Nebula.Recording;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Nebula.Recording
{
    class ReadOnlyStatefulGameObjectTests
    {
        private ReadOnlyStatefulGameObject _readOnlyStatefulGameObject;
        private IGameObject _gameObject;

        [SetUp]
        public void SetUp()
        {
            _gameObject = Substitute.For<IGameObject>();
            _gameObject.Position = new Vector3(1, 1, 1);
            _readOnlyStatefulGameObject = new ReadOnlyStatefulGameObject(_gameObject);
        }

        [Test]
        public void GetPositionDiff_WhenNoChangesWereIssued_ReturnsEmptyPosition()
        {
            var result = _readOnlyStatefulGameObject.GetPositionDiff();
            Check.That(result).IsEqualTo(Vector3.zero);
        }

        [Test]
        public void GetPositionDiff_AfterChange_CorrectDiffIsReturned()
        {            
            _gameObject.Position = new Vector3(2, 2, 2);
            var result = _readOnlyStatefulGameObject.GetPositionDiff();
            Check.That(result).IsEqualTo(new Vector3(1, 1, 1));
        }
    }
}
