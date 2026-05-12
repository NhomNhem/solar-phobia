using System;
using R3;

namespace SolarPhobia.Application.Services
{
    public enum MainMenuScreenState
    {
        MainMenu,
        Settings,
        Credits,
        QuitConfirm,
        NewGameConfirm,
        VideoApplyConfirm
    }

    public enum MainMenuSettingsTab
    {
        Audio,
        Video,
        Controls,
        Accessibility
    }

    public sealed class MainMenuUiState
    {
        public MainMenuScreenState ScreenState { get; set; }
        public MainMenuSettingsTab ActiveTab { get; set; }
        public bool HasSave { get; set; }
        public int SaveDay { get; set; }
        public MainMenuSettingsSnapshot Settings { get; set; }
    }

    public sealed class MainMenuSettingsSnapshot
    {
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }
        public float AmbientVolume { get; set; }
        public float UiScale { get; set; }
        public bool Subtitles { get; set; }
        public bool VSync { get; set; }
        public bool CameraShake { get; set; }
        public bool MotionBlur { get; set; }
        public bool InvertY { get; set; }
        public bool GamepadVibration { get; set; }
        public string TextSize { get; set; }
        public string InputDevice { get; set; }
        public string Resolution { get; set; }
        public string WindowMode { get; set; }
        public string Quality { get; set; }
        public bool HighContrast { get; set; }
        public bool ReduceMotion { get; set; }
        public bool ColorblindCues { get; set; }
    }

    public sealed class MainMenuApplicationService : IMainMenuApplicationService, IDisposable
    {
        // ── Persistence Keys ───────────────────────────────────────────
        private const string KeyMasterVolume = "Settings.MasterVolume";
        private const string KeyMusicVolume = "Settings.MusicVolume";
        private const string KeySfxVolume = "Settings.SFXVolume";
        private const string KeyAmbientVolume = "Settings.AmbientVolume";
        private const string KeySubtitlesEnabled = "Settings.SubtitlesEnabled";
        private const string KeyTextSize = "Settings.TextSize";
        private const string KeyResolutionWidth = "Settings.ResolutionWidth";
        private const string KeyResolutionHeight = "Settings.ResolutionHeight";
        private const string KeyWindowMode = "Settings.WindowMode";
        private const string KeyQualityLevel = "Settings.QualityLevel";
        private const string KeyVSync = "Settings.VSync";
        private const string KeyCameraShake = "Settings.CameraShake";
        private const string KeyMotionBlur = "Settings.MotionBlur";
        private const string KeyUiScale = "Settings.UIScale";
        private const string KeyInputDevice = "Settings.InputDevice";
        private const string KeyInvertYAxis = "Settings.InvertYAxis";
        private const string KeyGamepadVibration = "Settings.GamepadVibration";
        private const string KeyAccessibilityHighContrast = "Settings.Accessibility.HighContrast";
        private const string KeyAccessibilityReduceMotion = "Settings.Accessibility.ReduceMotion";
        private const string KeyAccessibilityColorblindCues = "Settings.Accessibility.ColorblindCues";
        private const string KeyHasSave = "SaveData.hasSave";
        private const string KeySaveDayNumber = "SaveData.dayNumber";

        // ── Defaults ───────────────────────────────────────────────────
        private const float DefaultMasterVolume = 0.75f;
        private const float DefaultMusicVolume = 0.60f;
        private const float DefaultSfxVolume = 0.80f;
        private const float DefaultAmbientVolume = 0.50f;
        private const float DefaultUiScale = 1.0f;
        private const string DefaultTextSize = "Medium";
        private const string DefaultInputDevice = "Keyboard/Mouse";
        private const bool DefaultSubtitles = true;
        private const bool DefaultVSync = true;
        private const bool DefaultCameraShake = true;
        private const bool DefaultMotionBlur = false;
        private const bool DefaultInvertY = false;
        private const bool DefaultGamepadVibration = true;
        private const bool DefaultHighContrast = false;
        private const bool DefaultReduceMotion = false;
        private const bool DefaultColorblindCues = false;

        // ── Reactive Outputs ───────────────────────────────────────────
        private readonly ReactiveProperty<MainMenuUiState> _currentState = new();
        private readonly Subject<Unit> _startNewGameRequested = new();
        private readonly Subject<Unit> _continueRequested = new();
        private readonly Subject<Unit> _quitConfirmed = new();
        private readonly IMainMenuSettingsStore _settingsStore;
        private readonly IMainMenuPlatformService _platformService;

        public ReadOnlyReactiveProperty<MainMenuUiState> CurrentState => _currentState;
        public Observable<Unit> OnStartNewGameRequested => _startNewGameRequested;
        public Observable<Unit> OnContinueRequested => _continueRequested;
        public Observable<Unit> OnQuitConfirmed => _quitConfirmed;

        // ── State ──────────────────────────────────────────────────────
        private MainMenuUiState _state;
        private string _pendingResolution;
        private string _pendingWindowMode;
        private int _pendingQualityIndex = -1;

        private static readonly string[] QualityLevels =
        {
            "Low",
            "Medium",
            "High",
            "Ultra",
            "Fantastic",
            "Maximum"
        };

        public MainMenuApplicationService(IMainMenuSettingsStore settingsStore, IMainMenuPlatformService platformService)
        {
            _settingsStore = settingsStore;
            _platformService = platformService;
        }

        public void Initialize()
        {
            var width = _settingsStore.GetInt(KeyResolutionWidth, _platformService.ScreenWidth);
            var height = _settingsStore.GetInt(KeyResolutionHeight, _platformService.ScreenHeight);
            var qualityIndex = Clamp(_settingsStore.GetInt(KeyQualityLevel, _platformService.GetCurrentQualityLevel()), 0, QualityLevels.Length - 1);

            _state = new MainMenuUiState
            {
                ScreenState = MainMenuScreenState.MainMenu,
                ActiveTab = MainMenuSettingsTab.Audio,
                HasSave = _settingsStore.GetInt(KeyHasSave, 0) == 1,
                SaveDay = _settingsStore.GetInt(KeySaveDayNumber, 1),
                Settings = new MainMenuSettingsSnapshot
                {
                    MasterVolume = _settingsStore.GetFloat(KeyMasterVolume, DefaultMasterVolume),
                    MusicVolume = _settingsStore.GetFloat(KeyMusicVolume, DefaultMusicVolume),
                    SfxVolume = _settingsStore.GetFloat(KeySfxVolume, DefaultSfxVolume),
                    AmbientVolume = _settingsStore.GetFloat(KeyAmbientVolume, DefaultAmbientVolume),
                    UiScale = _settingsStore.GetFloat(KeyUiScale, DefaultUiScale),
                    Subtitles = _settingsStore.GetInt(KeySubtitlesEnabled, DefaultSubtitles ? 1 : 0) == 1,
                    VSync = _settingsStore.GetInt(KeyVSync, DefaultVSync ? 1 : 0) == 1,
                    CameraShake = _settingsStore.GetInt(KeyCameraShake, DefaultCameraShake ? 1 : 0) == 1,
                    MotionBlur = _settingsStore.GetInt(KeyMotionBlur, DefaultMotionBlur ? 1 : 0) == 1,
                    InvertY = _settingsStore.GetInt(KeyInvertYAxis, DefaultInvertY ? 1 : 0) == 1,
                    GamepadVibration = _settingsStore.GetInt(KeyGamepadVibration, DefaultGamepadVibration ? 1 : 0) == 1,
                    TextSize = _settingsStore.GetString(KeyTextSize, DefaultTextSize),
                    InputDevice = _settingsStore.GetString(KeyInputDevice, DefaultInputDevice),
                    Resolution = $"{width}x{height}",
                    WindowMode = _settingsStore.GetString(KeyWindowMode, _platformService.GetCurrentWindowModeName()),
                    Quality = QualityLevels[qualityIndex],
                    HighContrast = _settingsStore.GetInt(KeyAccessibilityHighContrast, DefaultHighContrast ? 1 : 0) == 1,
                    ReduceMotion = _settingsStore.GetInt(KeyAccessibilityReduceMotion, DefaultReduceMotion ? 1 : 0) == 1,
                    ColorblindCues = _settingsStore.GetInt(KeyAccessibilityColorblindCues, DefaultColorblindCues ? 1 : 0) == 1
                }
            };

            ApplyAudioSettings();
            ApplyVideoSettings();
            _pendingResolution = null;
            _pendingWindowMode = null;
            _pendingQualityIndex = -1;
            Publish();
        }

        public void RequestNewGame()
        {
            if (_state.HasSave)
            {
                _state.ScreenState = MainMenuScreenState.NewGameConfirm;
                Publish();
                return;
            }

            _startNewGameRequested.OnNext(Unit.Default);
        }

        public void ConfirmNewGameOverwrite()
        {
            _state.ScreenState = MainMenuScreenState.MainMenu;
            Publish();
            _startNewGameRequested.OnNext(Unit.Default);
        }

        public void RequestContinue()
        {
            if (!_state.HasSave)
            {
                return;
            }

            _continueRequested.OnNext(Unit.Default);
        }

        public void OpenSettings()
        {
            _state.ScreenState = MainMenuScreenState.Settings;
            Publish();
        }

        public void OpenCredits()
        {
            _state.ScreenState = MainMenuScreenState.Credits;
            Publish();
        }

        public void RequestQuit()
        {
            _state.ScreenState = MainMenuScreenState.QuitConfirm;
            Publish();
        }

        public void ConfirmQuit()
        {
            _state.ScreenState = MainMenuScreenState.MainMenu;
            Publish();
            _quitConfirmed.OnNext(Unit.Default);
        }

        public void CancelDialog()
        {
            _state.ScreenState = MainMenuScreenState.MainMenu;
            Publish();
        }

        public void Back()
        {
            switch (_state.ScreenState)
            {
                case MainMenuScreenState.MainMenu:
                    RequestQuit();
                    break;
                case MainMenuScreenState.Settings:
                case MainMenuScreenState.Credits:
                case MainMenuScreenState.QuitConfirm:
                case MainMenuScreenState.NewGameConfirm:
                case MainMenuScreenState.VideoApplyConfirm:
                    _state.ScreenState = MainMenuScreenState.MainMenu;
                    Publish();
                    break;
            }
        }

        public void SetSettingsTab(int index)
        {
            var clamped = Clamp(index, 0, 3);
            _state.ActiveTab = (MainMenuSettingsTab)clamped;
            Publish();
        }

        public void SetMasterVolume(float value)
        {
            _state.Settings.MasterVolume = Clamp01(value);
            _settingsStore.SetFloat(KeyMasterVolume, _state.Settings.MasterVolume);
            ApplyAudioSettings();
            Publish();
        }

        public void SetMusicVolume(float value)
        {
            _state.Settings.MusicVolume = Clamp01(value);
            _settingsStore.SetFloat(KeyMusicVolume, _state.Settings.MusicVolume);
            Publish();
        }

        public void SetSfxVolume(float value)
        {
            _state.Settings.SfxVolume = Clamp01(value);
            _settingsStore.SetFloat(KeySfxVolume, _state.Settings.SfxVolume);
            Publish();
        }

        public void SetAmbientVolume(float value)
        {
            _state.Settings.AmbientVolume = Clamp01(value);
            _settingsStore.SetFloat(KeyAmbientVolume, _state.Settings.AmbientVolume);
            Publish();
        }

        public void SetUiScale(float value)
        {
            _state.Settings.UiScale = Clamp(value, 0.75f, 2.0f);
            _settingsStore.SetFloat(KeyUiScale, _state.Settings.UiScale);
            Publish();
        }

        public void SetSubtitles(bool value)
        {
            _state.Settings.Subtitles = value;
            _settingsStore.SetInt(KeySubtitlesEnabled, value ? 1 : 0);
            Publish();
        }

        public void SetVSync(bool value)
        {
            _state.Settings.VSync = value;
            _settingsStore.SetInt(KeyVSync, value ? 1 : 0);
            ApplyVideoSettings();
            Publish();
        }

        public void SetCameraShake(bool value)
        {
            _state.Settings.CameraShake = value;
            _settingsStore.SetInt(KeyCameraShake, value ? 1 : 0);
            Publish();
        }

        public void SetMotionBlur(bool value)
        {
            _state.Settings.MotionBlur = value;
            _settingsStore.SetInt(KeyMotionBlur, value ? 1 : 0);
            Publish();
        }

        public void SetInvertY(bool value)
        {
            _state.Settings.InvertY = value;
            _settingsStore.SetInt(KeyInvertYAxis, value ? 1 : 0);
            Publish();
        }

        public void SetGamepadVibration(bool value)
        {
            _state.Settings.GamepadVibration = value;
            _settingsStore.SetInt(KeyGamepadVibration, value ? 1 : 0);
            Publish();
        }

        public void SetTextSize(string value)
        {
            _state.Settings.TextSize = value ?? DefaultTextSize;
            _settingsStore.SetString(KeyTextSize, _state.Settings.TextSize);
            Publish();
        }

        public void SetInputDevice(string value)
        {
            _state.Settings.InputDevice = value ?? DefaultInputDevice;
            _settingsStore.SetString(KeyInputDevice, _state.Settings.InputDevice);
            Publish();
        }

        public void SetResolution(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            _pendingResolution = value;
            _state.ScreenState = MainMenuScreenState.VideoApplyConfirm;
            Publish();
        }

        public void SetWindowMode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            _pendingWindowMode = value;
            _state.ScreenState = MainMenuScreenState.VideoApplyConfirm;
            Publish();
        }

        public void SetQuality(string value)
        {
            var index = Array.IndexOf(QualityLevels, value);
            if (index < 0)
            {
                return;
            }

            _pendingQualityIndex = index;
            _state.ScreenState = MainMenuScreenState.VideoApplyConfirm;
            Publish();
        }

        public void SetHighContrast(bool value)
        {
            _state.Settings.HighContrast = value;
            _settingsStore.SetInt(KeyAccessibilityHighContrast, value ? 1 : 0);
            Publish();
        }

        public void SetReduceMotion(bool value)
        {
            _state.Settings.ReduceMotion = value;
            _settingsStore.SetInt(KeyAccessibilityReduceMotion, value ? 1 : 0);
            Publish();
        }

        public void SetColorblindCues(bool value)
        {
            _state.Settings.ColorblindCues = value;
            _settingsStore.SetInt(KeyAccessibilityColorblindCues, value ? 1 : 0);
            Publish();
        }

        public void ConfirmVideoApply()
        {
            if (!string.IsNullOrWhiteSpace(_pendingResolution))
            {
                var parts = _pendingResolution.Split('x');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out var width) &&
                    int.TryParse(parts[1], out var height))
                {
                    _platformService.ApplyResolution(width, height);
                    _settingsStore.SetInt(KeyResolutionWidth, width);
                    _settingsStore.SetInt(KeyResolutionHeight, height);
                    _state.Settings.Resolution = _pendingResolution;
                }
            }

            if (!string.IsNullOrWhiteSpace(_pendingWindowMode))
            {
                _platformService.ApplyWindowMode(_pendingWindowMode);
                _settingsStore.SetString(KeyWindowMode, _pendingWindowMode);
                _state.Settings.WindowMode = _pendingWindowMode;
            }

            if (_pendingQualityIndex >= 0 && _pendingQualityIndex < QualityLevels.Length)
            {
                _platformService.ApplyQualityLevel(_pendingQualityIndex);
                _settingsStore.SetInt(KeyQualityLevel, _pendingQualityIndex);
                _state.Settings.Quality = QualityLevels[_pendingQualityIndex];
            }

            _pendingResolution = null;
            _pendingWindowMode = null;
            _pendingQualityIndex = -1;
            _state.ScreenState = MainMenuScreenState.Settings;
            ApplyVideoSettings();
            Publish();
        }

        public void CancelVideoApply()
        {
            _pendingResolution = null;
            _pendingWindowMode = null;
            _pendingQualityIndex = -1;
            _state.ScreenState = MainMenuScreenState.Settings;
            Publish();
        }

        public void Dispose()
        {
            _currentState?.Dispose();
            _startNewGameRequested?.Dispose();
            _continueRequested?.Dispose();
            _quitConfirmed?.Dispose();
        }

        // ── Helpers ────────────────────────────────────────────────────
        private void ApplyAudioSettings()
        {
            _platformService.ApplyMasterVolume(_state.Settings.MasterVolume);
        }

        private void ApplyVideoSettings()
        {
            _platformService.ApplyVSync(_state.Settings.VSync);
        }

        private void Publish()
        {
            _currentState.Value = CloneState(_state);
        }

        private static MainMenuUiState CloneState(MainMenuUiState state)
        {
            return new MainMenuUiState
            {
                ScreenState = state.ScreenState,
                ActiveTab = state.ActiveTab,
                HasSave = state.HasSave,
                SaveDay = state.SaveDay,
                Settings = new MainMenuSettingsSnapshot
                {
                    MasterVolume = state.Settings.MasterVolume,
                    MusicVolume = state.Settings.MusicVolume,
                    SfxVolume = state.Settings.SfxVolume,
                    AmbientVolume = state.Settings.AmbientVolume,
                    UiScale = state.Settings.UiScale,
                    Subtitles = state.Settings.Subtitles,
                    VSync = state.Settings.VSync,
                    CameraShake = state.Settings.CameraShake,
                    MotionBlur = state.Settings.MotionBlur,
                    InvertY = state.Settings.InvertY,
                    GamepadVibration = state.Settings.GamepadVibration,
                    TextSize = state.Settings.TextSize,
                    InputDevice = state.Settings.InputDevice,
                    Resolution = state.Settings.Resolution,
                    WindowMode = state.Settings.WindowMode,
                    Quality = state.Settings.Quality,
                    HighContrast = state.Settings.HighContrast,
                    ReduceMotion = state.Settings.ReduceMotion,
                    ColorblindCues = state.Settings.ColorblindCues
                }
            };
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }

        private static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }

        private static float Clamp01(float value)
        {
            return Clamp(value, 0f, 1f);
        }
    }
}
