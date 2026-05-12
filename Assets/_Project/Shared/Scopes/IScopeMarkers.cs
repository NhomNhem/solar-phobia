namespace SolarPhobia.Shared.Scopes
{
    /// <summary>
    /// Marker interface for services that live for the whole application session.
    /// </summary>
    public interface IProjectScope
    {
    }

    /// <summary>
    /// Marker interface for gameplay runtime services tied to a run/session.
    /// </summary>
    public interface IRunScope
    {
    }

    /// <summary>
    /// Marker interface for UI composition and screen-local services.
    /// </summary>
    public interface IUIScope
    {
    }

    /// <summary>
    /// Marker interface for encounter/map-local services created beneath a run.
    /// </summary>
    public interface IEncounterScope
    {
    }
}
