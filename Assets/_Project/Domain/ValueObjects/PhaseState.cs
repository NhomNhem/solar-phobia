namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// Represents the different phases of the game state machine.
    /// </summary>
    public enum PhaseState
    {
        /// <summary>Initial boot phase</summary>
        Boot,
        
        /// <summary>Daytime hub where players prepare</summary>
        DayService,
        
        /// <summary>Dialogue phase with NPCs</summary>
        Dialogue,
        
        /// <summary>Order preparation phase</summary>
        Order,
        
        /// <summary>Sunset warning phase</summary>
        SunsetWarning,
        
        /// <summary>Night travel phase</summary>
        NightTravel,
        
        /// <summary>Shrine arrival phase</summary>
        ShrineArrival,
        
        /// <summary>Ending evaluation phase</summary>
        EndingEvaluation,
        
        /// <summary>Night survival phase (active gameplay)</summary>
        NightSurvival,
        
        /// <summary>Choice lock phase during decision making</summary>
        ChoiceLock
    }
}