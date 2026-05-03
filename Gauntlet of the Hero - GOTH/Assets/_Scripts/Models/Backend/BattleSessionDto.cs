using System;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class BattleSessionDto
    {
        [JsonProperty("sessionId")]
        public Guid sessionId;

        [JsonProperty("hero")]
        public HeroDto hero;

        [JsonProperty("monster")]
        public MonsterDto monster;

        [JsonProperty("battleState")]
        public BattleStateDto battleState;
    }
}