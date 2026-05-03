using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class HeroDto
    {
        [JsonProperty("id")]
        public long id;

        [JsonProperty("name")]
        public string name;

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

        [JsonProperty("moves")]
        public List<MoveDto> moves = new List<MoveDto>();
    }
}