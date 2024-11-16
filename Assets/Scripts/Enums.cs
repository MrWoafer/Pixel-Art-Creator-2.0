namespace PAC
{
    /// <summary>
    /// This should be interpreted as being clockwise.
    /// </summary>
    public enum RotationAngle
    {
        _0 = 0,
        _90 = 90,
        Minus90 = -90,
        _180 = 180
    }

    public enum FlipAxis
    {
        None = 0,
        Vertical = 1,
        Horizontal = 2,
        _45Degrees = 3,
        Minus45Degrees = 4,

    }
}