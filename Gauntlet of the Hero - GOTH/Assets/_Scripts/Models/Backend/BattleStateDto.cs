using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class BattleStateDto
    {
        [JsonProperty("heroId")]
        public long heroId;

        [JsonProperty("monsterId")]
        public long monsterId;

        [JsonProperty("selectedMoveId")]
        public long? selectedMoveId;

        [JsonProperty("heroMaxHealth")]
        public int? heroMaxHealth;

        [JsonProperty("heroMoveIds")]
        public List<long> heroMoveIds = new List<long>();

        [JsonProperty("heroCurrentHealth")]
        public int? heroCurrentHealth;

        [JsonProperty("heroAttack")]
        public int? heroAttack;

        [JsonProperty("heroDefense")]
        public int? heroDefense;

        [JsonProperty("heroMagic")]
        public int? heroMagic;

        [JsonProperty("monsterCurrentHealth")]
        public int? monsterCurrentHealth;

        [JsonProperty("monsterAttack")]
        public int? monsterAttack;

        [JsonProperty("monsterDefense")]
        public int? monsterDefense;

        [JsonProperty("monsterMagic")]
        public int? monsterMagic;

        [JsonProperty("activeBuffs")]
        public List<ActiveBuffDto> activeBuffs = new List<ActiveBuffDto>();
    }
}