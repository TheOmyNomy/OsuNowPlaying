using Newtonsoft.Json;

namespace Klserjht.Updater
{
    class Version
    {
        [JsonIgnore]
        public readonly int Major, Minor, Build, Revision;

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        public Version(string tagName)
        {
            TagName = tagName;

            var tokens = TagName.Split('.');
            if (tokens.Length != 4) return;

            Major = int.Parse(tokens[0]);
            Minor = int.Parse(tokens[1]);
            Build = int.Parse(tokens[2]);
            Revision = int.Parse(tokens[3]);
        }

        public override string ToString()
        {
            return TagName;
        }

        public static bool operator ==(Version left, Version right)
        {
            return left?.TagName == right?.TagName;
        }

        public static bool operator !=(Version left, Version right)
        {
            return left?.TagName != right?.TagName;
        }
    }
}
