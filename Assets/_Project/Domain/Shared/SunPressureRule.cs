
namespace SolarPhobia.Domain
{
    public static class SunPressureRule
    {
        public static bool IsHighPressure(GamePhase phase)
        {
            return phase == GamePhase.SUNSET_WARNING || phase == GamePhase.NIGHT_TRAVEL;
        }
    }
}

