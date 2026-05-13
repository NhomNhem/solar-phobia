// Assets/_Project/Application/Services/ConsequenceResolver.cs
using System;
using System.Collections.Generic;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Features.Consequences
{
    /// <summary>
    /// Deterministic mapping of abandoned soul IDs to night curse outcomes.
    /// Implements ADR-0007: Consequence Resolver Pattern.
    ///
    /// One-write rule: Resolve() may only be called once per run.
    /// Second calls throw InvalidOperationException.
    ///
    /// Mapping:
    ///   "Linh" → NightOutcomeState.Drag    (Water Trap)
    ///   "Van"  → NightOutcomeState.Block    (Blood Net)
    ///   "Minh" → NightOutcomeState.FakeShrine (Illusion)
    ///   Any other ID → defaults to Drag, logs InvalidSoulId warning
    /// </summary>
    public class ConsequenceResolver : IConsequenceResolver
    {
        // ── Static Mapping (from ADR-0007 and GDD) ────────────────────
        private static readonly Dictionary<string, NightOutcomeState> CurseMap = new()
        {
            ["linh"] = NightOutcomeState.Drag,
            ["van"]  = NightOutcomeState.Block,
            ["minh"] = NightOutcomeState.FakeShrine
        };

        // ── State ─────────────────────────────────────────────────────
        public bool HasResolved { get; private set; }

        // ── Dependencies ──────────────────────────────────────────────
        private readonly IGhostRepository _ghostRepository;

        public ConsequenceResolver(IGhostRepository ghostRepository)
        {
            _ghostRepository = ghostRepository;
        }

        /// <summary>
        /// Resolves the cursed fate of an abandoned soul.
        /// Writes NightOutcomeState on the ghost entity (AC-6).
        /// </summary>
        public CursePayload Resolve(string abandonedSoulId)
        {
            if (HasResolved)
            {
                throw new InvalidOperationException("Already resolved");
            }

            HasResolved = true;

            // Look up curse type (default to Drag for invalid/null IDs)
            if (!CurseMap.TryGetValue(abandonedSoulId ?? string.Empty, out var curseType))
            {
                curseType = NightOutcomeState.Drag;
            }

            // Write outcome on ghost entity (AC-6: Integration with Ghost Model)
            if (abandonedSoulId != null)
            {
                var ghost = _ghostRepository?.GetById(abandonedSoulId);
                if (ghost != null)
                {
                    ghost.NightOutcome = curseType;
                }
            }

            return new CursePayload
            {
                CurseType = curseType,
                Intensity = 1.0f,
                SpawnBias = abandonedSoulId
            };
        }

        public void Reset()
        {
            HasResolved = false;
        }
    }
}
