using System;
using R3;
using UnityEngine;

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

        public void Initialize()
        {
            var width = PlayerPrefs.GetInt(KeyResolutionWidth, Screen.width);
            var height = PlayerPrefs.GetInt(KeyResolutionHeight, Screen.height);
            var qualityIndex = Mathf.Clamp(PlayerPrefs.GetInt(KeyQualityLevel, QualitySettings.GetQualityLevel()), 0, QualityLevels.Length - 1);

            _state = new MainMenuUiState
            {
                ScreenState = MainMenuScreenState.MainMenu,
                ActiveTab = MainMenuSettingsTab.Audio,
                HasSave = PlayerPrefs.GetInt(KeyHasSave, 0) == 1,
                SaveDay = PlayerPrefs.GetInt(KeySaveDayNumber, 1),
                Settings = new MainMenuSettingsSnapshot
                {
                    MasterVolume = PlayerPrefs.GetFloat(KeyMasterVolume, DefaultMasterVolume),
                    MusicVolume = PlayerPrefs.GetFloat(KeyMusicVolume, DefaultMusicVolume),
                    SfxVolume = PlayerPrefs.GetFloat(KeySfxVolume, DefaultSfxVolume),
                    AmbientVolume = PlayerPrefs.GetFloat(KeyAmbientVolume, DefaultAmbientVolume),
                    UiScale = PlayerPrefs.GetFloat(KeyUiScale, DefaultUiScale),
                    Subtitles = PlayerPrefs.GetInt(KeySubtitlesEnabled, DefaultSubtitles ? 1 : 0) == 1,
                    VSync = PlayerPrefs.GetInt(KeyVSync, DefaultVSync ? 1 : 0) == 1,
                    CameraShake = PlayerPrefs.GetInt(KeyCameraShake, DefaultCameraShake ? 1 : 0) == 1,
                    MotionBlur = PlayerPrefs.GetInt(KeyMotionBlur, DefaultMotionBlur ? 1 : 0) == 1,
                    InvertY = PlayerPrefs.GetInt(KeyInvertYAxis, DefaultInvertY ? 1 : 0) == 1,
                    GamepadVibration = PlayerPrefs.GetInt(KeyGamepadVibration, DefaultGamepadVibration ? 1 : 0) == 1,
                    TextSize = PlayerPrefs.GetString(KeyTextSize, DefaultTextSize),
                    InputDevice = PlayerPrefs.GetString(KeyInputDevice, DefaultInputDevice),
                    Resolution = $"{width}x{height}",
                    WindowMode = PlayerPrefs.GetString(KeyWindowMode, GetCurrentWindowModeName()),
                    Quality = QualityLevels[qualityIndex],
                    HighContrast = PlayerPrefs.GetInt(KeyAccessibilityHighContrast, DefaultHighContrast ? 1 : 0) == 1,
                    ReduceMotion = PlayerPrefs.GetInt(KeyAccessibilityReduceMotion, DefaultReduceMotion ? 1 : 0) == 1,
                    ColorblindCues = PlayerPrefs.GetInt(KeyAccessibilityColorblindCues, DefaultColorblindCues ? 1 : 0) == 1
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
            var clamped = Mathf.Clamp(index, 0, 3);
            _state.ActiveTab = (MainMenuSettingsTab)clamped;
            Publish();
        }

        public void SetMasterVolume(float value)
        {
            _state.Settings.MasterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KeyMasterVolume, _state.Settings.MasterVolume);
            ApplyAudioSettings();
            Publish();
        }

        public void SetMusicVolume(float value)
        {
            _state.Settings.MusicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KeyMusicVolume, _state.Settings.MusicVolume);
            Publish();
        }

        public void SetSfxVolume(float value)
        {
            _state.Settings.SfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KeySfxVolume, _state.Settings.SfxVolume);
            Publish();
        }

        public void SetAmbientVolume(float value)
        {
            _state.Settings.AmbientVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KeyAmbientVolume, _state.Settings.AmbientVolume);
            Publish();
        }

        public void SetUiScale(float value)
        {
            _state.Settings.UiScale = Mathf.Clamp(value, 0.75f, 2.0f);
            PlayerPrefs.SetFloat(KeyUiScale, _state.Settings.UiScale);
            Publish();
        }

        public void SetSubtitles(bool value)
        {
            _state.Settings.Subtitles = value;
            PlayerPrefs.SetInt(KeySubtitlesEnabled, value ? 1 : 0);
            Publish();
        }

        public void SetVSync(bool value)
        {
            _state.Settings.VSync = value;
            PlayerPrefs.SetInt(KeyVSync, value ? 1 : 0);
            ApplyVideoSettings();
            Publish();
        }

        public void SetCameraShake(bool value)
        {
            _state.Settings.CameraShake = value;
            PlayerPrefs.SetInt(KeyCameraShake, value ? 1 : 0);
            Publish();
        }

        public void SetMotionBlur(bool value)
        {
            _state.Settings.MotionBlur = value;
            PlayerPrefs.SetInt(KeyMotionBlur, value ? 1 : 0);
            Publish();
        }

        public void SetInvertY(bool value)
        {
            _state.Settings.InvertY = value;
            PlayerPrefs.SetInt(KeyInvertYAxis, value ? 1 : 0);
            Publish();
        }

        public void SetGamepadVibration(bool value)
        {
            _state.Settings.GamepadVibration = value;
            PlayerPrefs.SetInt(KeyGamepadVibration, value ? 1 : 0);
            Publish();
        }

        public void SetTextSize(string value)
        {
            _state.Settings.TextSize = value ?? DefaultTextSize;
            PlayerPrefs.SetString(KeyTextSize, _state.Settings.TextSize);
            Publish();
        }

        public void SetInputDevice(string value)
        {
            _state.Settings.InputDevice = value ?? DefaultInputDevice;
            PlayerPrefs.SetString(KeyInputDevice, _state.Settings.InputDevice);
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
            PlayerPrefs.SetInt(KeyAccessibilityHighContrast, value ? 1 : 0);
            Publish();
        }

        public void SetReduceMotion(bool value)
        {
            _state.Settings.ReduceMotion = value;
            PlayerPrefs.SetInt(KeyAccessibilityReduceMotion, value ? 1 : 0);
            Publish();
        }

        public void SetColorblindCues(bool value)
        {
            _state.Settings.ColorblindCues = value;
            PlayerPrefs.SetInt(KeyAccessibilityColorblindCues, value ? 1 : 0);
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
                    Screen.SetResolution(width, height, Screen.fullScreenMode);
                    PlayerPrefs.SetInt(KeyResolutionWidth, width);
                    PlayerPrefs.SetInt(KeyResolutionHeight, height);
                    _state.Settings.Resolution = _pendingResolution;
                }
            }

            if (!string.IsNullOrWhiteSpace(_pendingWindowMode))
            {
                Screen.fullScreenMode = ToFullScreenMode(_pendingWindowMode);
                PlayerPrefs.SetString(KeyWindowMode, _pendingWindowMode);
                _state.Settings.WindowMode = _pendingWindowMode;
            }

            if (_pendingQualityIndex >= 0 && _pendingQualityIndex < QualityLevels.Length)
            {
                QualitySettings.SetQualityLevel(_pendingQualityIndex, true);
                PlayerPrefs.SetInt(KeyQualityLevel, _pendingQualityIndex);
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
            AudioListener.volume = _state.Settings.MasterVolume;
        }

        private void ApplyVideoSettings()
        {
            QualitySettings.vSyncCount = _state.Settings.VSync ? 1 : 0;
        }

        private static FullScreenMode ToFullScreenMode(string mode)
        {
            return mode switch
            {
                "Fullscreen" => FullScreenMode.ExclusiveFullScreen,
                "Borderless" => FullScreenMode.FullScreenWindow,
                _ => FullScreenMode.Windowed
            };
        }

        private static string GetCurrentWindowModeName()
        {
            return Screen.fullScreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => "Fullscreen",
                FullScreenMode.FullScreenWindow => "Borderless",
                _ => "Windowed"
            };
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
    }
}
