using NUnit.Framework;
using R3;
using SolarPhobia.Application.MainMenu;
using System.Collections.Generic;

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
    [TestFixture]
    public class MainMenuApplicationServiceTests
    {
        private MainMenuApplicationService _service;
        private MainMenuUiState _state;
        private FakeMainMenuSettingsStore _settingsStore;
        private FakeMainMenuPlatformService _platformService;

        [SetUp]
        public void SetUp()
        {
            _settingsStore = new FakeMainMenuSettingsStore();
            _platformService = new FakeMainMenuPlatformService();
            _service = new MainMenuApplicationService(_settingsStore, _platformService);
            _service.CurrentState.Subscribe(next => _state = next);
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
        }

        [Test]
        public void RequestNewGame_WithExistingSave_OpensNewGameConfirm()
        {
            _settingsStore.SetInt("SaveData.hasSave", 1);
            _service.Initialize();

            _service.RequestNewGame();

            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.NewGameConfirm));
        }

        [Test]
        public void ConfirmNewGameOverwrite_EmitsStartEvent()
        {
            var emitted = false;
            _settingsStore.SetInt("SaveData.hasSave", 1);
            _service.Initialize();
            _service.OnStartNewGameRequested.Subscribe(_ => emitted = true);
            _service.RequestNewGame();

            _service.ConfirmNewGameOverwrite();

            Assert.That(emitted, Is.True);
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.MainMenu));
        }

        [Test]
        public void SetSettingsTab_Accessibility_UpdatesState()
        {
            _service.Initialize();
            _service.OpenSettings();

            _service.SetSettingsTab(3);

            Assert.That(_state.ActiveTab, Is.EqualTo(MainMenuSettingsTab.Accessibility));
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.Settings));
        }

        [Test]
        public void SetResolution_OpensVideoApplyConfirm_AndCancelReturnsToSettings()
        {
            _service.Initialize();
            _service.OpenSettings();

            _service.SetResolution("1920x1080");
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.VideoApplyConfirm));

            _service.CancelVideoApply();
            Assert.That(_state.ScreenState, Is.EqualTo(MainMenuScreenState.Settings));
        }

        private sealed class FakeMainMenuSettingsStore : IMainMenuSettingsStore
        {
            private readonly Dictionary<string, int> _ints = new();
            private readonly Dictionary<string, float> _floats = new();
            private readonly Dictionary<string, string> _strings = new();

            public int GetInt(string key, int defaultValue)
            {
                return _ints.TryGetValue(key, out var value) ? value : defaultValue;
            }

            public float GetFloat(string key, float defaultValue)
            {
                return _floats.TryGetValue(key, out var value) ? value : defaultValue;
            }

            public string GetString(string key, string defaultValue)
            {
                return _strings.TryGetValue(key, out var value) ? value : defaultValue;
            }

            public void SetInt(string key, int value)
            {
                _ints[key] = value;
            }

            public void SetFloat(string key, float value)
            {
                _floats[key] = value;
            }

            public void SetString(string key, string value)
            {
                _strings[key] = value;
            }
        }

        private sealed class FakeMainMenuPlatformService : IMainMenuPlatformService
        {
            public int ScreenWidth { get; set; } = 1280;
            public int ScreenHeight { get; set; } = 720;
            public int CurrentQualityLevel { get; set; } = 1;
            public string CurrentWindowModeName { get; set; } = "Windowed";
            public float MasterVolume { get; private set; }
            public bool VSyncEnabled { get; private set; }

            public int GetCurrentQualityLevel()
            {
                return CurrentQualityLevel;
            }

            public string GetCurrentWindowModeName()
            {
                return CurrentWindowModeName;
            }

            public void ApplyMasterVolume(float value)
            {
                MasterVolume = value;
            }

            public void ApplyVSync(bool enabled)
            {
                VSyncEnabled = enabled;
            }

            public void ApplyResolution(int width, int height)
            {
                ScreenWidth = width;
                ScreenHeight = height;
            }

            public void ApplyWindowMode(string modeName)
            {
                CurrentWindowModeName = modeName;
            }

            public void ApplyQualityLevel(int qualityIndex)
            {
                CurrentQualityLevel = qualityIndex;
            }
        }
    }
}

