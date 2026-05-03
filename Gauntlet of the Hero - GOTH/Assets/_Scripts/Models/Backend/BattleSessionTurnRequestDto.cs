using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class BattleSessionTurnRequestDto
    {
        [JsonProperty("selectedMoveId")]
        public long selectedMoveId;
    }
}