using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GauntletOfTheHero.Backend.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BuffAttribute
    {
        Attack,
        Defense,
        Magic
    }
}