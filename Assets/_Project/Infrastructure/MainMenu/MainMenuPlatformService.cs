using SolarPhobia.Application.MainMenu;
using UnityEngine;

namespace SolarPhobia.Infrastructure.MainMenu
{
    public sealed class MainMenuPlatformService : IMainMenuPlatformService
    {
        public int ScreenWidth => Screen.width;

        public int ScreenHeight => Screen.height;

        public int GetCurrentQualityLevel()
        {
            return QualitySettings.GetQualityLevel();
        }

        public string GetCurrentWindowModeName()
        {
            return Screen.fullScreenMode switch
            {
                FullScreenMode.ExclusiveFullScreen => "Fullscreen",
                FullScreenMode.FullScreenWindow => "Borderless",
                _ => "Windowed"
            };
        }

        public void ApplyMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

        public void ApplyVSync(bool enabled)
        {
            QualitySettings.vSyncCount = enabled ? 1 : 0;
        }

        public void ApplyResolution(int width, int height)
        {
            Screen.SetResolution(width, height, Screen.fullScreenMode);
        }

        public void ApplyWindowMode(string modeName)
        {
            Screen.fullScreenMode = modeName switch
            {
                "Fullscreen" => FullScreenMode.ExclusiveFullScreen,
                "Borderless" => FullScreenMode.FullScreenWindow,
                _ => FullScreenMode.Windowed
            };
        }

        public void ApplyQualityLevel(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
        }
    }
}
