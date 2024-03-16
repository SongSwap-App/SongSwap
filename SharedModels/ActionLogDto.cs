using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedModels
{
    [Serializable]
    public class ActionLogDto
    {
        [JsonPropertyName("datetime")]
        public string DateTime { get; set; } = string.Empty;

        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("arguments")]
        public string? Arguments { get; set; }

        [JsonPropertyName("queryArguments")]
        public string? QueryArguments { get; set; }

        [JsonPropertyName("timeElapsed")]
        public double TimeElapsed { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

    }
}
