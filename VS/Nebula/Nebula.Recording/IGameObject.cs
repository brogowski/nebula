using UnityEngine;

namespace Nebula.Recording
{
    public interface IGameObject
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }
}