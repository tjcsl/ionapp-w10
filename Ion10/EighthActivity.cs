using System;
using System.Linq;
using Windows.Data.Json;

namespace Ion10 {
    public sealed class EighthActivity {
        public bool BothBlocks { get; set; }
        public bool Cancelled { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public bool Favorited { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int NumberSignedUp { get; set; }
        public bool OneADay { get; set; }
        public bool PreSign { get; set; }
        public bool Restricted { get; set; }
        public string[] Rooms { get; set; }
        public Uri Roster { get; set; }
        public bool Special { get; set; }
        public string[] Sponsors { get; set; }
        public bool Sticky { get; set; }

        public static EighthActivity FromJson(JsonObject json) {
            return new EighthActivity {
                BothBlocks = json.GetNamedBoolean("both_blocks"),
                Cancelled = json.GetNamedBoolean("cancelled"),
                Capacity = (int)json.GetNamedObject("roster").GetNamedNumber("capacity"),
                Description = json.GetNamedString("description"),
                Favorited = json.GetNamedBoolean("favorited"),
                ID = (int)json.GetNamedNumber("id"),
                Name = json.GetNamedString("name"),
                NumberSignedUp = (int)json.GetNamedObject("roster").GetNamedNumber("count"),
                OneADay = json.GetNamedBoolean("one_a_day"),
                PreSign = json.GetNamedBoolean("presign"),
                Restricted = json.GetNamedBoolean("restricted_for_user"),
                Rooms = json.GetNamedArray("rooms").Select(r => r.GetString()).ToArray(),
                Roster = new Uri(json.GetNamedObject("roster").GetNamedString("url")),
                Special = json.GetNamedBoolean("special"),
                Sponsors = json.GetNamedArray("sponsors").Select(s => s.GetString()).ToArray(),
                Sticky = json.GetNamedBoolean("sticky")
            };
        }
    }
}
