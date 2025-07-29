using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.Geometry;
using PAC.Tools.Interfaces;
using PAC.Geometry.Extensions;

using UnityEngine;
using PAC.Extensions.UnityEngine;

namespace PAC.Tools
{
    /// <summary>
    /// The pixels, given relative to the position of the mouse, that will be affected by the current brush.
    /// </summary>
    public abstract class BrushShape : IReadOnlyCollection<IntVector2>
    {
        public abstract IntRect boundingRect { get; }
        public abstract Texture2D texture { get; }

        public abstract int Count { get; }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract IEnumerator<IntVector2> GetEnumerator();

        public class Pixel : BrushShape
        {
            public override IntRect boundingRect => new IntRect(IntVector2.zero, IntVector2.zero);

            public override Texture2D texture { get; } = Texture2DExtensions.Solid(1, 1, Config.Colours.mask);

            public override int Count => 1;

            public Pixel() { }

            public override IEnumerator<IntVector2> GetEnumerator()
            {
                yield return IntVector2.zero;
            }
        }

        public class Circle : BrushShape, IHasBrushSize
        {
            public int brushSize
            {
                get => backingShape.boundingRect.width / 2 + 1;
                set
                {
                    backingShape.boundingRect = new IntRect(new IntVector2(-value + 1, -value + 1), new IntVector2(value - 1, value - 1));

                    _texture.Reinitialize(boundingRect.width, boundingRect.height);
                    foreach (IntVector2 pixel in _texture.GetRect())
                    {
                        if (backingShape.Contains(pixel + boundingRect.bottomLeft))
                        {
                            _texture.SetPixel(pixel, Config.Colours.mask);
                        }
                        else
                        {
                            _texture.SetPixel(pixel, Config.Colours.transparent);
                        }
                    }
                    _texture.Apply();
                }
            }

            public override IntRect boundingRect => backingShape.boundingRect;

            private Texture2D _texture;
            public override Texture2D texture => _texture;

            /// <summary>
            /// The <see cref="Geometry.Shapes.Interfaces.IShape"/> that backs this <see cref="BrushShape"/>.
            /// </summary>
            private Geometry.Shapes.Ellipse backingShape;

            public override int Count => backingShape.Count;

            /// <param name="brushSize">See <see cref="brushSize"/>.</param>
            public Circle(int brushSize)
            {
                backingShape = new Geometry.Shapes.Ellipse(new IntRect(IntVector2.zero, IntVector2.zero), true);
                _texture = new Texture2D(1, 1);
                this.brushSize = brushSize;
            }

            public override IEnumerator<IntVector2> GetEnumerator() => backingShape.GetEnumerator();
        }

        public class Square : BrushShape, IHasBrushSize
        {
            public int brushSize
            {
                get => backingShape.boundingRect.width / 2 + 1;
                set
                {
                    backingShape.boundingRect = new IntRect(new IntVector2(-value + 1, -value + 1), new IntVector2(value - 1, value - 1));

                    _texture.Reinitialize(boundingRect.width, boundingRect.height);
                    foreach (IntVector2 pixel in _texture.GetRect())
                    {
                        if (backingShape.Contains(pixel + boundingRect.bottomLeft))
                        {
                            _texture.SetPixel(pixel, Config.Colours.mask);
                        }
                        else
                        {
                            _texture.SetPixel(pixel, Config.Colours.transparent);
                        }
                    }
                    _texture.Apply();
                }
            }

            public override IntRect boundingRect => backingShape.boundingRect;

            private Texture2D _texture;
            public override Texture2D texture => _texture;

            /// <summary>
            /// The <see cref="Geometry.Shapes.Interfaces.IShape"/> that backs this <see cref="BrushShape"/>.
            /// </summary>
            private Geometry.Shapes.Rectangle backingShape;

            public override int Count => backingShape.Count;

            /// <param name="brushSize">See <see cref="brushSize"/>.</param>
            public Square(int brushSize)
            {
                backingShape = new Geometry.Shapes.Rectangle(new IntRect(IntVector2.zero, IntVector2.zero), true);
                _texture = new Texture2D(1, 1);
                this.brushSize = brushSize;
            }

            public override IEnumerator<IntVector2> GetEnumerator() => backingShape.GetEnumerator();
        }

        public class Diamond : BrushShape, IHasBrushSize
        {
            public int brushSize
            {
                get => backingShape.boundingRect.width / 2 + 1;
                set
                {
                    backingShape.boundingRect = new IntRect(new IntVector2(-value + 1, -value + 1), new IntVector2(value - 1, value - 1));

                    _texture.Reinitialize(boundingRect.width, boundingRect.height);
                    foreach (IntVector2 pixel in _texture.GetRect())
                    {
                        if (backingShape.Contains(pixel + boundingRect.bottomLeft))
                        {
                            _texture.SetPixel(pixel, Config.Colours.mask);
                        }
                        else
                        {
                            _texture.SetPixel(pixel, Config.Colours.transparent);
                        }
                    }
                    _texture.Apply();
                }
            }

            public override IntRect boundingRect => backingShape.boundingRect;

            private Texture2D _texture;
            public override Texture2D texture => _texture;

            /// <summary>
            /// The <see cref="Geometry.Shapes.Interfaces.IShape"/> that backs this <see cref="BrushShape"/>.
            /// </summary>
            private Geometry.Shapes.Diamond backingShape;

            public override int Count => backingShape.Count;

            /// <param name="brushSize">See <see cref="brushSize"/>.</param>
            public Diamond(int brushSize)
            {
                backingShape = new Geometry.Shapes.Diamond(new IntRect(IntVector2.zero, IntVector2.zero), true);
                _texture = new Texture2D(1, 1);
                this.brushSize = brushSize;
            }

            public override IEnumerator<IntVector2> GetEnumerator() => backingShape.GetEnumerator();
        }

        public class Custom : BrushShape
        {
            private IntVector2[] pixels;

            private IntRect _boundingRect;
            public override IntRect boundingRect => _boundingRect;

            private Texture2D _texture;
            public override Texture2D texture => _texture;

            public override int Count => pixels.Length;

            public Custom(IEnumerable<IntVector2> pixels)
            {
                this.pixels = pixels.Distinct().ToArray();
                _boundingRect = IntRect.BoundingRect(this.pixels);

                _texture = Texture2DExtensions.Solid(boundingRect.width, boundingRect.height, Config.Colours.transparent);
                foreach (IntVector2 pixel in pixels)
                {
                    _texture.SetPixel(pixel, Config.Colours.mask);
                }
                _texture.Apply();
            }

            /// <summary>
            /// Takes any pixels with non-zero alpha.
            /// </summary>
            public Custom(Texture2D texture) : this(texture.GetRect().Where(pixel => texture.GetPixel(pixel).a != 0f)) { }

            public override IEnumerator<IntVector2> GetEnumerator() => (pixels as IEnumerable<IntVector2>).GetEnumerator();
        }
    }
}