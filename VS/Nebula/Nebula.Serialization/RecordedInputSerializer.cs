using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nebula.Connectivity;
using Nebula.Input;

namespace Nebula.Serialization
{
    public class RecordedInputSerializer : IPacketConverter<RecordedInput>
    {
        private const string HeaderFormat = "RecordedInput({0})";
        private const string InputDataFormat = "{0}:{1}";
        private readonly string _separator = Environment.NewLine;
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        public string Serialize(RecordedInput arg)
        {
            var header = string.Format(HeaderFormat, SerializeDouble(arg.Duration));
            if (arg.Data == null || !arg.Data.Any())
                return header;
            var body = arg.Data.Select(FormatInputData).Aggregate(JoinWithSeparator);
            return header + _separator + body;
        }

        private string FormatInputData(RecordedInput.InputData q)
        {
            return string.Format(InputDataFormat, q.Name, SerializeDouble(q.Value));
        }

        private string JoinWithSeparator(string s, string s1)
        {
            return string.Join(_separator, new[] {s, s1});
        }

        private string SerializeDouble(double arg)
        {
            return arg.ToString(_culture);
        }

        public RecordedInput Deserialize(string packet)
        {
            var data = packet.Split(new []{ _separator }, StringSplitOptions.RemoveEmptyEntries);
            var duration = GetDuration(data[0]);
            var inputData = GetInputData(data.Skip(1));
            var deserialized = new RecordedInput(inputData, duration);            
            return deserialized;
        }

        private RecordedInput.InputData[] GetInputData(IEnumerable<string> data)
        {
            return data.Select(ParseInputData).ToArray();
        }

        private RecordedInput.InputData ParseInputData(string arg)
        {
            var split = arg.Split(':');
            return new RecordedInput.InputData(split[0], float.Parse(split[1], _culture));
        }

        private float GetDuration(string packet)
        {
            var headless = RemoveHeader(packet);
            var braceless = RemoveBraces(headless);
            return float.Parse(braceless, _culture);
        }

        private string RemoveBraces(string headless)
        {
            return headless.Replace("(", "").Replace(")", "");
        }

        private string RemoveHeader(string packet)
        {
            return packet.Remove(0, "RecordedInput".Length);
        }
    }
}