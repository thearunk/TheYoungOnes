using System;
using Newtonsoft.Json;

namespace TheYoungOnes
{
    [JsonObject]
    public class PartialUserList
    {
        public PartialUserList()
        {
        }

        [JsonProperty("result")]
        public int[] UserIds { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        public bool HasMoreUsers()
        {
            return !string.IsNullOrEmpty(Token);
        }
    }
}
