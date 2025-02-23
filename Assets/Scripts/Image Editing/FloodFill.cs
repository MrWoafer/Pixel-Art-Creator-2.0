using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Extensions;

using UnityEngine;

namespace PAC.ImageEditing
{
    /// <summary>
    /// Handles flood-filling pixel art images.
    /// </summary>
    public static class FloodFill
    {
        /// <summary>
        /// Returns the largest connected (in terms of being adjacent (left/right/up/down)) set containing <paramref name="startPoint"/> where all pixels have the same colour.
        /// </summary>
        /// <param name="maxNumOfIterations">After this many pixels have been enumerated, the method will stop. Useful to prevent huge frame drops when filling large areas.</param>
        public static IEnumerable<IntVector2> GetFloodFillPixels(Texture2D texture, IntVector2 startPoint, int maxNumOfIterations = 1_000_000)
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

                foreach (IntVector2 offset in IntVector2.upDownLeftRight)
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