using Newtonsoft.Json;

namespace DiscordBot
{
    internal struct ConfigJSON
    {
        [JsonProperty("token")]
        internal string Token { get; private set; }

        [JsonProperty("prefix")]
        internal string Prefix { get; private set; }
    }
}
