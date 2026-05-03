using System.Collections.Generic;
using Newtonsoft.Json;

namespace GauntletOfTheHero.Backend.Models
{
    [System.Serializable]
    public class BackendErrorResponseDto
    {
        [JsonProperty("timestamp")]
        public string timestamp;

        [JsonProperty("status")]
        public int status;

        [JsonProperty("error")]
        public string error;

        [JsonProperty("code")]
        public string code;

        [JsonProperty("message")]
        public string message;

        [JsonProperty("path")]
        public string path;

        [JsonProperty("details")]
        public List<string> details = new List<string>();
    }
}