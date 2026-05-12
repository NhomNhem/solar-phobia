namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// Engine-agnostic 2D point/vector value.
    /// </summary>
    public readonly struct Float2
    {
        public readonly float X;
        public readonly float Y;

        public Float2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
