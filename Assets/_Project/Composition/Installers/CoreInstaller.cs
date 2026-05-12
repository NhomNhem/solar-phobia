using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Shared.Configuration;
using SolarPhobia.Shared.InputActions;
using SolarPhobia.Infrastructure.Services;
using VContainer.Unity;
using VContainer;

namespace SolarPhobia.Composition.Installers
{
    public class CoreLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ── Data-Driven Gameplay Config ──────────────────────────────────
            GameplayBalanceConfig gameplayBalanceConfig = GameplayBalanceConfigLoader.Load();
            builder.RegisterInstance(gameplayBalanceConfig);

            // ── Audio Service ───────────────────────────────────────────────
            builder.Register<BroAudioService>(Lifetime.Singleton).As<SolarPhobia.Application.Services.IAudioService>();

            // ── Phase State Machine ─────────────────────────────────────────
            builder.Register<PhaseStateMachine>(Lifetime.Singleton).As<IPhaseStateMachine>();

            // ── Soul Repository ────────────────────────────────────────────────
            builder.Register<SoulRepository>(Lifetime.Singleton).As<ISoulRepository>();

            // ── Day Phase Timeline ─────────────────────────────────────────────
            builder.Register<DayPhaseTimelineService>(Lifetime.Singleton).As<IDayPhaseTimelineService>();

            // ── Core Gameplay Services ────────────────────────────────────────
            builder.Register<PlayerInputHandler>(Lifetime.Singleton).As<IPlayerInputHandler>();
            builder.Register<PlayerStateMachine>(Lifetime.Singleton).As<IPlayerStateMachine>();
            builder.Register<MapSpawnDirector>(Lifetime.Singleton).As<IMapSpawnDirector>();
            builder.Register<SolarPhobia.Application.Services.StrikeWarningController>(Lifetime.Singleton).As<IStrikeWarningController>();
            builder.Register<StrikeController>(Lifetime.Singleton).As<IStrikeController>();
            builder.Register<SprintController>(Lifetime.Singleton).As<ISprintController>();
            builder.Register<SwingGlideController>(Lifetime.Singleton).As<ISwingGlideController>();
            builder.Register<DashController>(Lifetime.Singleton).As<IDashController>();
            builder.Register<DayActionController>(Lifetime.Singleton).As<IDayActionController>();
            builder.Register<InteractHandler>(Lifetime.Singleton).As<IInteractHandler>();
            builder.Register<CoverDetector2D>(Lifetime.Singleton).As<ICoverDetector2D>();
            builder.Register<CursorController>(Lifetime.Singleton).As<ICursorController>();
            builder.Register<Movement2DCalculator>(Lifetime.Singleton).As<IMovement2DCalculator>();
            builder.Register<PlatformerFeelController>(Lifetime.Singleton).As<IPlatformerFeelController>();
            builder.Register<KarmaHazardService>(Lifetime.Singleton).As<IKarmaHazardService>();

            // ── Camera Service ───────────────────────────────────────────────
            builder.Register<DayNightCameraController>(Lifetime.Singleton).As<IDayNightCameraController>();

            // ── Ward Timer ─────────────────────────────────────────────────
            builder.Register<WardTimerService>(Lifetime.Singleton)
                .AsSelf()
                .As<SolarPhobia.Domain.IWardTimerService>()
                .As<IWardTimerService>();
            builder.RegisterEntryPoint<WardTimerServiceEntryPoint>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SolarPhobia.Application.Services.Objective.WardDeathTriggerService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SolarPhobia.Application.Services.Phase.NightToDayResetService>(Lifetime.Singleton);

            // ── Day Selection Validator ──────────────────────────────────
            builder.Register<DaySelectionValidator>(Lifetime.Singleton).As<IDaySelectionValidator>();

            // ── Day Service UI Controller ───────────────────────────────
            builder.Register<DayServiceUIController>(Lifetime.Singleton).As<IDayServiceUIController>();

            // ── Main Menu Application Service ───────────────────────────
            builder.Register<MainMenuSettingsStore>(Lifetime.Singleton).As<IMainMenuSettingsStore>();
            builder.Register<MainMenuPlatformService>(Lifetime.Singleton).As<IMainMenuPlatformService>();
            builder.Register<MainMenuApplicationService>(Lifetime.Singleton).As<IMainMenuApplicationService>();

            // ── Ritual Assignment ──────────────────────────────────────
            builder.Register<RitualAssignmentService>(Lifetime.Singleton).As<IRitualAssignmentService>();

            // ── Resource Effects ───────────────────────────────────────
            builder.Register<ResourceEffectsService>(Lifetime.Singleton).As<IResourceEffectsService>();

            // ── Curse Effect Manager ──────────────────────────────────────
            builder.Register<CurseEffectManager>(Lifetime.Singleton).As<ICurseEffectManager>();

            // ── Water Trap Effect ──────────────────────────────────────
            builder.Register<WaterTrapEffectService>(Lifetime.Singleton).As<IWaterTrapEffectService>();

            // ── Consequence Resolver ────────────────────────────────────
            builder.Register<ConsequenceResolver>(Lifetime.Singleton).As<IConsequenceResolver>();
            builder.Register<SoulRepository>(Lifetime.Singleton).As<IGhostRepository>();

            // ── Input Actions (New Input System) ─────────────────────────
            builder.Register<SolarPhobiaInputActions>(Lifetime.Singleton);
        }
    }
}
