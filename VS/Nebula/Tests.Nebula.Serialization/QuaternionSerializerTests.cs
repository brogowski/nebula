using System;
using System.Collections.Generic;
using System.Text;
using Nebula.Serialization;
using NFluent;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Nebula.Serialization
{
    [TestFixture]
    public class QuaternionSerializerTests
    {
        private QuaternionSerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = new QuaternionSerializer();
        }

        [Test]
        public void EmptyQuaternionIsCorrectlySerialized()
        {
            var serializedQuaternion = _serializer.Serialize(new Quaternion(0, 0, 0, 0));

            Check.That(serializedQuaternion).IsEqualTo("Quaternion(0,0,0,0)");
        }

        [Test]
        public void NonEmptyQuaternionIsCorrectlySerialized()
        {
            var serializedQuaternion = _serializer.Serialize(new Quaternion(1.25f, 2.43f, -10.22f, 54f));

            Check.That(serializedQuaternion).IsEqualTo("Quaternion(1.25,2.43,-10.22,54)");
        }

        [Test]
        public void SmallQuaternionIsRoundedAndCorrectlySerialized()
        {
            var serializedQuaternion = _serializer.Serialize(new Quaternion(0.0000025f, 1.245566643f, -102323.0000022f, -1.00200023f));

            Check.That(serializedQuaternion).IsEqualTo("Quaternion(2.5E-06,1.245567,-102323,-1.002)");
        }

        [Test]
        public void WhenStringHaveInccorectHeaderExceptionIsThrown()
        {
            Check.ThatCode(() => _serializer.Deserialize("Wrong(0,0,0,0)"))
                .Throws<FormatException>();
        }

        [Test]
        public void WhenStringHaveInccorectValuesExceptionIsThrown()
        {
            Check.ThatCode(() => _serializer.Deserialize("Quaternion()"))
                .Throws<FormatException>();
            Check.ThatCode(() => _serializer.Deserialize("Quaternion(a,b,c,d)"))
                .Throws<FormatException>();
            Check.ThatCode(() => _serializer.Deserialize("Quaternion(1,2,c,d)"))
                .Throws<FormatException>();
        }

        [Test]
        public void EmptyQuaternionIsCorrectlyDeserialized()
        {
            var deserializedQuaternion = _serializer.Deserialize("Quaternion(0,0,0,0)");

            Check.That(deserializedQuaternion).IsEqualTo(new Quaternion());
        }

        [Test]
        public void NonEmptyQuaternionIsCorrectlyDeserialized()
        {
            var deserializedQuaternion = _serializer.Deserialize("Quaternion(1.25,2.43,-10.22,-1.002)");

            Check.That(deserializedQuaternion).IsEqualTo(new Quaternion(1.25f, 2.43f, -10.22f, -1.002f));
        }

        [Test]
        public void SmallQuaternionIsCorrectlyDeserialized()
        {
            var deserializedQuaternion = _serializer.Deserialize("Quaternion(2.5E-06,1.245566643,-102323.0000022,-1.00200023)");

            Check.That(deserializedQuaternion).IsEqualTo(new Quaternion(0.0000025f, 1.245566643f, -102323.0000022f, -1.00200023f));
        }
    }
}
