// Assets/_Project/Application/Editor/Tests/MapSeedTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Map.Directors;

using SolarPhobia.Application.Resources;

using SolarPhobia.Application.Strike;

using SolarPhobia.Application.Consequences.WaterTrap;

using SolarPhobia.Application.Rituals;

using SolarPhobia.Application.Shrines;

using SolarPhobia.Application.Day;

using SolarPhobia.Application.Flow;

using SolarPhobia.Application.Player.State;

using SolarPhobia.Application.Player.Input;

using SolarPhobia.Application.Player.Interactions;

using SolarPhobia.Application.Player.Cursor;

using SolarPhobia.Application.Player.Events;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-001 â€” Deterministic Seed + Chunk Generation.
    /// Story 001: Deterministic Seed â€” Chunk Generation + Run Reproducibility.
    ///
    /// Tests MapSpawnDirector in isolation.
    /// No Unity scene, no MonoBehaviour required.
    /// </summary>
    [TestFixture]
    public class MapSeedTests
    {
        private MapSpawnDirector _director;

        [SetUp]
        public void Setup()
        {
            _director = new MapSpawnDirector();
        }

        // â”€â”€ AC-1: Initialize(seed) sets deterministic RNG â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC1_Initialize_SetsSeed()
        {
            _director.Initialize(42);

            Assert.AreEqual(42, _director.Seed);
        }

        [Test]
        public void AC1_Initialize_DifferentSeed_StoresDifferentSeed()
        {
            _director.Initialize(999);

            Assert.AreEqual(999, _director.Seed);
        }

        [Test]
        public void AC1_GenerateChunk_ThrowsIfNotInitialized()
        {
            Assert.Throws<System.InvalidOperationException>(
                () => _director.GenerateChunk(0),
                "GenerateChunk must throw if Initialize was not called"
            );
        }

        // â”€â”€ AC-2: Same seed + index â†’ identical ChunkData â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC2_SameSeedAndIndex_ProducesIdenticalChunk()
        {
            _director.Initialize(42);
            var chunk1 = _director.GenerateChunk(0);

            // Re-initialize with same seed
            _director.Initialize(42);
            var chunk2 = _director.GenerateChunk(0);

            Assert.AreEqual(chunk1.ChunkSeed,       chunk2.ChunkSeed);
            Assert.AreEqual(chunk1.SafeMoundCount,  chunk2.SafeMoundCount);
            Assert.AreEqual(chunk1.CursedMoundCount,chunk2.CursedMoundCount);
            Assert.AreEqual(chunk1.HasFalseSafeMound, chunk2.HasFalseSafeMound);
        }

        [Test]
        public void AC2_SameSeedAndIndex_ProducesIdenticalPositions()
        {
            _director.Initialize(100);
            var chunk1 = _director.GenerateChunk(3);

            _director.Initialize(100);
            var chunk2 = _director.GenerateChunk(3);

            Assert.AreEqual(chunk1.SafeMoundPositions.Length, chunk2.SafeMoundPositions.Length);
            for (int i = 0; i < chunk1.SafeMoundPositions.Length; i++)
            {
                Assert.AreEqual(chunk1.SafeMoundPositions[i], chunk2.SafeMoundPositions[i], 0.0001f,
                    $"SafeMound position [{i}] must be identical for same seed+index");
            }
        }

        [Test]
        public void AC2_ChunkSeed_IsRunSeedPlusIndex()
        {
            _director.Initialize(1000);
            var chunk = _director.GenerateChunk(5);

            Assert.AreEqual(1005, chunk.ChunkSeed,
                "ChunkSeed must equal runSeed + chunkIndex");
        }

        [Test]
        public void AC2_ChunkIndex_StoredInChunkData()
        {
            _director.Initialize(42);
            var chunk = _director.GenerateChunk(7);

            Assert.AreEqual(7, chunk.Index);
        }

        // â”€â”€ AC-3: Different seeds â†’ different layouts â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC3_DifferentSeeds_ProduceDifferentChunks()
        {
            _director.Initialize(1);
            var chunkA = _director.GenerateChunk(0);

            _director.Initialize(2);
            var chunkB = _director.GenerateChunk(0);

            // At minimum the chunk seeds must differ
            Assert.AreNotEqual(chunkA.ChunkSeed, chunkB.ChunkSeed,
                "Different run seeds must produce different chunk seeds");
        }

        [Test]
        public void AC3_DifferentIndices_SameSeed_ProduceDifferentChunks()
        {
            _director.Initialize(42);
            var chunk0 = _director.GenerateChunk(0);
            var chunk1 = _director.GenerateChunk(1);

            Assert.AreNotEqual(chunk0.ChunkSeed, chunk1.ChunkSeed,
                "Different chunk indices must produce different chunk seeds");
        }

        // â”€â”€ AC-4: Seed retrievable for snapshot â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC4_Seed_IsRetrievableAfterInitialize()
        {
            _director.Initialize(12345);

            Assert.AreEqual(12345, _director.Seed,
                "Seed must be retrievable for run snapshot/replay");
        }

        [Test]
        public void AC4_Seed_CanBeReinitialized()
        {
            _director.Initialize(1);
            _director.Initialize(2);

            Assert.AreEqual(2, _director.Seed,
                "Re-initializing must update the stored seed");
        }

        // â”€â”€ AC-5: Pure math â€” no Unity scene state â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC5_GenerateChunk_ReturnsValidChunkData()
        {
            _director.Initialize(42);
            var chunk = _director.GenerateChunk(0);

            Assert.GreaterOrEqual(chunk.SafeMoundCount, 0);
            Assert.GreaterOrEqual(chunk.CursedMoundCount, 0);
            Assert.IsNotNull(chunk.SafeMoundPositions);
            Assert.IsNotNull(chunk.CursedMoundPositions);
        }

        [Test]
        public void AC5_SafeMoundPositions_InRange_0_To_1()
        {
            _director.Initialize(42);
            var chunk = _director.GenerateChunk(0);

            foreach (float pos in chunk.SafeMoundPositions)
            {
                Assert.GreaterOrEqual(pos, 0f, "SafeMound position must be >= 0");
                Assert.LessOrEqual(pos, 1f,    "SafeMound position must be <= 1");
            }
        }

        [Test]
        public void AC5_CursedMoundPositions_InRange_0_To_1()
        {
            _director.Initialize(42);
            var chunk = _director.GenerateChunk(0);

            foreach (float pos in chunk.CursedMoundPositions)
            {
                Assert.GreaterOrEqual(pos, 0f);
                Assert.LessOrEqual(pos, 1f);
            }
        }

        // â”€â”€ Multiple chunks in sequence â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void MultipleChunks_AllDeterministic()
        {
            const int seed = 777;
            const int chunkCount = 10;

            _director.Initialize(seed);
            var firstPass = new int[chunkCount];
            for (int i = 0; i < chunkCount; i++)
            {
                firstPass[i] = _director.GenerateChunk(i).ChunkSeed;
            }

            _director.Initialize(seed);
            for (int i = 0; i < chunkCount; i++)
            {
                var chunk = _director.GenerateChunk(i);
                Assert.AreEqual(firstPass[i], chunk.ChunkSeed,
                    $"Chunk {i} must be identical on second pass with same seed");
            }
        }

        [Test]
        public void DefaultSafeMoundsPerChunk_Is2()
        {
            Assert.AreEqual(2, MapSpawnDirector.DefaultSafeMoundsPerChunk);
        }
    }
}

