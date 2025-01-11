using PAC.DataStructures;

using UnityEngine;

namespace PAC.Shapes
{
    /// <summary>
    /// <para>
    /// These methods could be defined as default implementations in IShape, but are defined as extension methods for one (or both) of the following reasons:
    /// </para>
    /// <para>
    /// 1)
    ///     They are not directly related to the purpose of the IShape, which is just to represent a shape. E.g. ToTexture() is not directly related to this.
    /// </para>
    /// <para>
    /// 2)  
    ///     That would require casting to IShape to use them. Making them as extension methods avoids needing this cast. We could turn IShape into an abstract class to avoid this default
    ///     implementation casting issue, but then to have a more specific return type in methods like Translate() (e.g. Line.Translate() returns a Line instead of just IShape) we need covariant
    ///     return types, which isn't yet supported in Unity's compiler.
    /// </para>
    /// </summary>
    public static class IShapeExtensions
    {
        /// <summary>
        /// Turns the pixels in the shape's bounding rect into a Texture2D.
        /// </summary>
        public static Texture2D ToTexture(this IShape shape, Color colour) => shape.ToTexture(colour, shape.boundingRect);
        /// <summary>
        /// Turns the pixels in the given IntRect into a Texture2D, using any of the shape's pixels that lie within that rect. 
        /// </summary>
        public static Texture2D ToTexture(this IShape shape, Color colour, IntRect texRect)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texRect.width, texRect.height);

            foreach (IntVector2 pixel in shape)
            {
                if (texRect.Contains(pixel))
                {
                    tex.SetPixel(pixel - texRect.bottomLeft, colour);
                }
            }

            tex.Apply();
            return tex;
        }
    }
}