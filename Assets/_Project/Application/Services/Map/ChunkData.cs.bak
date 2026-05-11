// Assets/_Project/Application/Services/Map/ChunkData.cs
namespace SolarPhobia.Application.Services.Map
{
    /// <summary>
    /// Data produced by MapSpawnDirector.GenerateChunk().
    /// Describes the contents of one lane segment.
    /// Pure data struct — no Unity scene references.
    /// </summary>
    public readonly struct ChunkData
    {
        /// <summary>Chunk index in the lane sequence.</summary>
        public readonly int Index;

        /// <summary>Seed used to generate this chunk (chunkSeed = runSeed + index).</summary>
        public readonly int ChunkSeed;

        /// <summary>Number of SafeMound (MoThuong) cover objects in this chunk.</summary>
        public readonly int SafeMoundCount;

        /// <summary>Number of CursedMound (MoOan) objects in this chunk.</summary>
        public readonly int CursedMoundCount;

        /// <summary>Whether this chunk contains a FalseSafeMound trap.</summary>
        public readonly bool HasFalseSafeMound;

        /// <summary>Relative X positions of SafeMounds within the chunk (0.0–1.0).</summary>
        public readonly float[] SafeMoundPositions;

        /// <summary>Relative X positions of CursedMounds within the chunk (0.0–1.0).</summary>
        public readonly float[] CursedMoundPositions;

        public ChunkData(
            int index,
            int chunkSeed,
            int safeMoundCount,
            int cursedMoundCount,
            bool hasFalseSafeMound,
            float[] safeMoundPositions,
            float[] cursedMoundPositions)
        {
            Index               = index;
            ChunkSeed           = chunkSeed;
            SafeMoundCount      = safeMoundCount;
            CursedMoundCount    = cursedMoundCount;
            HasFalseSafeMound   = hasFalseSafeMound;
            SafeMoundPositions  = safeMoundPositions;
            CursedMoundPositions = cursedMoundPositions;
        }
    }
}
