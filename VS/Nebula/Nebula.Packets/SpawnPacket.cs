using System;
using UnityEngine;

namespace Nebula.Packets
{
    public struct SpawnPacket
    {
        public Guid Id;
        public Vector3 Position;
        public Quaternion Rotation;
        public string Type;

        public SpawnPacket(Guid id, Vector3 position, Quaternion rotation, string type)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Type = type;
        }
    }
}
