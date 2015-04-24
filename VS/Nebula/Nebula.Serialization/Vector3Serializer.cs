using System;
using System.Globalization;
using System.Linq;
using Nebula.Connectivity;
using UnityEngine;

namespace Nebula.Serialization
{
    public class Vector3Serializer : IPacketConverter<Vector3>
    {
        private const string Vector3SerializationFormat = "Vector3({0},{1},{2})";

        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        public string Serialize(Vector3 arg)
        {
            return string.Format(Vector3SerializationFormat,
                arg.x.ToString(_cultureInfo),
                arg.y.ToString(_cultureInfo),
                arg.z.ToString(_cultureInfo));
        }

        public Vector3 Deserialize(string packet)
        {
            if (!HeaderIsSupported(packet))
                throw new FormatException();

            var valuesWithBraces = RemoveVectorHeader(packet);
            var rawValues = valuesWithBraces
                .Replace("(", "")
                .Replace(")","");
            var splittedValues = rawValues.Split(',')
                .Select(s => float.Parse(s, _cultureInfo))
                .ToArray();

            return new Vector3(splittedValues[0], splittedValues[1], splittedValues[2]);
        }

        private bool HeaderIsSupported(string packet)
        {
            return packet.Contains("Vector3");
        }

        private string RemoveVectorHeader(string packet)
        {
            return packet.Remove(0, "Vector3".Length);
        }
    }
}