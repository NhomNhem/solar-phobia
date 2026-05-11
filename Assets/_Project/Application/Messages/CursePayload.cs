// Assets/_Project/Application/Messages/CursePayload.cs
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Messages
{
    /// <summary>
    /// Payload produced by the Consequence Resolver after mapping an abandoned soul to a curse.
    /// Delivered to downstream systems (Map & Spawn Director, Curse Effect Modules).
    /// </summary>
    public class CursePayload
    {
        public NightOutcomeState CurseType { get; set; }
        public float Intensity { get; set; }
        public string SpawnBias { get; set; }
    }
}