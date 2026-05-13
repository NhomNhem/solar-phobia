using SolarPhobia.Domain;

namespace SolarPhobia.Domain.Services
{
    public static class DialogueBranchResolver
    {
        public static string ResolveTarget(Choice choice)
        {
            return choice?.Target;
        }
    }
}

