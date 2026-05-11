using System;
using System.Collections.Generic;

namespace SolarPhobia.Presentation.MainMenu
{
    public enum Language
    {
        English,
        Vietnamese
    }

    public static class LocalizationKeys
    {
        // ── Main Menu ──────────────────────────────────
        public const string TITLE = "MAINMENU_TITLE";
        public const string SUBTITLE = "MAINMENU_SUBTITLE";
        public const string BUTTON_NEWGAME = "MAINMENU_BUTTON_NEWGAME";
        public const string BUTTON_CONTINUE = "MAINMENU_BUTTON_CONTINUE";
        public const string BUTTON_SETTINGS = "MAINMENU_BUTTON_SETTINGS";
        public const string BUTTON_CREDITS = "MAINMENU_BUTTON_CREDITS";
        public const string BUTTON_QUIT = "MAINMENU_BUTTON_QUIT";

        // ── Settings ──────────────────────────────────
        public const string SETTINGS_TITLE = "SETTINGS_TITLE";
        public const string SETTINGS_CLOSE = "SETTINGS_CLOSE";
        public const string TAB_AUDIO = "SETTINGS_TAB_AUDIO";
        public const string TAB_VIDEO = "SETTINGS_TAB_VIDEO";
        public const string TAB_CONTROLS = "SETTINGS_TAB_CONTROLS";
        public const string TAB_ACCESSIBILITY = "SETTINGS_TAB_ACCESSIBILITY";
        public const string LABEL_MASTER_VOLUME = "SETTINGS_LABEL_MASTER_VOLUME";
        public const string LABEL_MUSIC_VOLUME = "SETTINGS_LABEL_MUSIC_VOLUME";
        public const string LABEL_SFX_VOLUME = "SETTINGS_LABEL_SFX_VOLUME";
        public const string LABEL_AMBIENT_VOLUME = "SETTINGS_LABEL_AMBIENT_VOLUME";
        public const string LABEL_SUBTITLES = "SETTINGS_LABEL_SUBTITLES";
        public const string LABEL_TEXT_SIZE = "SETTINGS_LABEL_TEXT_SIZE";
        public const string LABEL_RESOLUTION = "SETTINGS_LABEL_RESOLUTION";
        public const string LABEL_WINDOW_MODE = "SETTINGS_LABEL_WINDOW_MODE";
        public const string LABEL_QUALITY = "SETTINGS_LABEL_QUALITY";
        public const string LABEL_VSYNC = "SETTINGS_LABEL_VSYNC";
        public const string LABEL_CAMERA_SHAKE = "SETTINGS_LABEL_CAMERA_SHAKE";
        public const string LABEL_MOTION_BLUR = "SETTINGS_LABEL_MOTION_BLUR";
        public const string LABEL_UI_SCALE = "SETTINGS_LABEL_UI_SCALE";
        public const string LABEL_INPUT_DEVICE = "SETTINGS_LABEL_INPUT_DEVICE";
        public const string LABEL_INVERT_Y = "SETTINGS_LABEL_INVERT_Y_AXIS";
        public const string LABEL_GAMEPAD_VIBRATION = "SETTINGS_LABEL_GAMEPAD_VIBRATION";
        public const string LABEL_HIGH_CONTRAST = "SETTINGS_LABEL_HIGH_CONTRAST";
        public const string LABEL_REDUCE_MOTION = "SETTINGS_LABEL_REDUCE_MOTION";
        public const string LABEL_COLORBLIND_CUES = "SETTINGS_LABEL_COLORBLIND_CUES";

        // ── Credits ──────────────────────────────────
        public const string CREDITS_TITLE = "CREDITS_TITLE";
        public const string CREDITS_BACK = "CREDITS_BUTTON_BACK";

        // ── Quit Dialog ───────────────────────────────
        public const string QUIT_TITLE = "QUIT_TITLE";
        public const string QUIT_MESSAGE = "QUIT_MESSAGE";
        public const string QUIT_CONFIRM = "QUIT_BUTTON_CONFIRM";
        public const string QUIT_CANCEL = "QUIT_BUTTON_CANCEL";
        public const string NEWGAME_TITLE = "NEWGAME_TITLE";
        public const string NEWGAME_MESSAGE = "NEWGAME_MESSAGE";
        public const string NEWGAME_CONFIRM = "NEWGAME_BUTTON_CONFIRM";
        public const string VIDEO_APPLY_TITLE = "VIDEO_APPLY_TITLE";
        public const string VIDEO_APPLY_MESSAGE = "VIDEO_APPLY_MESSAGE";
        public const string VIDEO_APPLY_CONFIRM = "VIDEO_APPLY_BUTTON_CONFIRM";
        public const string VIDEO_APPLY_CANCEL = "VIDEO_APPLY_BUTTON_CANCEL";

        // ── Version ──────────────────────────────────
        public const string VERSION = "MAINMENU_VERSION";
        public const string BUTTON_CONTINUE_DAY_FORMAT = "MAINMENU_BUTTON_CONTINUE_DAY_FORMAT";

        // ── Language ──────────────────────────────────
        private static Language _currentLanguage = Language.English;
        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnLanguageChanged?.Invoke();
                }
            }
        }

        public static event Action OnLanguageChanged;

        // ── Translations (English / Vietnamese) ─────────
        private static readonly Dictionary<string, Dictionary<Language, string>> _translations = 
            new Dictionary<string, Dictionary<Language, string>>();

        static LocalizationKeys()
        {
            // Main Menu
            _translations[TITLE] = CreateDict("Solar Phobia", "Solar Phobia");
            _translations[SUBTITLE] = CreateDict("Nắng Gắt", "Nắng Gắt");
            _translations[BUTTON_NEWGAME] = CreateDict("New Game", "Chơi Mới");
            _translations[BUTTON_CONTINUE] = CreateDict("Continue", "Tiếp Tục");
            _translations[BUTTON_SETTINGS] = CreateDict("Settings", "Cài Đặt");
            _translations[BUTTON_CREDITS] = CreateDict("Credits", "Ghi Danh");
            _translations[BUTTON_QUIT] = CreateDict("Quit", "Thoát");

            // Settings
            _translations[SETTINGS_TITLE] = CreateDict("Settings", "Cài Đặt");
            _translations[SETTINGS_CLOSE] = CreateDict("Close", "Đóng");
            _translations[TAB_AUDIO] = CreateDict("Audio", "Âm Thanh");
            _translations[TAB_VIDEO] = CreateDict("Video", "Hình Ảnh");
            _translations[TAB_CONTROLS] = CreateDict("Controls", "Điều Khiển");
            _translations[TAB_ACCESSIBILITY] = CreateDict("Accessibility", "Trợ Năng");
            _translations[LABEL_MASTER_VOLUME] = CreateDict("Master Volume", "Âm Lượng Chính");
            _translations[LABEL_MUSIC_VOLUME] = CreateDict("Music Volume", "Âm Lượng Nhạc");
            _translations[LABEL_SFX_VOLUME] = CreateDict("SFX Volume", "Âm Thanh Hiệu Ứng");
            _translations[LABEL_AMBIENT_VOLUME] = CreateDict("Ambient Volume", "Âm Thanh Môi Trường");
            _translations[LABEL_SUBTITLES] = CreateDict("Subtitles", "Phụ Đề");
            _translations[LABEL_TEXT_SIZE] = CreateDict("Text Size", "Cỡ Chữ");
            _translations[LABEL_RESOLUTION] = CreateDict("Resolution", "Độ Phân Giải");
            _translations[LABEL_WINDOW_MODE] = CreateDict("Window Mode", "Chế Độ Cửa Sổ");
            _translations[LABEL_QUALITY] = CreateDict("Quality", "Chất Lượng");
            _translations[LABEL_VSYNC] = CreateDict("V-Sync", "Đồng Bộ Dọc");
            _translations[LABEL_CAMERA_SHAKE] = CreateDict("Camera Shake", "Lắc Máy Quay");
            _translations[LABEL_MOTION_BLUR] = CreateDict("Motion Blur", "Nhòe Chuyển Động");
            _translations[LABEL_UI_SCALE] = CreateDict("UI Scaling", "Tỷ Lệ UI");
            _translations[LABEL_INPUT_DEVICE] = CreateDict("Input Device", "Thiết Bị Điều Khiển");
            _translations[LABEL_INVERT_Y] = CreateDict("Invert Y-Axis", "Đảo Trục Y");
            _translations[LABEL_GAMEPAD_VIBRATION] = CreateDict("Gamepad Vibration", "Rung Tay Cầm");
            _translations[LABEL_HIGH_CONTRAST] = CreateDict("High Contrast", "Tương Phản Cao");
            _translations[LABEL_REDUCE_MOTION] = CreateDict("Reduce Motion", "Giảm Chuyển Động");
            _translations[LABEL_COLORBLIND_CUES] = CreateDict("Colorblind Cues", "Hỗ Trợ Mù Màu");

            // Credits
            _translations[CREDITS_TITLE] = CreateDict("Credits", "Ghi Danh");
            _translations[CREDITS_BACK] = CreateDict("Back", "Quay Lại");

            // Quit Dialog
            _translations[QUIT_TITLE] = CreateDict("Quit to Desktop?", "Thoát Game?");
            _translations[QUIT_MESSAGE] = CreateDict("Your progress is auto-saved.", "Tiến trình đã được tự động lưu.");
            _translations[QUIT_CONFIRM] = CreateDict("Confirm Quit", "Xác Nhận Thoát");
            _translations[QUIT_CANCEL] = CreateDict("Cancel", "Hủy");
            _translations[NEWGAME_TITLE] = CreateDict("Start New Game?", "Bắt Đầu Chơi Mới?");
            _translations[NEWGAME_MESSAGE] = CreateDict("Existing progress will be overwritten.", "Tiến trình hiện tại sẽ bị ghi đè.");
            _translations[NEWGAME_CONFIRM] = CreateDict("Start New", "Chơi Mới");
            _translations[VIDEO_APPLY_TITLE] = CreateDict("Apply Video Changes?", "Áp Dụng Thay Đổi Hình Ảnh?");
            _translations[VIDEO_APPLY_MESSAGE] = CreateDict("Apply these display settings now?", "Áp dụng cài đặt hiển thị ngay bây giờ?");
            _translations[VIDEO_APPLY_CONFIRM] = CreateDict("Apply", "Áp Dụng");
            _translations[VIDEO_APPLY_CANCEL] = CreateDict("Revert", "Hoàn Tác");

            // Version
            _translations[VERSION] = CreateDict("v1.0.0", "v1.0.0");
            _translations[BUTTON_CONTINUE_DAY_FORMAT] = CreateDict("Continue (Day {0})", "Tiếp Tục (Ngày {0})");
        }

        private static Dictionary<Language, string> CreateDict(string english, string vietnamese)
        {
            return new Dictionary<Language, string>
            {
                { Language.English, english },
                { Language.Vietnamese, vietnamese }
            };
        }

        public static string Get(string key)
        {
            if (_translations.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(CurrentLanguage, out var text))
                    return text;
                // Fallback to English
                if (translations.TryGetValue(Language.English, out var englishText))
                    return englishText;
            }
            return key;
        }
    }
}
