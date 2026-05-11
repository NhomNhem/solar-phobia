namespace SolarPhobia.Shared.Conventions
{
    public static class CodingRules
    {
        /// <summary>
        /// R3 ReactiveProperty access pattern - use wrapper property, not .Value directly.
        /// </summary>
        /// <example>
        /// // WRONG - ReadOnlyReactiveProperty doesn't have .Value
        /// var state = _myProperty.Value;
        ///
        /// // CORRECT - add wrapper property to interface
        /// public T CurrentStateValue => _myProperty.Value;
        /// var state = _myManager.CurrentStateValue;
        /// </example>
        public const string R3PropertyAccess = "Use wrapper property CurrentStateValue, not .Value on ReadOnlyReactiveProperty";

        /// <summary>
        /// Required using statements based on what's being used.
        /// </summary>
        public static class RequiredUsings
        {
            public const string UnityEngine = "using UnityEngine; // for Vector3, Quaternion, etc.";
            public const string Messages = "// for PhaseChangedEvent, DayStartEvent, etc.";
            public const string Domain = "using SolarPhobia.Domain.ValueObjects; // for PhaseState, NightOutcomeState, etc.";
        }

        /// <summary>
        /// Valid PhaseState values from PhaseState.cs
        /// </summary>
        public static class PhaseStates
        {
            public const string Boot = "Boot";
            public const string DayService = "DayService";
            public const string Dialogue = "Dialogue";
            public const string Order = "Order";
            public const string SunsetWarning = "SunsetWarning";
            public const string NightTravel = "NightTravel";
            public const string ShrineArrival = "ShrineArrival";
            public const string EndingEvaluation = "EndingEvaluation";
            public const string NightSurvival = "NightSurvival";
            public const string ChoiceLock = "ChoiceLock";
        }
    }
}
