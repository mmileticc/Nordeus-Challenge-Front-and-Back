using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class BattleTurnResultDto
    {
        [JsonProperty("heroId")]
        public long heroId;

        [JsonProperty("monsterId")]
        public long monsterId;

        [JsonProperty("heroCurrentHealth")]
        public int heroCurrentHealth;

        [JsonProperty("heroAttack")]
        public int heroAttack;

        [JsonProperty("heroDefense")]
        public int heroDefense;

        [JsonProperty("heroMagic")]
        public int heroMagic;

        [JsonProperty("monsterCurrentHealth")]
        public int monsterCurrentHealth;

        [JsonProperty("monsterAttack")]
        public int monsterAttack;

        [JsonProperty("monsterDefense")]
        public int monsterDefense;

        [JsonProperty("monsterMagic")]
        public int monsterMagic;

        [JsonProperty("heroMoveName")]
        public string heroMoveName;

        [JsonProperty("monsterMoveName")]
        public string monsterMoveName;

        [JsonProperty("battleFinished")]
        public bool battleFinished;

        [JsonProperty("winner")]
        public string winner;

        [JsonProperty("logs")]
        public List<string> logs = new List<string>();

        [JsonProperty("activeBuffs")]
        public List<ActiveBuffDto> activeBuffs = new List<ActiveBuffDto>();
    }
}