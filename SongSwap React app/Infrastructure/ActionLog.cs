namespace SongSwap_React_app.Infrastructure
{
    public class ActionLog
    {
        private readonly DateTime dateTime;

        public string DateTime { get => dateTime.ToString("MM/dd/yyyy hh:mm:ss"); }

        public string Action { get; }

        public string? Arguments { get; }

        public double TimeElapsed { get; }



        public ActionLog(DateTime dateTime, string action, string? arguments, double elapsed)
        {
            this.dateTime = dateTime;
            Action = action;
            TimeElapsed = elapsed;
            Arguments = arguments;
        }
    }
}
