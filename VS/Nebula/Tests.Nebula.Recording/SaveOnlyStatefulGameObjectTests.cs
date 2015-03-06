using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Recording;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Nebula.Recording
{
    class SaveOnlyStatefulGameObjectTests
    {
        private SaveOnlyStatefulGameObject _saveOnlyStatefulGameObject;
        private IGameObject _gameObject;

        [SetUp]
        public void SetUp()
        {
            _gameObject = Substitute.For<IGameObject>();
            _saveOnlyStatefulGameObject = new SaveOnlyStatefulGameObject(_gameObject);
        }


        [Test]
        public void ApplyPositionDiff_WorksCorrectly()
        {
            _gameObject.Position = new Vector3(1, 1, 1);
            _saveOnlyStatefulGameObject.ApplyPositionDiff(new Vector3(1, 1, 1));

            Check.That(_gameObject.Position).Equals(new Vector3(2, 2, 2));
        }

        [Test]
        public void ApplyRotationDiff_WorksCorrectly()
        {
            _gameObject.Rotation = new Quaternion(2, 2, 2, 2);
            _saveOnlyStatefulGameObject.ApplyRotationDiff(new Quaternion(3, 3, 3, 3));

            Check.That(_gameObject.Rotation).Equals(new Quaternion(12, 12, 12, -12));
        }
    }
}
