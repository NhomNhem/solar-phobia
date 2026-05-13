using NhemDangFugBixs.NhemLogging;
using SolarPhobia.Application.Features.Combat;
using SolarPhobia.Application.Features.Audio;
using SolarPhobia.Application.Features.Consequences;
using SolarPhobia.Application.Features.Consequences.WaterTrap;
using SolarPhobia.Application.Features.Day;
using SolarPhobia.Application.Features.Hazards;
using SolarPhobia.Application.Features.MainMenu;
using SolarPhobia.Application.Features.Map.Cover;
using SolarPhobia.Application.Features.Map.Directors;
using SolarPhobia.Application.Features.Phase.Day;
using SolarPhobia.Application.Features.Phase.Flow;
using SolarPhobia.Application.Features.Phase.Reset;
using SolarPhobia.Application.Features.Phase.Timeline;
using SolarPhobia.Application.Features.Player.Cursor;
using SolarPhobia.Application.Features.Player.Input;
using SolarPhobia.Application.Features.Player.Interactions;
using SolarPhobia.Application.Features.Player.Movement;
using SolarPhobia.Application.Features.Player.State;
using SolarPhobia.Application.Features.Player.Warnings;
using SolarPhobia.Application.Features.Resources;
using SolarPhobia.Application.Features.Rituals;
using SolarPhobia.Application.Features.Ward;
using SolarPhobia.Domain.Features.Ward;
using SolarPhobia.Application.Features.Phase;

using SolarPhobia.Application.Features.Shrines;
using SolarPhobia.Application.Features.Strike;
using SolarPhobia.Infrastructure.Features.Soul;
using SolarPhobia.Infrastructure.Features.Ghost;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Infrastructure.Hazards;
using SolarPhobia.Infrastructure.MainMenu;
using SolarPhobia.Infrastructure.Features.Audio;
using SolarPhobia.Infrastructure.Features.CameraControl;
using SolarPhobia.Infrastructure.Features.Ward;
using SolarPhobia.Shared.Configuration;
using SolarPhobia.Shared.InputActions;
using VContainer;
using VContainer.Unity;
namespace SolarPhobia.Composition.Installers
{
    public class CoreLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<INhemLogger>(new NhemUnityLogger());

            // ── Data-Driven Gameplay Config ──────────────────────────────────
            GameplayBalanceConfig gameplayBalanceConfig = GameplayBalanceConfigLoader.Load();
            builder.RegisterInstance(gameplayBalanceConfig);

            // ── Audio Service ───────────────────────────────────────────────
            builder.Register<BroAudioService>(Lifetime.Singleton).As<IAudioCueService>();

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
            builder.Register<StrikeWarningController>(Lifetime.Singleton).As<IStrikeWarningController>();
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
            builder.Register<KarmaHazardRuntime>(Lifetime.Singleton).As<IKarmaHazardRuntime>();
            builder.Register<KarmaHazardService>(Lifetime.Singleton).As<IKarmaHazardService>();

            // ── Camera Service ───────────────────────────────────────────────
            builder.Register<DayNightCameraController>(Lifetime.Singleton).As<IDayNightCameraController>();

            // ── Ward Timer ─────────────────────────────────────────────────
            builder.Register<WardTimerService>(Lifetime.Singleton)
                .AsSelf()
                .As<IWardTimerService>()
                .As<IWardTimerPort>();
            builder.RegisterEntryPoint<WardTimerServiceEntryPoint>(Lifetime.Singleton);
            builder.RegisterEntryPoint<WardDeathTriggerService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<NightToDayResetService>(Lifetime.Singleton);

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
            builder.Register<GhostRepository>(Lifetime.Singleton).As<IGhostRepository>();

            // ── Input Actions (New Input System) ─────────────────────────
            builder.Register<SolarPhobiaInputActions>(Lifetime.Singleton);
        }
    }
}





