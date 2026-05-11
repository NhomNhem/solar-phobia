namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// Represents the different actions that can trigger phase transitions.
    /// </summary>
    public enum GameAction
    {
        /// <summary>Start the game from boot</summary>
        StartGame,
        
        /// <summary>Complete day hub preparation</summary>
        CompleteDayPrep,
        
        /// <summary>Finish dialogue with NPC</summary>
        FinishDialogue,
        
        /// <summary>Complete order preparation</summary>
        CompleteOrder,
        
        /// <summary>Sunset warning timeout</summary>
        SunsetWarningTimeout,
        
        /// <summary>Arrive at shrine during night travel</summary>
        ArriveAtShrine,
        
        /// <summary>Complete ending evaluation</summary>
        CompleteEndingEvaluation,
        
        /// <summary>Survive night phase</summary>
        SurviveNight,
        
        /// <summary>Make a choice during choice lock</summary>
        MakeChoice,
        
        /// <summary>Reset game to initial state</summary>
        ResetGame
    }
}