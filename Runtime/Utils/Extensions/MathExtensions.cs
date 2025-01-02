using Unity.Mathematics;

namespace ControlRigging
{
    public static class MathExtensions
    {
        /// <summary>
        ///   <para>Loops the value t, so that it is never larger than length and never smaller than 0.</para>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        public static float Repeat(this float t, float length = 1)
        {
            return math.clamp(t - math.floor(t / length) * length, 0.0f, length);
        }
    }
}