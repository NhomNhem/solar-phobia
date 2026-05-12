using SolarPhobia.Shared.Scopes;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Composition
{
    /// <summary>
    /// Legacy root scope kept for existing scene/prefab references while the
    /// architecture migrates to explicit scope types under Composition/Scopes.
    /// </summary>
    public class CompositionLifetimeScope : LifetimeScope, IProjectScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
        }
    }
}
