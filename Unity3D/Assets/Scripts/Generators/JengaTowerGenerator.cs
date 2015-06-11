using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Generators
{
    class JengaTowerGenerator
    {
        public void GenerateTower(Vector3 startPosition, int maxHeight, GameObject prefab)
        {
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
                        Object.Instantiate(prefab, newPosition, Quaternion.Euler(0, rotation, 0));
                    }
                    if (rotation == 0)
                    {
                        var newPosition = startPosition + new Vector3(horizontalSpacing * currentWidth, verticalSpacing * currentHeight, 0);
                        Object.Instantiate(prefab, newPosition, Quaternion.Euler(0, rotation, 0));
                    }
                }
            }
        }
    }
}
