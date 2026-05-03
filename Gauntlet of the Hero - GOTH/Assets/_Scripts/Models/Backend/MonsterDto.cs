using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class MonsterDto
    {
        [JsonProperty("id")]
        public long id;

        [JsonProperty("name")]
        public string name;

        [JsonProperty("difficultyRank")]
        public int difficultyRank;

        [JsonProperty("maxHealth")]
        public int maxHealth;

        [JsonProperty("attack")]
        public int attack;

        [JsonProperty("defense")]
        public int defense;

        [JsonProperty("magic")]
        public int magic;
    }
}