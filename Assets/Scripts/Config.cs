using UnityEngine;

namespace PAC
{
    public static class Config
    {
        public static class Files
        {
            public const string pacFileExtension = "pac";
        }

        public static class Tools
        {
            public const int minBrushSize = 1;
            public const int maxBrushSize = 25;
        }

        public static class Colours
        {
            public static readonly Color transparent = new Color(0f, 0f, 0f, 0f);

            public static readonly Color mask = new Color(1f, 1f, 1f, 1f);

            public static readonly Color brushOutlineDark = new Color(0f, 0f, 0f, 1f);
            public static readonly Color brushOutlineLight = new Color(1f, 1f, 1f, 1f);
        }
    }
}
