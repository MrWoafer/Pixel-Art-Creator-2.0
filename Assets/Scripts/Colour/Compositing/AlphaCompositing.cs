using PAC.Extensions;

using UnityEngine;

namespace PAC.Colour.Compositing
{
    /// <summary>
    /// Methods for combining two colours based on their alpha values.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Alpha_compositing"/>
    public abstract record AlphaCompositing
    {
        /// <summary>
        /// Alpha compositing methods where the colours are in <see href="https://learn.microsoft.com/en-us/windows/apps/develop/win2d/premultiplied-alpha#premultiplied-alpha-1">premultiplied
        /// alpha</see> form.
        /// </summary>
        public static class Premultiplied
        {
            /// <summary>
            /// The Porter-Duff <see href="https://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_srcover"><i>source-over</i></see> operator. Also known as
            /// <see href="https://www.w3.org/TR/compositing-1/#simplealphacompositing"><i>simple alpha compositing</i></see>.
            /// </summary>
            /// <returns>
            /// The composited colour in premultiplied alpha form.
            /// </returns>
            /// <remarks>
            /// <para>
            /// <paramref name="source"/> and <paramref name="destination"/> should be in premultiplied alpha form. This method does not check they are valid premultiplied colours.
            /// </para>
            /// <para>
            /// Does not do any colour space conversion.
            /// </para>
            /// </remarks>
            /// <param name="source">The foreground colour.</param>
            /// <param name="destination">The background colour.</param>
            public static Color SourceOver(Color source, Color destination) => source + (1f - source.a) * destination;
        }

        /// <summary>
        /// Alpha compositing methods where the colours are in <see href="https://learn.microsoft.com/en-us/windows/apps/develop/win2d/premultiplied-alpha#straight-alpha">straight alpha</see>
        /// form.
        /// </summary>
        public static class Straight
        {
            /// <summary>
            /// The Porter-Duff <see href="https://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_srcover"><i>source-over</i></see> operator. Also known as
            /// <see href="https://www.w3.org/TR/compositing-1/#simplealphacompositing"><i>simple alpha compositing</i></see>.
            /// </summary>
            /// <returns>
            /// The composited colour in straight alpha form.
            /// </returns>
            /// <remarks>
            /// <para>
            /// <paramref name="source"/> and <paramref name="destination"/> should be in straight alpha form.
            /// </para>
            /// <para>
            /// Does not do any colour space conversion.
            /// </para>
            /// </remarks>
            /// <param name="source">The foreground colour.</param>
            /// <param name="destination">The background colour.</param>
            public static Color SourceOver(Color source, Color destination) => Premultiplied.SourceOver(source.Premultiplied(), destination.Premultiplied()).Straight();
        }
    }
}