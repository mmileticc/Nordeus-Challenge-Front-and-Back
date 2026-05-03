using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunResolveEncounterRequestDto
    {
        [JsonProperty("heroWon")]
        public bool heroWon;
    }
}