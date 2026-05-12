using UnityEngine;

namespace SolarPhobia.Infrastructure.VFX
{
    /// <summary>
    /// Caches shader property IDs for global Solar Phobia parameters.
    /// Use these IDs instead of strings for performance in update loops.
    /// </summary>
    public static class ShaderPropertyIds
    {
        public static readonly int Phase01 = Shader.PropertyToID("_SP_Phase01");
        public static readonly int DayPressure01 = Shader.PropertyToID("_SP_DayPressure01");
        public static readonly int NightDanger01 = Shader.PropertyToID("_SP_NightDanger01");
        public static readonly int SensoryDecay01 = Shader.PropertyToID("_SP_SensoryDecay01");
        public static readonly int PlayerWorldPos = Shader.PropertyToID("_SP_PlayerWorldPos");
    }
}
