using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunHeroProgressDto
    {
        [JsonProperty("heroId")]
        public long heroId;

        [JsonProperty("heroName")]
        public string heroName;

        [JsonProperty("level")]
        public int level;

        [JsonProperty("xp")]
        public int xp;

        [JsonProperty("xpToNextLevel")]
        public int xpToNextLevel;

        [JsonProperty("maxHealth")]
        public int maxHealth;

        [JsonProperty("currentHealth")]
        public int currentHealth;

        [JsonProperty("attack")]
        public int attack;

        [JsonProperty("defense")]
        public int defense;

        [JsonProperty("magic")]
        public int magic;
    }
}