namespace SolarPhobia.Rules {
    public static class ScenePaths {
        public const string ProjectRoot = "Assets/_Project";
        
        public static class Scenes {
            public const string Root = "Assets/_Project/_Scenes";
            public const string Dev = "Assets/_Project/_Scenes/Dev";
            public const string Gameplay = "Assets/_Project/_Scenes/Gameplay";
            public const string DevPrototype = "Assets/_Project/_Scenes/Dev/Prototype";
            public const string DevDialogue = "Assets/_Project/_Scenes/Dev/Dialogue";
        }
        
        public static class Settings {
            public const string Root = "Assets/_Project/Settings";
            public const string SceneTemplates = "Assets/_Project/Settings/Scenes";
        }
        
        public static class Deprecated {
            public const string OldScenesRoot = "Assets/Scenes";
        }
        
        public static class BuildSettings {
            public const string GameplayScenesKey = "Gameplay";
            public const string DevScenesKey = "Dev";
        }
    }
    
    public static class SceneNames {
        public const string PrototypeTest = "Prototype Test Scene";
    }
}