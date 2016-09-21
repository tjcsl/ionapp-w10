// The MIT License (MIT) 
// 
// Copyright (c) 2016 Ion Native App Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Newtonsoft.Json;

namespace Ion10.Models {
    public sealed class EighthActivityForBlock {
        [JsonProperty("comments")]
        public string Comments { get; private set; }

        [JsonProperty("name_with_flags_for_user")]
        public string NameWithFlagsForUser { get; private set; }

        [JsonProperty("sticky")]
        public bool Sticky { get; private set; }

        [JsonProperty("url")]
        public Uri Uri { get; private set; }

        [JsonProperty("cancelled")]
        public bool Cancelled { get; private set; }

        [JsonProperty("one_a_day")]
        public bool OneADay { get; private set; }

        [JsonProperty("special")]
        public bool Special { get; private set; }

        [JsonProperty("roster")]
        public EighthActivityRoster Roster { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("both_blocks")]
        public bool BothBlocks { get; private set; }

        [JsonProperty("aid")]
        public int ActivityId { get; private set; }

        [JsonProperty("sponsors")]
        public string[] Sponsors { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("display_text")]
        public string DisplayText { get; private set; }

        [JsonProperty("favorited")]
        public bool Favorited { get; private set; }

        [JsonProperty("restricted")]
        public bool Restricted { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("presign")]
        public bool Presign { get; private set; }

        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("restricted_for_user")]
        public bool RestrictedForUser { get; private set; }

        [JsonProperty("administrative")]
        public bool Administrative { get; private set; }

        [JsonProperty("rooms")]
        public string[] Rooms { get; private set; }
    }
}
