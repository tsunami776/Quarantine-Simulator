using System.Collections.Generic;
using UnityEngine;

namespace Assets.Backpack.Extensions
{
    public class CubemapEdgePoints
    {
        public CubemapFace CubemapFace { get; }
        public Vector3 TopLeft { get; }
        public Vector3 TopRight { get; }
        public Vector3 BottomLeft { get; }
        public Vector3 BottomRight { get; }
        public Bounds Bounds { get; }

        public CubemapEdgePoints(CubemapFace cubemapFace, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, Bounds bounds)
        {
            CubemapFace = cubemapFace;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Bounds = bounds;
        }
    }

    public static class BoundsExtensions
    {
        private static readonly Dictionary<CubemapFace, int[][]> AxisToCubeEdgePointMap = new Dictionary<CubemapFace, int[][]>()
        {
            [CubemapFace.NegativeZ] = new int[][]
            {
                new int[3] {-1, 1, -1},
                new int[3] {1, 1, -1},
                new int[3] {-1, -1, -1},
                new int[3] {1, -1, -1},
            },
            [CubemapFace.PositiveZ] = new int[][]
            {
                new int[3] {1, 1, 1},
                new int[3] {-1, 1, 1},
                new int[3] {1, -1, 1},
                new int[3] {-1, -1, 1},
            },
            [CubemapFace.NegativeX] = new int[][]
            {
                new int[3] {-1, 1, 1},
                new int[3] {-1, 1, -1},
                new int[3] {-1, -1, 1},
                new int[3] {-1, -1, -1},
            },
            [CubemapFace.PositiveX] = new int[][]
            {
                new int[3] {1, 1, -1},
                new int[3] {1, 1, 1},
                new int[3] {1, -1, -1},
                new int[3] {1, -1, 1},
            },
            [CubemapFace.PositiveY] = new int[][]
            {
                new int[3] {-1, 1, 1},
                new int[3] {1, 1, 1},
                new int[3] {-1, 1, -1},
                new int[3] {1, 1, -1},
            },
            [CubemapFace.NegativeY] = new int[][]
            {
                new int[3] {-1, -1, -1},
                new int[3] {1, -1, -1},
                new int[3] {-1, -1, 1},
                new int[3] {1, -1, 1},
            }
        };

        public static CubemapEdgePoints GetCubemapEdgePoints(this Bounds bounds, CubemapFace face)
        {
            var points = GetHolsterFaceCorners(bounds, face);
            return new CubemapEdgePoints(
                face,
                points[0],
                points[1],
                points[2],
                points[3],
                bounds
            );
        }
        
        private static Vector3[] GetHolsterFaceCorners(Bounds bounds, CubemapFace face)
        {
            int[][] pointMap = AxisToCubeEdgePointMap[face];
            var points = new Vector3[4];
            for (var i = 0; i < pointMap.Length; i++)
            {
                points[i] = bounds.center + new Vector3(
                                pointMap[i][0] * bounds.extents.x,
                                pointMap[i][1] * bounds.extents.y,
                                pointMap[i][2] * bounds.extents.z);
            }

            return points;
        }

        public static Vector3 GetScaleToFitWithinBound(this Bounds target, Bounds fitToBounds)
        {
            var targetScale = fitToBounds.size.magnitude / target.size.magnitude;
            var targetScaleVector = new Vector3(targetScale, targetScale, targetScale);
            return targetScaleVector;
        }
    }
}
