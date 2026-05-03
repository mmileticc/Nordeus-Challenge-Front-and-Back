using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunStartRequestDto
    {
        [JsonProperty("heroId")]
        public long? heroId;
    }
}