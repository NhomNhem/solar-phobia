namespace SolarPhobia.Rules {
    public static class NamingConventions {
        public const string RootNamespace = "SolarPhobia";
        
        public const string DomainNamespace = "SolarPhobia.Domain";
        public const string ApplicationNamespace = "SolarPhobia.Application";
        public const string InfrastructureNamespace = "SolarPhobia.Infrastructure";
        public const string PresentationNamespace = "SolarPhobia.Presentation";
        public const string SharedNamespace = "SolarPhobia.Shared";
        
        public static class Layers {
            public const string Domain = "Domain";
            public const string Application = "Application";
            public const string Infrastructure = "Infrastructure";
            public const string Presentation = "Presentation";
            public const string Shared = "Shared";
        }
        
        public static class Suffixes {
            public const string Service = "Service";
            public const string Repository = "Repository";
            public const string UseCase = "UseCase";
            public const string Entity = "Entity";
            public const string ValueObject = "ValueObject";
            public const string Controller = "Controller";
            public const string View = "View";
            public const string Installer = "Installer";
        }
        
        public static class Prefixes {
            public const string Interface = "I";
            public const string Abstract = "Abstract";
        }
    }
}