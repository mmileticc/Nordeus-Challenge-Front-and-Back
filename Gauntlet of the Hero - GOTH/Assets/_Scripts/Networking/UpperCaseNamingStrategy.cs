using Newtonsoft.Json.Serialization;

namespace GauntletOfTheHero.Backend.Networking
{
    public sealed class UpperCaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return name == null ? null : name.ToUpperInvariant();
        }
    }
}