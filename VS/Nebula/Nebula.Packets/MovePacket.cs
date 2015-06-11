using System;
using UnityEngine;

namespace Nebula.Packets
{
    public struct MovePacket : IPacket
    {
        public Guid Id;
        public Vector3 Move;
        public Quaternion Rotation;
        public float Duration;

        public MovePacket(Guid id, Vector3 move, Quaternion rotation, float duration)
        {
            Id = id;
            Move = move;
            Rotation = rotation;
            Duration = duration;
        }
    }
}