using System;
using System.Collections.Generic;
using R3;
using SolarPhobia.Application.Services;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace SolarPhobia.Presentation.MainMenu
{
    public class MainMenuController : MonoBehaviour, IDisposable
    {
        public static event Action OnNewGameRequested;
        public static event Action OnContinueRequested;
        public static event Action OnQuitRequested;

        [Inject] internal IMainMenuApplicationService _mainMenuService;
        [SerializeField] private UIDocument _document;

        // ── UI Root ────────────────────────────────────────────────────
        private VisualElement _root;
        private VisualElement _settingsPanel;
        private VisualElement _creditsPanel;
        private VisualElement _dialogOverlay;

        // ── Main Menu Buttons ──────────────────────────────────────────
        private Button _btnNewGame;
        private Button _btnContinue;
        private Button _btnSettings;
        private Button _btnCredits;
        private Button _btnQuit;

        // ── Settings ───────────────────────────────────────────────────
        private Button _btnCloseSettings;
        private RadioButtonGroup _settingsTabs;
        private VisualElement _tabAudio;
        private VisualElement _tabVideo;
        private VisualElement _tabControls;
        private VisualElement _tabAccessibility;
        private Slider _sliderMasterVolume;
        private Slider _sliderMusicVolume;
        private Slider _sliderSfxVolume;
        private Slider _sliderAmbientVolume;
        private Slider _sliderUiScale;
        private Toggle _toggleSubtitles;
        private Toggle _toggleVSync;
        private Toggle _toggleCameraShake;
        private Toggle _toggleMotionBlur;
        private Toggle _toggleInvertY;
        private Toggle _toggleGamepadVibration;
        private Toggle _toggleHighContrast;
        private Toggle _toggleReduceMotion;
        private Toggle _toggleColorblindCues;
        private DropdownField _dropdownResolution;
        private DropdownField _dropdownWindowMode;
        private DropdownField _dropdownQuality;
        private DropdownField _dropdownTextSize;
        private DropdownField _dropdownInputDevice;

        // ── Credits ────────────────────────────────────────────────────
        private Button _btnBackCredits;

        // ── Dialog ─────────────────────────────────────────────────────
        private Label _dialogTitle;
        private Label _dialogMessage;
        private Button _btnConfirmDialog;
        private Button _btnCancelDialog;

        // ── Labels ─────────────────────────────────────────────────────
        private Label _labelTitle;
        private Label _labelSubtitle;
        private Label _labelVersion;
        private Label _settingsTitle;
        private Label _creditsTitle;
        private Label _valueMasterVolume;
        private Label _valueMusicVolume;
        private Label _valueSfxVolume;
        private Label _valueAmbientVolume;
        private Label _valueUiScale;

        // ── State/Subscriptions ────────────────────────────────────────
        private readonly List<Button> _menuButtons = new();
        private IDisposable _stateSubscription;
        private IDisposable _newGameSubscription;
        private IDisposable _continueSubscription;
        private IDisposable _quitSubscription;
        private MainMenuUiState _latestState;
        private MainMenuScreenState _lastRenderedScreen;

        private void Awake()
        {
            if (_document == null)
            {
                _document = FindFirstObjectByType<UIDocument>();
            }

            if (_document == null)
            {
                Debug.LogError("[MainMenuController] UIDocument is required.");
                enabled = false;
                return;
            }

            if (_mainMenuService == null)
            {
                Debug.LogError("[MainMenuController] IMainMenuApplicationService was not injected.");
                enabled = false;
                return;
            }

            _root = _document.rootVisualElement;
            CacheElements();
            BindEvents();
            BindService();
            ApplyLocalizedText();
            LocalizationKeys.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            Dispose();
            LocalizationKeys.OnLanguageChanged -= OnLanguageChanged;
        }

        private void Update()
        {
            if (_latestState == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                if (_latestState.ScreenState == MainMenuScreenState.VideoApplyConfirm)
                {
                    _mainMenuService.CancelVideoApply();
                    return;
                }

                _mainMenuService.Back();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                if (_latestState.ScreenState == MainMenuScreenState.QuitConfirm ||
                    _latestState.ScreenState == MainMenuScreenState.NewGameConfirm ||
                    _latestState.ScreenState == MainMenuScreenState.VideoApplyConfirm)
                {
                    OnDialogConfirm();
                }
            }
        }

        public void Dispose()
        {
            _stateSubscription?.Dispose();
            _newGameSubscription?.Dispose();
            _continueSubscription?.Dispose();
            _quitSubscription?.Dispose();
            (_mainMenuService as IDisposable)?.Dispose();
        }

        private void OnLanguageChanged()
        {
            ApplyLocalizedText();
            if (_latestState != null)
            {
                RenderState(_latestState);
            }
        }

        private void CacheElements()
        {
            _btnNewGame = _root.Q<Button>("new-game");
            _btnContinue = _root.Q<Button>("continue");
            _btnSettings = _root.Q<Button>("settings");
            _btnCredits = _root.Q<Button>("credits");
            _btnQuit = _root.Q<Button>("quit");
            _menuButtons.AddRange(new[] { _btnNewGame, _btnContinue, _btnSettings, _btnCredits, _btnQuit });

            _settingsPanel = _root.Q<VisualElement>("settings-panel");
            _btnCloseSettings = _settingsPanel?.Q<Button>("settings-close");
            _settingsTabs = _settingsPanel?.Q<RadioButtonGroup>("settings-tab-group");
            _tabAudio = _settingsPanel?.Q<VisualElement>("settings-tab-content--audio");
            _tabVideo = _settingsPanel?.Q<VisualElement>("settings-tab-content--video");
            _tabControls = _settingsPanel?.Q<VisualElement>("settings-tab-content--controls");
            _tabAccessibility = _settingsPanel?.Q<VisualElement>("settings-tab-content--accessibility");

            _sliderMasterVolume = _settingsPanel?.Q<Slider>("master-volume");
            _sliderMusicVolume = _settingsPanel?.Q<Slider>("music-volume");
            _sliderSfxVolume = _settingsPanel?.Q<Slider>("sfx-volume");
            _sliderAmbientVolume = _settingsPanel?.Q<Slider>("ambient-volume");
            _sliderUiScale = _settingsPanel?.Q<Slider>("ui-scale");
            _toggleSubtitles = _settingsPanel?.Q<Toggle>("subtitles");
            _toggleVSync = _settingsPanel?.Q<Toggle>("vsync");
            _toggleCameraShake = _settingsPanel?.Q<Toggle>("camera-shake");
            _toggleMotionBlur = _settingsPanel?.Q<Toggle>("motion-blur");
            _toggleInvertY = _settingsPanel?.Q<Toggle>("invert-y");
            _toggleGamepadVibration = _settingsPanel?.Q<Toggle>("gamepad-vibration");
            _toggleHighContrast = _settingsPanel?.Q<Toggle>("high-contrast");
            _toggleReduceMotion = _settingsPanel?.Q<Toggle>("reduce-motion");
            _toggleColorblindCues = _settingsPanel?.Q<Toggle>("colorblind-cues");
            _dropdownResolution = _settingsPanel?.Q<DropdownField>("resolution");
            _dropdownWindowMode = _settingsPanel?.Q<DropdownField>("window-mode");
            _dropdownQuality = _settingsPanel?.Q<DropdownField>("quality");
            _dropdownTextSize = _settingsPanel?.Q<DropdownField>("text-size");
            _dropdownInputDevice = _settingsPanel?.Q<DropdownField>("input-device");

            if (_dropdownResolution != null)
            {
                _dropdownResolution.choices = new List<string>(SettingsDefaults.RESOLUTIONS);
            }

            if (_dropdownWindowMode != null)
            {
                _dropdownWindowMode.choices = new List<string>(SettingsDefaults.WINDOW_MODES);
            }

            if (_dropdownQuality != null)
            {
                _dropdownQuality.choices = new List<string>(SettingsDefaults.QUALITY_LEVELS);
            }

            if (_dropdownTextSize != null)
            {
                _dropdownTextSize.choices = new List<string>(SettingsDefaults.TEXT_SIZES);
            }

            if (_dropdownInputDevice != null)
            {
                _dropdownInputDevice.choices = new List<string>(SettingsDefaults.INPUT_DEVICES);
            }

            _creditsPanel = _root.Q<VisualElement>("credits-panel");
            _btnBackCredits = _creditsPanel?.Q<Button>("credits-back");

            _dialogOverlay = _root.Q<VisualElement>("dialog-overlay");
            _dialogTitle = _dialogOverlay?.Q<Label>("dialog-title");
            _dialogMessage = _dialogOverlay?.Q<Label>("dialog-message");
            _btnConfirmDialog = _dialogOverlay?.Q<Button>("confirm-quit");
            _btnCancelDialog = _dialogOverlay?.Q<Button>("cancel-quit");

            _labelTitle = _root.Q<Label>("title");
            _labelSubtitle = _root.Q<Label>("subtitle");
            _labelVersion = _root.Q<Label>("version");
            _settingsTitle = _root.Q<Label>("settings-title");
            _creditsTitle = _root.Q<Label>("credits-title");
            _valueMasterVolume = _settingsPanel?.Q<Label>("value-master-volume");
            _valueMusicVolume = _settingsPanel?.Q<Label>("value-music-volume");
            _valueSfxVolume = _settingsPanel?.Q<Label>("value-sfx-volume");
            _valueAmbientVolume = _settingsPanel?.Q<Label>("value-ambient-volume");
            _valueUiScale = _settingsPanel?.Q<Label>("value-ui-scale");
        }

        private void BindEvents()
        {
            _btnNewGame?.RegisterCallback<ClickEvent>(_ => _mainMenuService.RequestNewGame());
            _btnContinue?.RegisterCallback<ClickEvent>(_ => _mainMenuService.RequestContinue());
            _btnSettings?.RegisterCallback<ClickEvent>(_ => _mainMenuService.OpenSettings());
            _btnCredits?.RegisterCallback<ClickEvent>(_ => _mainMenuService.OpenCredits());
            _btnQuit?.RegisterCallback<ClickEvent>(_ => _mainMenuService.RequestQuit());
            _btnCloseSettings?.RegisterCallback<ClickEvent>(_ => _mainMenuService.Back());
            _btnBackCredits?.RegisterCallback<ClickEvent>(_ => _mainMenuService.Back());

            _btnConfirmDialog?.RegisterCallback<ClickEvent>(_ => OnDialogConfirm());
            _btnCancelDialog?.RegisterCallback<ClickEvent>(_ => OnDialogCancel());

            _settingsTabs?.RegisterValueChangedCallback(evt => _mainMenuService.SetSettingsTab(evt.newValue));

            _sliderMasterVolume?.RegisterValueChangedCallback(evt => _mainMenuService.SetMasterVolume(evt.newValue));
            _sliderMusicVolume?.RegisterValueChangedCallback(evt => _mainMenuService.SetMusicVolume(evt.newValue));
            _sliderSfxVolume?.RegisterValueChangedCallback(evt => _mainMenuService.SetSfxVolume(evt.newValue));
            _sliderAmbientVolume?.RegisterValueChangedCallback(evt => _mainMenuService.SetAmbientVolume(evt.newValue));
            _sliderUiScale?.RegisterValueChangedCallback(evt => _mainMenuService.SetUiScale(evt.newValue));

            _toggleSubtitles?.RegisterValueChangedCallback(evt => _mainMenuService.SetSubtitles(evt.newValue));
            _toggleVSync?.RegisterValueChangedCallback(evt => _mainMenuService.SetVSync(evt.newValue));
            _toggleCameraShake?.RegisterValueChangedCallback(evt => _mainMenuService.SetCameraShake(evt.newValue));
            _toggleMotionBlur?.RegisterValueChangedCallback(evt => _mainMenuService.SetMotionBlur(evt.newValue));
            _toggleInvertY?.RegisterValueChangedCallback(evt => _mainMenuService.SetInvertY(evt.newValue));
            _toggleGamepadVibration?.RegisterValueChangedCallback(evt => _mainMenuService.SetGamepadVibration(evt.newValue));
            _toggleHighContrast?.RegisterValueChangedCallback(evt => _mainMenuService.SetHighContrast(evt.newValue));
            _toggleReduceMotion?.RegisterValueChangedCallback(evt => _mainMenuService.SetReduceMotion(evt.newValue));
            _toggleColorblindCues?.RegisterValueChangedCallback(evt => _mainMenuService.SetColorblindCues(evt.newValue));

            _dropdownResolution?.RegisterValueChangedCallback(evt => _mainMenuService.SetResolution(evt.newValue));
            _dropdownWindowMode?.RegisterValueChangedCallback(evt => _mainMenuService.SetWindowMode(evt.newValue));
            _dropdownQuality?.RegisterValueChangedCallback(evt => _mainMenuService.SetQuality(evt.newValue));
            _dropdownTextSize?.RegisterValueChangedCallback(evt => _mainMenuService.SetTextSize(evt.newValue));
            _dropdownInputDevice?.RegisterValueChangedCallback(evt => _mainMenuService.SetInputDevice(evt.newValue));
        }

        private void BindService()
        {
            _mainMenuService.Initialize();

            _stateSubscription = _mainMenuService.CurrentState.Subscribe(RenderState);
            _newGameSubscription = _mainMenuService.OnStartNewGameRequested.Subscribe(_ => OnNewGameRequested?.Invoke());
            _continueSubscription = _mainMenuService.OnContinueRequested.Subscribe(_ => OnContinueRequested?.Invoke());
            _quitSubscription = _mainMenuService.OnQuitConfirmed.Subscribe(_ =>
            {
                OnQuitRequested?.Invoke();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });

        }

        private void RenderState(MainMenuUiState state)
        {
            if (state == null)
            {
                return;
            }

            _latestState = state;

            _settingsPanel.style.display = state.ScreenState == MainMenuScreenState.Settings ? DisplayStyle.Flex : DisplayStyle.None;
            _creditsPanel.style.display = state.ScreenState == MainMenuScreenState.Credits ? DisplayStyle.Flex : DisplayStyle.None;

            var showDialog =
                state.ScreenState == MainMenuScreenState.QuitConfirm ||
                state.ScreenState == MainMenuScreenState.NewGameConfirm ||
                state.ScreenState == MainMenuScreenState.VideoApplyConfirm;
            _dialogOverlay.style.display = showDialog ? DisplayStyle.Flex : DisplayStyle.None;

            _btnContinue.SetEnabled(state.HasSave);
            _btnContinue.text = state.HasSave
                ? string.Format(LocalizationKeys.Get(LocalizationKeys.BUTTON_CONTINUE_DAY_FORMAT), state.SaveDay)
                : LocalizationKeys.Get(LocalizationKeys.BUTTON_CONTINUE);

            _settingsTabs.value = (int)state.ActiveTab;
            _tabAudio.style.display = state.ActiveTab == MainMenuSettingsTab.Audio ? DisplayStyle.Flex : DisplayStyle.None;
            _tabVideo.style.display = state.ActiveTab == MainMenuSettingsTab.Video ? DisplayStyle.Flex : DisplayStyle.None;
            _tabControls.style.display = state.ActiveTab == MainMenuSettingsTab.Controls ? DisplayStyle.Flex : DisplayStyle.None;
            _tabAccessibility.style.display = state.ActiveTab == MainMenuSettingsTab.Accessibility ? DisplayStyle.Flex : DisplayStyle.None;

            var settings = state.Settings;
            _sliderMasterVolume.value = settings.MasterVolume;
            _sliderMusicVolume.value = settings.MusicVolume;
            _sliderSfxVolume.value = settings.SfxVolume;
            _sliderAmbientVolume.value = settings.AmbientVolume;
            _sliderUiScale.value = settings.UiScale;
            _toggleSubtitles.value = settings.Subtitles;
            _toggleVSync.value = settings.VSync;
            _toggleCameraShake.value = settings.CameraShake;
            _toggleMotionBlur.value = settings.MotionBlur;
            _toggleInvertY.value = settings.InvertY;
            _toggleGamepadVibration.value = settings.GamepadVibration;
            _toggleHighContrast.value = settings.HighContrast;
            _toggleReduceMotion.value = settings.ReduceMotion;
            _toggleColorblindCues.value = settings.ColorblindCues;
            _dropdownTextSize.value = settings.TextSize;
            _dropdownInputDevice.value = settings.InputDevice;
            _dropdownResolution.value = settings.Resolution;
            _dropdownWindowMode.value = settings.WindowMode;
            _dropdownQuality.value = settings.Quality;

            _root.style.scale = new StyleScale(new Vector3(settings.UiScale, settings.UiScale, 1f));

            if (_valueMasterVolume != null)
            {
                _valueMasterVolume.text = $"{Mathf.RoundToInt(settings.MasterVolume * 100f)}%";
            }

            if (_valueMusicVolume != null)
            {
                _valueMusicVolume.text = $"{Mathf.RoundToInt(settings.MusicVolume * 100f)}%";
            }

            if (_valueSfxVolume != null)
            {
                _valueSfxVolume.text = $"{Mathf.RoundToInt(settings.SfxVolume * 100f)}%";
            }

            if (_valueAmbientVolume != null)
            {
                _valueAmbientVolume.text = $"{Mathf.RoundToInt(settings.AmbientVolume * 100f)}%";
            }

            if (_valueUiScale != null)
            {
                _valueUiScale.text = $"{settings.UiScale:0.0}x";
            }

            ApplyDialogLocalization(state.ScreenState);
            ApplyFocusForState(state.ScreenState);
        }

        private void ApplyDialogLocalization(MainMenuScreenState screenState)
        {
            switch (screenState)
            {
                case MainMenuScreenState.NewGameConfirm:
                    _dialogTitle.text = LocalizationKeys.Get(LocalizationKeys.NEWGAME_TITLE);
                    _dialogMessage.text = LocalizationKeys.Get(LocalizationKeys.NEWGAME_MESSAGE);
                    _btnConfirmDialog.text = LocalizationKeys.Get(LocalizationKeys.NEWGAME_CONFIRM);
                    _btnCancelDialog.text = LocalizationKeys.Get(LocalizationKeys.QUIT_CANCEL);
                    break;
                case MainMenuScreenState.VideoApplyConfirm:
                    _dialogTitle.text = LocalizationKeys.Get(LocalizationKeys.VIDEO_APPLY_TITLE);
                    _dialogMessage.text = LocalizationKeys.Get(LocalizationKeys.VIDEO_APPLY_MESSAGE);
                    _btnConfirmDialog.text = LocalizationKeys.Get(LocalizationKeys.VIDEO_APPLY_CONFIRM);
                    _btnCancelDialog.text = LocalizationKeys.Get(LocalizationKeys.VIDEO_APPLY_CANCEL);
                    break;
                default:
                    _dialogTitle.text = LocalizationKeys.Get(LocalizationKeys.QUIT_TITLE);
                    _dialogMessage.text = LocalizationKeys.Get(LocalizationKeys.QUIT_MESSAGE);
                    _btnConfirmDialog.text = LocalizationKeys.Get(LocalizationKeys.QUIT_CONFIRM);
                    _btnCancelDialog.text = LocalizationKeys.Get(LocalizationKeys.QUIT_CANCEL);
                    break;
            }
        }

        private void OnDialogConfirm()
        {
            var state = _latestState;
            if (state == null)
            {
                return;
            }

            switch (state.ScreenState)
            {
                case MainMenuScreenState.NewGameConfirm:
                    _mainMenuService.ConfirmNewGameOverwrite();
                    break;
                case MainMenuScreenState.VideoApplyConfirm:
                    _mainMenuService.ConfirmVideoApply();
                    break;
                default:
                    _mainMenuService.ConfirmQuit();
                    break;
            }
        }

        private void OnDialogCancel()
        {
            var state = _latestState;
            if (state == null)
            {
                return;
            }

            if (state.ScreenState == MainMenuScreenState.VideoApplyConfirm)
            {
                _mainMenuService.CancelVideoApply();
                return;
            }

            _mainMenuService.CancelDialog();
        }

        private void ApplyLocalizedText()
        {
            _labelTitle.text = LocalizationKeys.Get(LocalizationKeys.TITLE);
            _labelSubtitle.text = LocalizationKeys.Get(LocalizationKeys.SUBTITLE);
            _labelVersion.text = LocalizationKeys.Get(LocalizationKeys.VERSION);
            _btnNewGame.text = LocalizationKeys.Get(LocalizationKeys.BUTTON_NEWGAME);
            _btnSettings.text = LocalizationKeys.Get(LocalizationKeys.BUTTON_SETTINGS);
            _btnCredits.text = LocalizationKeys.Get(LocalizationKeys.BUTTON_CREDITS);
            _btnQuit.text = LocalizationKeys.Get(LocalizationKeys.BUTTON_QUIT);
            _settingsTitle.text = LocalizationKeys.Get(LocalizationKeys.SETTINGS_TITLE);
            _creditsTitle.text = LocalizationKeys.Get(LocalizationKeys.CREDITS_TITLE);
            _btnCloseSettings.text = LocalizationKeys.Get(LocalizationKeys.SETTINGS_CLOSE);
            _btnBackCredits.text = LocalizationKeys.Get(LocalizationKeys.CREDITS_BACK);

            SetLabelText("tab-audio", LocalizationKeys.Get(LocalizationKeys.TAB_AUDIO));
            SetLabelText("tab-video", LocalizationKeys.Get(LocalizationKeys.TAB_VIDEO));
            SetLabelText("tab-controls", LocalizationKeys.Get(LocalizationKeys.TAB_CONTROLS));
            SetLabelText("tab-accessibility", LocalizationKeys.Get(LocalizationKeys.TAB_ACCESSIBILITY));
            SetLabelText("label-master-volume", LocalizationKeys.Get(LocalizationKeys.LABEL_MASTER_VOLUME));
            SetLabelText("label-music-volume", LocalizationKeys.Get(LocalizationKeys.LABEL_MUSIC_VOLUME));
            SetLabelText("label-sfx-volume", LocalizationKeys.Get(LocalizationKeys.LABEL_SFX_VOLUME));
            SetLabelText("label-ambient-volume", LocalizationKeys.Get(LocalizationKeys.LABEL_AMBIENT_VOLUME));
            SetLabelText("label-subtitles", LocalizationKeys.Get(LocalizationKeys.LABEL_SUBTITLES));
            SetLabelText("label-text-size", LocalizationKeys.Get(LocalizationKeys.LABEL_TEXT_SIZE));
            SetLabelText("label-resolution", LocalizationKeys.Get(LocalizationKeys.LABEL_RESOLUTION));
            SetLabelText("label-window-mode", LocalizationKeys.Get(LocalizationKeys.LABEL_WINDOW_MODE));
            SetLabelText("label-quality", LocalizationKeys.Get(LocalizationKeys.LABEL_QUALITY));
            SetLabelText("label-vsync", LocalizationKeys.Get(LocalizationKeys.LABEL_VSYNC));
            SetLabelText("label-camera-shake", LocalizationKeys.Get(LocalizationKeys.LABEL_CAMERA_SHAKE));
            SetLabelText("label-motion-blur", LocalizationKeys.Get(LocalizationKeys.LABEL_MOTION_BLUR));
            SetLabelText("label-ui-scale", LocalizationKeys.Get(LocalizationKeys.LABEL_UI_SCALE));
            SetLabelText("label-input-device", LocalizationKeys.Get(LocalizationKeys.LABEL_INPUT_DEVICE));
            SetLabelText("label-invert-y", LocalizationKeys.Get(LocalizationKeys.LABEL_INVERT_Y));
            SetLabelText("label-gamepad-vibration", LocalizationKeys.Get(LocalizationKeys.LABEL_GAMEPAD_VIBRATION));
            SetLabelText("label-high-contrast", LocalizationKeys.Get(LocalizationKeys.LABEL_HIGH_CONTRAST));
            SetLabelText("label-reduce-motion", LocalizationKeys.Get(LocalizationKeys.LABEL_REDUCE_MOTION));
            SetLabelText("label-colorblind-cues", LocalizationKeys.Get(LocalizationKeys.LABEL_COLORBLIND_CUES));
        }

        private void ApplyFocusForState(MainMenuScreenState screenState)
        {
            if (screenState == _lastRenderedScreen)
            {
                return;
            }

            _lastRenderedScreen = screenState;
            switch (screenState)
            {
                case MainMenuScreenState.MainMenu:
                    _btnNewGame?.Focus();
                    break;
                case MainMenuScreenState.Settings:
                    _btnCloseSettings?.Focus();
                    break;
                case MainMenuScreenState.Credits:
                    _btnBackCredits?.Focus();
                    break;
                default:
                    _btnCancelDialog?.Focus();
                    break;
            }
        }

        private void SetLabelText(string elementName, string text)
        {
            var label = _root.Q<Label>(elementName);
            if (label != null)
            {
                label.text = text;
                return;
            }

            var button = _root.Q<Button>(elementName);
            if (button != null)
            {
                button.text = text;
                return;
            }

            var radioButton = _root.Q<RadioButton>(elementName);
            if (radioButton != null)
            {
                radioButton.label = text;
            }
        }
    }
}
