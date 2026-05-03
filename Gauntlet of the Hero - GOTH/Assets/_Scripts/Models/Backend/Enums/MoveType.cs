using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GauntletOfTheHero.Backend.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MoveType
    {
        Physical,
        Magic
    }
}