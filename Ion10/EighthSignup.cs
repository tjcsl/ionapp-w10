// Copyright (c) 2015, Joshua Cotton
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Ion10 {
    public sealed class EighthSignup {
        public int BlockID { get; set; }
        public int ActivityID { get; set; }
        public string ActivityTitle { get; set; }
        public DateTime Date { get; set; }
        public string BlockName { get; set; }

        public static async Task<Collection<EighthSignup>> GetAsync(DateTime fromDate, int count) {
#if false
            var results = new Collection<JsonArray>();
            var page = 1;
            var retrieved = 0;
            do {
                var json = JsonObject.Parse(await App.HttpClient.GetStringAsync(
                    new Uri($"/api/blocks?start_date={fromDate:yyyy-MM-dd}&page={page}")));
                retrieved += (int)json.GetNamedNumber("count");
                results.Add(json.GetNamedArray("results"));
                page++;
            } while(retrieved < count);
            var blocks = results
                .SelectMany(r => r)
                .Select(v => v.GetObject())
                .OrderBy(b => b.GetNamedString("date"))
                .ThenBy(b => b.GetNamedString("block_letter"))
                .Take(count);
#endif
            fromDate = fromDate.Date;
            var json = JsonArray
                .Parse(await App.HttpClient.GetStringAsync(new Uri("/api/signups/user?format=json")))
                .Select(o => o.GetObject())
                .Where(s => DateTime.Parse(s.GetNamedObject("block").GetNamedString("date")) >= fromDate)
                .Take(count);
            var results = new Collection<EighthSignup>();
            foreach(var signup in json) {
                var blockid = (int)signup.GetNamedObject("block").GetNamedNumber("id");
                var activity = signup.GetNamedObject("activity");
                results.Add(new EighthSignup {
                    ActivityID = (int)activity.GetNamedNumber("id"),
                    ActivityTitle = activity.GetNamedString("title"),
                    BlockID = blockid,
                    BlockName =
                        JsonObject.Parse(
                            await App.HttpClient.GetStringAsync(new Uri($"/api/blocks/{blockid}?format=json")))
                            .GetNamedString("block_letter")
                });
            }
            return results;
        }
    }
}
