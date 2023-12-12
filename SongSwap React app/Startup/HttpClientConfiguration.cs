namespace SongSwap_React_app.Startup
{
    public static class HttpClientConfiguration
    {
        public static IServiceCollection AddCofiguredHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("SongSwapClient", client =>
            {
                client.BaseAddress = new Uri("https://api.musicapi.com/api/");
                client.Timeout = TimeSpan.FromSeconds(30);

                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });


            return services;
        }
    }
}
