// Assets/_Project/Application/Services/Interfaces/IConsequenceResolver.cs
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Resolves abandoned soul IDs into curse payloads for the night phase.
    /// Enforces the one-write rule: Resolve() may only be called once per run.
    /// </summary>
    public interface IConsequenceResolver
    {
        /// <summary>True if Resolve has already been called this run.</summary>
        bool HasResolved { get; }

        /// <summary>
        /// Map an abandoned soul ID to a NightOutcomeState and return the curse payload.
        /// Throws InvalidOperationException if called more than once.
        /// </summary>
        CursePayload Resolve(string abandonedSoulId);
    }
}