using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Application.Services.Phase;
using UnityEngine;

namespace SolarPhobia.Application.Services.Combat
{
    public readonly struct StrikeWarning
    {
        /// <summary>
        /// Auto-incremented identifier for this warning instance.
        /// Assigned by <see cref="StrikeWarningController"/> at registration time.
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// World-space position of the strike at registration time.
        /// Used for nearest-strike priority selection.
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// Initialises a new <see cref="StrikeWarning"/> with the given identifier and position.
        /// </summary>
        /// <param name="id">Unique identifier for this warning instance.</param>
        /// <param name="position">World-space position of the incoming strike.</param>
        public StrikeWarning(int id, Vector2 position)
        {
            Id       = id;
            Position = position;
        }
    }
}
