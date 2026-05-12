namespace SolarPhobia.Application.Resources
{
    public interface IResourceEffectApplier
    {
        void ApplyTeaEffect(string soulId, float scalingRatio, bool isPreferred);
        void ApplyIncenseEffect(string soulId, float scalingRatio, bool isPreferred);
        void ApplyOfferingEffect(string soulId, float scalingRatio, bool isPreferred);
        int GetCurrentHuongHoa();
        bool TrySpendHuongHoa(int amount);
    }
}

