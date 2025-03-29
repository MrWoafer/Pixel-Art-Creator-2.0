using System.Collections.Generic;

using PAC.Extensions;
using PAC.Geometry;
using PAC.Geometry.Extensions;

using UnityEngine;

namespace PAC.ImageEditing
{
    /// <summary>
    /// Handles flood-filling pixel art images.
    /// </summary>
    public static class FloodFill
    {
        /// <summary>
        /// Returns the largest connected (in terms of being adjacent) set containing <paramref name="startPoint"/> where all pixels have the same colour.
        /// </summary>
        /// <param name="includeDiagonallyAdjacent">Whether to flood-fill diagonally-adjacent pixels (as well as up/down/left/right-adjacent).</param>
        /// <param name="maxNumOfIterations">After this many pixels have been enumerated, the method will stop. Useful to prevent huge frame drops when filling large areas.</param>
        public static IEnumerable<IntVector2> GetPixelsToFill(Texture2D texture, IntVector2 startPoint, bool includeDiagonallyAdjacent, int maxNumOfIterations = 1_000_000)
            => GetPixelsToFill(
                texture,
                startPoint,
                includeDiagonallyAdjacent ? Direction8.All : Direction8.UpDownLeftRight,
                maxNumOfIterations
                );
        private static IEnumerable<IntVector2> GetPixelsToFill(Texture2D texture, IntVector2 startPoint, IEnumerable<Direction8> adjacentDirections, int maxNumOfIterations)
        {
            Color colourToReplace = texture.GetPixel(startPoint);

            Queue<IntVector2> toVisit = new Queue<IntVector2>();
            HashSet<IntVector2> visited = new HashSet<IntVector2>();

            toVisit.Enqueue(startPoint);
            visited.Add(startPoint);
            yield return startPoint;

            int iterations = 0;
            while (toVisit.Count > 0 && iterations < maxNumOfIterations)
            {
                IntVector2 coord = toVisit.Dequeue();

                foreach (Direction8 offset in adjacentDirections)
                {
                    IntVector2 adjacentCoord = coord + offset;
                    if (!visited.Contains(adjacentCoord) && texture.ContainsPixel(adjacentCoord) && texture.GetPixel(adjacentCoord) == colourToReplace)
                    {
                        toVisit.Enqueue(adjacentCoord);
                        visited.Add(adjacentCoord);
                        yield return adjacentCoord;
                    }
                }

                iterations++;
            }
        }
    }
}