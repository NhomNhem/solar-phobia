namespace SolarPhobia.Rules {
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = false)]
    public class LayerAttribute : System.Attribute {
        public string LayerName { get; }
        
        public LayerAttribute(string layerName) {
            LayerName = layerName;
        }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class DomainLayerAttribute : LayerAttribute {
        public DomainLayerAttribute() : base(NamingConventions.Layers.Domain) { }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class ApplicationLayerAttribute : LayerAttribute {
        public ApplicationLayerAttribute() : base(NamingConventions.Layers.Application) { }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class InfrastructureLayerAttribute : LayerAttribute {
        public InfrastructureLayerAttribute() : base(NamingConventions.Layers.Infrastructure) { }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class PresentationLayerAttribute : LayerAttribute {
        public PresentationLayerAttribute() : base(NamingConventions.Layers.Presentation) { }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false)]
    public class NoAwakeStartUpdateRuleAttribute : System.Attribute {
        public string Reason { get; }
        
        public NoAwakeStartUpdateRuleAttribute(string reason = "Use VContainer IInitializable/ITickable instead") {
            Reason = reason;
        }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Constructor | System.AttributeTargets.Method, AllowMultiple = false)]
    public class ConstructorInjectionRequiredAttribute : System.Attribute {
        public ConstructorInjectionRequiredAttribute() { }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class ReactivePropertyRequiredAttribute : System.Attribute {
        public ReactivePropertyRequiredAttribute() { }
    }
}