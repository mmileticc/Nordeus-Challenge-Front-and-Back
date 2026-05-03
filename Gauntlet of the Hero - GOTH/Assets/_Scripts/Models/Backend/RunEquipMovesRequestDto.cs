using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunEquipMovesRequestDto
    {
        [JsonProperty("moveIds")]
        public List<long> moveIds = new List<long>();
    }
}