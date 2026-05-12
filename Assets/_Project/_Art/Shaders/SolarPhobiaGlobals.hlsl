#ifndef SOLAR_PHOBIA_GLOBALS_INCLUDED
#define SOLAR_PHOBIA_GLOBALS_INCLUDED

// Global parameters driven by SolarPhobiaVFXDirector.cs
// These are set via Shader.SetGlobal* methods.

float _SP_Phase01;
float _SP_DayPressure01;
float _SP_NightDanger01;
float _SP_SensoryDecay01;
float3 _SP_PlayerWorldPos;

// Convenience macro for world-space distance checks from player
float GetDistanceToPlayer(float3 worldPos)
{
    return distance(worldPos, _SP_PlayerWorldPos);
}

#endif
