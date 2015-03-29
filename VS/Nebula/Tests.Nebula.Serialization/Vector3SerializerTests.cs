using System;
using Nebula.Serialization;
using NFluent;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Nebula.Serialization
{
    [TestFixture]
    public class Vector3SerializerTests
    {
        private Vector3Serializer _serializer;

        [SetUp]
        public void Setup()
        {
            _serializer = new Vector3Serializer();
        }

        [Test]
        public void EmptyVectorIsCorrectlySerialized()
        {
            var serializedVector = _serializer.Serialize(new Vector3(0, 0, 0));

            Check.That(serializedVector).IsEqualTo("Vector3(0,0,0)");
        }

        [Test]
        public void NonEmptyVectorIsCorrectlySerialized()
        {
            var serializedVector = _serializer.Serialize(new Vector3(1.25f, 2.43f, -10.22f));

            Check.That(serializedVector).IsEqualTo("Vector3(1.25,2.43,-10.22)");
        }

        [Test]
        public void SmallVectorIsRoundedAndCorrectlySerialized()
        {
            var serializedVector = _serializer.Serialize(new Vector3(0.0000025f, 1.245566643f, -102323.0000022f));

            Check.That(serializedVector).IsEqualTo("Vector3(2.5E-06,1.245567,-102323)");
        }

        [Test]
        public void WhenStringHaveInccorectHeaderExceptionIsThrown()
        {
            Check.ThatCode(() => _serializer.Deserialize("Wrong(0,0,0)"))
                .Throws<FormatException>();
        }

        [Test]
        public void WhenStringHaveInccorectValuesExceptionIsThrown()
        {
            Check.ThatCode(() => _serializer.Deserialize("Vector3()"))
                .Throws<FormatException>();
            Check.ThatCode(() => _serializer.Deserialize("Vector3(a,b,c)"))
                .Throws<FormatException>();
            Check.ThatCode(() => _serializer.Deserialize("Vector3(1,2,c)"))
                .Throws<FormatException>();
        }

        [Test]
        public void EmptyVectorIsCorrectlyDeserialized()
        {
            var deserializedVector = _serializer.Deserialize("Vector3(0,0,0)");

            Check.That(deserializedVector).IsEqualTo(new Vector3());
        }

        [Test]
        public void NonEmptyVectorIsCorrectlyDeserialized()
        {
            var deserializedVector = _serializer.Deserialize("Vector3(1.25,2.43,-10.22)");

            Check.That(deserializedVector).IsEqualTo(new Vector3(1.25f, 2.43f, -10.22f));
        }

        [Test]
        public void SmallVectorIsCorrectlyDeserialized()
        {
            var deserializedVector = _serializer.Deserialize("Vector3(2.5E-06,1.245566643,-102323.0000022)");

            Check.That(deserializedVector).IsEqualTo(new Vector3(0.0000025f, 1.245566643f, -102323.0000022f));
        }
    }
}
