using R3;

namespace SolarPhobia.Application.MainMenu
{
    public interface IMainMenuApplicationService
    {
        ReadOnlyReactiveProperty<MainMenuUiState> CurrentState { get; }
        Observable<Unit> OnStartNewGameRequested { get; }
        Observable<Unit> OnContinueRequested { get; }
        Observable<Unit> OnQuitConfirmed { get; }

        void Initialize();
        void RequestNewGame();
        void ConfirmNewGameOverwrite();
        void RequestContinue();
        void OpenSettings();
        void OpenCredits();
        void RequestQuit();
        void ConfirmQuit();
        void CancelDialog();
        void Back();
        void SetSettingsTab(int index);
        void SetMasterVolume(float value);
        void SetMusicVolume(float value);
        void SetSfxVolume(float value);
        void SetAmbientVolume(float value);
        void SetUiScale(float value);
        void SetSubtitles(bool value);
        void SetVSync(bool value);
        void SetCameraShake(bool value);
        void SetMotionBlur(bool value);
        void SetInvertY(bool value);
        void SetGamepadVibration(bool value);
        void SetTextSize(string value);
        void SetInputDevice(string value);
        void SetResolution(string value);
        void SetWindowMode(string value);
        void SetQuality(string value);
        void SetHighContrast(bool value);
        void SetReduceMotion(bool value);
        void SetColorblindCues(bool value);
        void ConfirmVideoApply();
        void CancelVideoApply();
    }
}
