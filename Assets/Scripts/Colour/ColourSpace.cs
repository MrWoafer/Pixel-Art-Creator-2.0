namespace PAC.Colour
{
    /* Notes about colour spaces in Unity:
     * 
     * - Texture2D, SpriteRenderer, etc expect you to give colours in gamma space, even if your project is set to use linear space.
     * - The shadergraph 'Colorspace Conversion' node treats HSV and RGB as in gamma space.
     * - Shaders expect colour input/output to be in whatever colour space your Unity project is set to use.
     */
}
