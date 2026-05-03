using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class RunConfigDto
    {
        [JsonProperty("runId")]
        public Guid runId;

        [JsonProperty("hero")]
        public RunHeroProgressDto hero;

        [JsonProperty("encounters")]
        public List<RunEncounterDto> encounters = new List<RunEncounterDto>();

        [JsonProperty("learnedMoves")]
        public List<MoveDto> learnedMoves = new List<MoveDto>();

        [JsonProperty("equippedMoveIds")]
        public List<long> equippedMoveIds = new List<long>();

        [JsonProperty("completed")]
        public bool completed;
    }
}