using System;
using Newtonsoft.Json;

namespace Ion10.Models {
    public sealed class EighthActivityRoster {
        [JsonProperty("capacity")]
        public int Capacity { get; private set; }

        [JsonProperty("url")]
        public Uri Uri { get; private set; }

        [JsonProperty("count")]
        public int Count { get; private set; }
    }
}
