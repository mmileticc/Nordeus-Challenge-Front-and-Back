using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class BattleSessionCreateRequestDto
    {
        [JsonProperty("heroId")]
        public long heroId;

        [JsonProperty("monsterId")]
        public long monsterId;

        [JsonProperty("heroMaxHealth")]
        public int? heroMaxHealth;

        [JsonProperty("heroCurrentHealth")]
        public int? heroCurrentHealth;

        [JsonProperty("heroAttack")]
        public int? heroAttack;

        [JsonProperty("heroDefense")]
        public int? heroDefense;

        [JsonProperty("heroMagic")]
        public int? heroMagic;

        [JsonProperty("heroMoveIds")]
        public List<long> heroMoveIds = new List<long>();
    }
}