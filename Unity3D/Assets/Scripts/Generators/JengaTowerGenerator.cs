using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    class JengaTowerGenerator
    {
        public IEnumerable<JengaBlock> GenerateTower(Vector3 startPosition, int maxHeight, GameObject prefab)
        {
            var blocks = new List<JengaBlock>();
            var widthRatio = prefab.transform.localScale.z / prefab.transform.localScale.x;
            var horizontalSpacing = prefab.transform.localScale.x + 0.1f;
            var verticalSpacing = prefab.transform.localScale.y + 0.1f;

            for (var currentHeight = 0; currentHeight < maxHeight; currentHeight++)
            {
                for (var currentWidth = 0; currentWidth < widthRatio; currentWidth++)
                {
                    var rotation = currentHeight % 2 == 0 ? 90 : 0;
                    if (rotation == 90)
                    {
                        var newPosition = startPosition + new Vector3(horizontalSpacing, verticalSpacing * currentHeight, horizontalSpacing * (currentWidth - 1));
                        blocks.Add(GetJengaBlock(prefab, newPosition, rotation));
                    }
                    if (rotation == 0)
                    {
                        var newPosition = startPosition + new Vector3(horizontalSpacing * currentWidth, verticalSpacing * currentHeight, 0);
                        blocks.Add(GetJengaBlock(prefab, newPosition, rotation));
                    }
                }
            }
            return blocks;
        }

        private JengaBlock GetJengaBlock(GameObject prefab, Vector3 newPosition, int rotation)
        {
            return new JengaBlock(prefab, newPosition, Quaternion.Euler(0, rotation, 0));
        }
    }

    public struct JengaBlock
    {
        public GameObject Prefab;
        public Vector3 Position;
        public Quaternion Rotation;

        public JengaBlock(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            Prefab = prefab;
            Position = position;
            Rotation = rotation;
        }
    }
}
