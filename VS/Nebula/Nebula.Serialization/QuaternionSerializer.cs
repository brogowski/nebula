using System;
using System.Globalization;
using System.Linq;
using Nebula.Connectivity;
using UnityEngine;

namespace Nebula.Serialization
{
    public class QuaternionSerializer : IPacketSerializer<Quaternion>, IPacketDeserializer<Quaternion>
    {
        private const string QuaternionSerializationFormat = "Quaternion({0},{1},{2},{3})";

        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        public string Serialize(Quaternion arg)
        {
            return string.Format(QuaternionSerializationFormat,
                arg.x.ToString(_cultureInfo),
                arg.y.ToString(_cultureInfo),
                arg.z.ToString(_cultureInfo),
                arg.w.ToString(_cultureInfo));
        }

        public Quaternion Deserialize(string packet)
        {
            if (!HeaderIsSupported(packet))
                throw new FormatException();

            var valuesWithBraces = RemoveVectorHeader(packet);
            var rawValues = valuesWithBraces
                .Replace("(", "")
                .Replace(")", "");
            var splittedValues = rawValues.Split(',')
                .Select(s => float.Parse(s, _cultureInfo))
                .ToArray();

            return new Quaternion(splittedValues[0], splittedValues[1], splittedValues[2], splittedValues[3]);
        }

        private bool HeaderIsSupported(string packet)
        {
            return packet.Contains("Quaternion");
        }

        private string RemoveVectorHeader(string packet)
        {
            return packet.Remove(0, "Quaternion".Length);
        }
    }
}