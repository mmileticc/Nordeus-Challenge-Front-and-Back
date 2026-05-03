using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunEncounterDto
    {
        [JsonProperty("encounterIndex")]
        public int encounterIndex;

        [JsonProperty("monsterId")]
        public long monsterId;

        [JsonProperty("monsterName")]
        public string monsterName;

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

        [JsonProperty("moves")]
        public List<MoveDto> moves = new List<MoveDto>();

        [JsonProperty("defeated")]
        public bool defeated;

        [JsonProperty("unlocked")]
        public bool unlocked;
    }
}