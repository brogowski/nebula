using System.IO;
using Nebula.Serialization;

namespace Assets.Scripts.Recording
{
    class ToFilePhysicsRecorder : PhysicsRecorder
    {
        private readonly Vector3Serializer _vectorSerializer;
        private readonly QuaternionSerializer _quaternionSerializer;
        private readonly TextWriter _vectorFileStream;
        private readonly TextWriter _quaternionFileStream;

        public ToFilePhysicsRecorder()
        {
            _vectorSerializer = new Vector3Serializer();
            _quaternionSerializer = new QuaternionSerializer();
            _vectorFileStream = new StreamWriter(File.Open("debug_scene_02_vector.txt", FileMode.Create));
            _quaternionFileStream = new StreamWriter(File.Open("debug_scene_02_quaternion.txt", FileMode.Create));
        }

        public override void RecordPhysics(float time)
        {
            _vectorFileStream.WriteLine(_vectorSerializer.Serialize(GameObjects[0].GetPositionDiff()));
            _quaternionFileStream.WriteLine(_quaternionSerializer.Serialize(GameObjects[0].GetRotationDiff()));
            base.RecordPhysics(time);
        }

        public override void Dispose()
        {
            _vectorFileStream.Dispose();
            _quaternionFileStream.Dispose();
            base.Dispose();
        }
    }
}
