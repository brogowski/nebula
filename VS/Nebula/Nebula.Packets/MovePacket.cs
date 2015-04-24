using System;
using UnityEngine;

namespace Nebula.Packets
{
    public struct MovePacket
    {
        public Guid Id;
        public Vector3 Move;
        public Quaternion Rotation;

        public MovePacket(Guid id, Vector3 move, Quaternion rotation)
        {
            Id = id;
            Move = move;
            Rotation = rotation;
        }
    }
}