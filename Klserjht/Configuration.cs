using Newtonsoft.Json;

namespace Klserjht
{
    class Configuration
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
