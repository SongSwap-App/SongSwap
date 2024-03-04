namespace LoggingApp.Infrastructure
{
    public class ActionLog
    {
        private readonly DateTime dateTime;

        public string DateTime { get => dateTime.ToString("MM/dd/yyyy hh:mm:ss"); }

        public string Action { get; }

        public string? Arguments { get; }

        public string? QueryArguments { get; }

        public double TimeElapsed { get; }

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
