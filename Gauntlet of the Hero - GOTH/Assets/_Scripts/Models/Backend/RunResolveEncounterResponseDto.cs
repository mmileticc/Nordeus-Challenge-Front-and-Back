using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunResolveEncounterResponseDto
    {
        [JsonProperty("run")]
        public RunConfigDto run;

        [JsonProperty("learnedMove")]
        public MoveDto learnedMove;

        [JsonProperty("xpGained")]
        public int xpGained;

        [JsonProperty("levelsGained")]
        public int levelsGained;
    }
}