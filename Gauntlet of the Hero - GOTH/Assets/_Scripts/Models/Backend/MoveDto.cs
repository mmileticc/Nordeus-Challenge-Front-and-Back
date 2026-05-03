using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class MoveDto
    {
        [JsonProperty("id")]
        public long id;

        [JsonProperty("name")]
        public string name;

        [JsonProperty("moveType")]
        public MoveType moveType;

        [JsonProperty("effectType")]
        public EffectType effectType;

        [JsonProperty("power")]
        public int power;

        [JsonProperty("buffAttribute")]
        public BuffAttribute? buffAttribute;

        [JsonProperty("buffTarget")]
        public BuffTarget? buffTarget;

        [JsonProperty("buffAmount")]
        public int buffAmount;

        [JsonProperty("buffDuration")]
        public int buffDuration;
    }
}