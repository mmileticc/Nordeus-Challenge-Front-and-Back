using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class ActiveBuffDto
    {
        [JsonProperty("targetSide")]
        public CharacterSide targetSide;

        [JsonProperty("attribute")]
        public BuffAttribute attribute;

        [JsonProperty("amount")]
        public int amount;

        [JsonProperty("remainingTurns")]
        public int remainingTurns;
    }
}