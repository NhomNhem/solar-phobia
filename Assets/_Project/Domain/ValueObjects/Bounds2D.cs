namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// Engine-agnostic axis-aligned 2D bounds value.
    /// </summary>
    public readonly struct Bounds2D
    {
        public readonly Float2 Center;
        public readonly Float2 Size;

        public Bounds2D(Float2 center, Float2 size)
        {
            Center = center;
            Size = size;
        }
    }
}
