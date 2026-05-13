namespace SolarPhobia.Application.Features.MainMenu
{
    public interface IMainMenuPlatformService
    {
        int ScreenWidth { get; }
        int ScreenHeight { get; }

        int GetCurrentQualityLevel();
        string GetCurrentWindowModeName();

        void ApplyMasterVolume(float value);
        void ApplyVSync(bool enabled);
        void ApplyResolution(int width, int height);
        void ApplyWindowMode(string modeName);
        void ApplyQualityLevel(int qualityIndex);
    }
}

