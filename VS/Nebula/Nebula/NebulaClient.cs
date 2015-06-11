using System.Collections.Generic;
using Nebula.Connectivity;
using Nebula.Input;
using Nebula.Packets;

namespace Nebula
{
    public class NebulaClient
    {
        private readonly AbstractSender<RecordedInput> _inputSender;
        private readonly AbstractReciver<IPacket> _packetReciver;
        private readonly InputRecorder _inputRecorder;

        public NebulaClient(AbstractSender<RecordedInput> inputSender, AbstractReciver<IPacket> packetReciver, InputRecorder inputRecorder)
        {
            _inputSender = inputSender;
            _packetReciver = packetReciver;
            _inputRecorder = inputRecorder;
        }

        public void RecordInput(RecordedInput recording)
        {
            _inputRecorder.RecordInput(recording);
        }

        public void UpdateRemoteInputs(float time)
        {
            var inputs = _inputRecorder.GetInputForDuration(time);
            foreach (var recordedInput in inputs)
            {                
                _inputSender.Send(recordedInput);
            }
        }

        public IEnumerable<IPacket> GetPhysicsPackets()
        {
            return _packetReciver.Recive();
        }
    }
}
