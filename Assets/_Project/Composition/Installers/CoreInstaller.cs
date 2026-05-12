using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Infrastructure.Services.Input;
using SolarPhobia.Infrastructure.Services;
using VContainer.Unity;
using VContainer;

namespace SolarPhobia.Composition.Installers
{
    public class CoreLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ── Audio Service ───────────────────────────────────────────────
            builder.Register<BroAudioService>(Lifetime.Singleton).As<SolarPhobia.Application.Services.IAudioService>();

            // ── Phase State Machine ─────────────────────────────────────────
            builder.Register<PhaseStateMachine>(Lifetime.Singleton).As<IPhaseStateMachine>();

            // ── Soul Repository ────────────────────────────────────────────────
            builder.Register<SoulRepository>(Lifetime.Singleton).As<ISoulRepository>();

            // ── Day Phase Timeline ─────────────────────────────────────────────
            builder.Register<SolarPhobia.Application.Services.Phase.DayPhaseTimelineService>(Lifetime.Singleton).As<IDayPhaseTimelineService>();

            // ── Core Gameplay Services ────────────────────────────────────────
            builder.Register<SolarPhobia.Application.Services.Input.PlayerInputHandler>(Lifetime.Singleton).As<IPlayerInputHandler>();
            builder.Register<SolarPhobia.Application.Services.Phase.PlayerStateMachine>(Lifetime.Singleton).As<IPlayerStateMachine>();
            builder.Register<SolarPhobia.Application.Services.Map.MapSpawnDirector>(Lifetime.Singleton).As<IMapSpawnDirector>();
            builder.Register<SolarPhobia.Application.Services.StrikeWarningController>(Lifetime.Singleton).As<IStrikeWarningController>();
            builder.Register<SolarPhobia.Application.Services.Combat.StrikeController>(Lifetime.Singleton).As<IStrikeController>();
            builder.Register<SolarPhobia.Application.Services.Movement.SprintController>(Lifetime.Singleton).As<ISprintController>();
            builder.Register<SolarPhobia.Application.Services.Movement.SwingGlideController>(Lifetime.Singleton).As<ISwingGlideController>();
            builder.Register<SolarPhobia.Application.Services.Movement.DashController>(Lifetime.Singleton).As<IDashController>();
            builder.Register<SolarPhobia.Application.Services.Phase.DayActionController>(Lifetime.Singleton).As<IDayActionController>();
            builder.Register<InteractHandler>(Lifetime.Singleton).As<IInteractHandler>();
            builder.Register<SolarPhobia.Application.Services.Cover.CoverDetector2D>(Lifetime.Singleton).As<ICoverDetector2D>();
            builder.Register<SolarPhobia.Application.Services.Input.CursorController>(Lifetime.Singleton).As<ICursorController>();
            builder.Register<SolarPhobia.Application.Services.Movement.Movement2DCalculator>(Lifetime.Singleton).As<IMovement2DCalculator>();
            builder.Register<SolarPhobia.Application.Services.Movement.PlatformerFeelController>(Lifetime.Singleton).As<IPlatformerFeelController>();
            builder.Register<KarmaHazardService>(Lifetime.Singleton).As<IKarmaHazardService>();

            // ── Camera Service ───────────────────────────────────────────────
            builder.Register<DayNightCameraController>(Lifetime.Singleton).As<IDayNightCameraController>();

            // ── Ward Timer ─────────────────────────────────────────────────
            builder.RegisterEntryPoint<WardTimerService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SolarPhobia.Application.Services.Objective.WardDeathTriggerService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SolarPhobia.Application.Services.Phase.NightToDayResetService>(Lifetime.Singleton);

            // ── Day Selection Validator ──────────────────────────────────
            builder.Register<DaySelectionValidator>(Lifetime.Singleton).As<IDaySelectionValidator>();

            // ── Day Service UI Controller ───────────────────────────────
            builder.Register<DayServiceUIController>(Lifetime.Singleton).As<IDayServiceUIController>();

            // ── Main Menu Application Service ───────────────────────────
            builder.Register<MainMenuApplicationService>(Lifetime.Singleton).As<IMainMenuApplicationService>();

            // ── Ritual Assignment ──────────────────────────────────────
            builder.Register<SolarPhobia.Application.Services.Objective.RitualAssignmentService>(Lifetime.Singleton).As<IRitualAssignmentService>();

            // ── Curse Effect Manager ──────────────────────────────────────
            builder.Register<CurseEffectManager>(Lifetime.Singleton).As<ICurseEffectManager>();

            // ── Water Trap Effect ──────────────────────────────────────
            builder.Register<SolarPhobia.Application.Services.Combat.WaterTrapEffectService>(Lifetime.Singleton).As<IWaterTrapEffectService>();

            // ── Consequence Resolver ────────────────────────────────────
            builder.Register<ConsequenceResolver>(Lifetime.Singleton).As<IConsequenceResolver>();
            builder.Register<SoulRepository>(Lifetime.Singleton).As<IGhostRepository>();

            // ── Input Actions (New Input System) ─────────────────────────
            builder.Register<SolarPhobiaInputActions>(Lifetime.Singleton);
        }
    }
}
