using System;
using UnityEngine;

namespace SolarPhobia.Presentation.MainMenu
{
    public static class PlayerPrefsKeys
    {
        public const string MASTER_VOLUME = "Settings.MasterVolume";
        public const string MUSIC_VOLUME = "Settings.MusicVolume";
        public const string SFX_VOLUME = "Settings.SFXVolume";
        public const string AMBIENT_VOLUME = "Settings.AmbientVolume";
        public const string SUBTITLES_ENABLED = "Settings.SubtitlesEnabled";
        public const string TEXT_SIZE = "Settings.TextSize";
        public const string RESOLUTION_WIDTH = "Settings.ResolutionWidth";
        public const string RESOLUTION_HEIGHT = "Settings.ResolutionHeight";
        public const string WINDOW_MODE = "Settings.WindowMode";
        public const string QUALITY_LEVEL = "Settings.QualityLevel";
        public const string V_SYNC = "Settings.VSync";
        public const string CAMERA_SHAKE = "Settings.CameraShake";
        public const string MOTION_BLUR = "Settings.MotionBlur";
        public const string UI_SCALE = "Settings.UIScale";
        public const string INPUT_DEVICE = "Settings.InputDevice";
        public const string INVERT_Y_AXIS = "Settings.InvertYAxis";
        public const string GAMEPAD_VIBRATION = "Settings.GamepadVibration";
        public const string HAS_SAVE = "SaveData.hasSave";
        public const string SAVE_DAY_NUMBER = "SaveData.dayNumber";

        public static class Defaults
        {
            public const float MASTER_VOLUME = 0.75f;
            public const float MUSIC_VOLUME = 0.60f;
            public const float SFX_VOLUME = 0.80f;
            public const float AMBIENT_VOLUME = 0.50f;
            public const bool SUBTITLES_ENABLED = true;
            public const string TEXT_SIZE = "Medium";
            public const int RESOLUTION_WIDTH = 1920;
            public const int RESOLUTION_HEIGHT = 1080;
            public const string WINDOW_MODE = "Fullscreen";
            public const int QUALITY_LEVEL = 3;
            public const bool V_SYNC = true;
            public const bool CAMERA_SHAKE = true;
            public const bool MOTION_BLUR = false;
            public const float UI_SCALE = 1.0f;
            public const string INPUT_DEVICE = "Keyboard/Mouse";
            public const bool INVERT_Y_AXIS = false;
            public const bool GAMEPAD_VIBRATION = true;
        }
    }

    public static class SettingsDefaults
    {
        public static string[] RESOLUTIONS = new[]
        {
            "1920x1080", "1600x900", "1366x768", "1280x720", "2560x1440", "3840x2160"
        };

        public static string[] WINDOW_MODES = new[]
        {
            "Fullscreen", "Windowed", "Borderless"
        };

        public static string[] QUALITY_LEVELS = new[]
        {
            "Low", "Medium", "High", "Ultra", "Fantastic", "Maximum"
        };

        public static string[] TEXT_SIZES = new[]
        {
            "Small", "Medium", "Large"
        };

        public static string[] INPUT_DEVICES = new[]
        {
            "Keyboard/Mouse", "Gamepad"
        };
    }
}