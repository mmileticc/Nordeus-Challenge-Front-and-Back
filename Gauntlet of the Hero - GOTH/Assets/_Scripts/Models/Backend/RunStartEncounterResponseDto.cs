using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunStartEncounterResponseDto
    {
        [JsonProperty("run")]
        public RunConfigDto run;

        [JsonProperty("battleSession")]
        public BattleSessionDto battleSession;
    }
}