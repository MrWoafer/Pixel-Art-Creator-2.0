using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.Extensions.System;
using PAC.Extensions.UnityEngine;
using PAC.Geometry;
using PAC.Geometry.Extensions;
using PAC.Interfaces;

using UnityEngine;

namespace PAC.ImageEditing
{
    public class TextureMask : IReadOnlyContains<IntVector2>
    {
        #region Fields
        public HashSet<IntVector2> pixels { get; private set; }
        public Texture2D texture { get; private set; }
        public readonly Color maskColour;
        #endregion

        #region Properties
        public IntRect rect => new IntRect(IntVector2.zero, new IntVector2(texture.width - 1, texture.height - 1));

        public int Count => pixels.Count;
        #endregion

        #region Events
        public event EventHandler<TextureMask, EventArgs> onMaskChanged;
        #endregion

        #region Constructors
        public TextureMask(int width, int height, Color maskColour)
        {
            pixels = new HashSet<IntVector2>(width * height);
            texture = Texture2DExtensions.Transparent(width, height);
            this.maskColour = maskColour;
        }

        public TextureMask(Texture2D texture, Color maskColour) : this(texture.width, texture.height, maskColour)
        {
            pixels = new HashSet<IntVector2>(texture.width * texture.height);
            texture = new Texture2D(texture.width, texture.height);
            this.maskColour = maskColour;

            foreach (IntVector2 pixel in rect)
            {
                if (texture.GetPixel(pixel).a == 0f)
                {
                    texture.SetPixel(pixel, Color.clear);
                }
                else
                {
                    AddNoEvent(pixel);
                }
            }
        }
        #endregion

        #region Operations
        public bool Contains(IntVector2 pixel)
        {
            // Throw error if outside as it's probably unintended
            if (!rect.Contains(pixel))
            {
                throw new ArgumentException($"{nameof(pixel)} must be inside {nameof(rect)}.", nameof(pixel));
            }

            return pixels.Contains(pixel);
        }

        private bool AddNoEvent(IntVector2 pixel)
        {
            if (!rect.Contains(pixel))
            {
                throw new ArgumentException($"{nameof(pixel)} must be inside {nameof(rect)}.", nameof(pixel));
            }

            bool changed = pixels.Add(pixel);
            texture.SetPixel(pixel, maskColour);
            return changed;
        }
        private bool AddNoEvent(IEnumerable<IntVector2> pixels)
        {
            if (pixels is null)
            {
                throw new ArgumentNullException(nameof(pixels));
            }

            return pixels.Any(pixel => AddNoEvent(pixel));
        }
        public bool Add(IntVector2 pixel)
        {
            bool changed = AddNoEvent(pixel);
            if (changed)
            {
                onMaskChanged?.Invoke(this, EventArgs.Empty);
            }
            return changed;
        }
        public bool Add(IEnumerable<IntVector2> pixels)
        {
            bool changed = AddNoEvent(pixels);
            if (changed)
            {
                onMaskChanged?.Invoke(this, EventArgs.Empty);
            }
            return changed;
        }

        private bool RemoveNoEvent(IntVector2 pixel)
        {
            // Throw error if outside as it's probably unintended
            if (!rect.Contains(pixel))
            {
                throw new ArgumentException($"{nameof(pixel)} must be inside {nameof(rect)}.", nameof(pixel));
            }

            bool changed = pixels.Remove(pixel);
            texture.SetPixel(pixel, Color.clear);
            return changed;
        }
        private bool RemoveNoEvent(IEnumerable<IntVector2> pixels)
        {
            if (pixels is null)
            {
                throw new ArgumentNullException(nameof(pixels));
            }

            return pixels.Any(pixel => RemoveNoEvent(pixel));
        }
        public bool Remove(IntVector2 pixel)
        {
            bool changed = RemoveNoEvent(pixel);
            if (changed)
            {
                onMaskChanged?.Invoke(this, EventArgs.Empty);
            }
            return changed;
        }
        public bool Remove(IEnumerable<IntVector2> pixels)
        {
            bool changed = RemoveNoEvent(pixels);
            if (changed)
            {
                onMaskChanged?.Invoke(this, EventArgs.Empty);
            }
            return changed;
        }

        public bool Set(IntVector2 pixel, bool value) => value ? Add(pixel) : Remove(pixel);
        public bool Set(IEnumerable<IntVector2> pixels, bool value) => value ? Add(pixels) : Remove(pixels);

        private bool ClearNoEvent()
        {
            if (Count == 0)
            {
                return false;
            }

            pixels.Clear();
            texture.SetPixels32(ArrayExtensions.Filled(new Color32(0, 0, 0, 0), rect.Count));

            return true;
        }
        public bool Clear()
        {
            bool changed = ClearNoEvent();
            if (changed)
            {
                onMaskChanged?.Invoke(this, EventArgs.Empty);
            }
            return changed;
        }

        public bool Overwrite(IEnumerable<IntVector2> pixels)
        {
            if (pixels is null)
            {
                throw new ArgumentNullException(nameof(pixels));
            }

            bool changed = ClearNoEvent() | AddNoEvent(pixels);
            if (changed)
            {
                onMaskChanged?.Invoke(this, EventArgs.Empty);
            }
            return changed;
        }
        #endregion

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator() => pixels.GetEnumerator();
    }
}