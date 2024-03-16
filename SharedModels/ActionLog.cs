using System.Text.Json.Serialization;

namespace SharedModels
{
    public class ActionLog
    {
        private readonly DateTime dateTime;

        [JsonPropertyName("datetime")]
        public string DateTime { get => dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss zzz"); }

        [JsonPropertyName("action")]
        public string Action { get; }

        [JsonPropertyName("arguments")]
        public string? Arguments { get; }

        [JsonPropertyName("queryArguments")]
        public string? QueryArguments { get; }

        [JsonPropertyName("timeElapsed")]
        public double TimeElapsed { get; }

        [JsonPropertyName("error")]
        public string? Error { get; }

        public ActionLog(DateTime dateTime, string action, string? arguments, double elapsed, string? error = null, string? queryArguments = null)
        {
            this.dateTime = dateTime;
            Action = action;
            TimeElapsed = elapsed;
            Arguments = arguments;
            Error = error;
            QueryArguments = queryArguments;
        }
    }
}
