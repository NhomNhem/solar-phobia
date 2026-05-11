using System;

namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// Represents the timeline phases within the Day Service phase.
    /// These phases create escalating pressure as time progresses.
    /// </summary>
    [Serializable]
    public enum TimelinePhase
    {
        /// <summary>0:00-1:30 - Space for 4 people (Tú + 3 souls)</summary>
        Stability,
        
        /// <summary>1:30-3:00 - Shadows shrink 30%, Light Interrupts, Souls Panic AI</summary>
        Tension,
        
        /// <summary>3:00-4:30 - Only space for 3 people, player must Swap</summary>
        Crisis,
        
        /// <summary>4:30-5:00 - One soul MUST be abandoned</summary>
        Collapse,
        
        /// <summary>5:00+ - Transition to ChoiceLock</summary>
        ChoiceLock
    }
}