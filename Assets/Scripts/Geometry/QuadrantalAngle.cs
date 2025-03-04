namespace PAC.Geometry
{
    /// <summary>
    /// An angle that's a multiple of 90 degrees.
    /// </summary>
    public enum QuadrantalAngle : byte
    {
        /// <summary>
        /// 0 degrees.
        /// </summary>
        _0 = 0,
        /// <summary>
        /// 90 degrees in the clockwise direction.
        /// </summary>
        Clockwise90 = 1,
        /// <summary>
        /// 180 degrees.
        /// </summary>
        _180 = 2,
        /// <summary>
        /// 90 degrees in the anticlockwise direction.
        /// </summary>
        Anticlockwise90 = 3
    }
}