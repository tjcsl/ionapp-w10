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
        public char BlockName { get; set; }

        public static async Task<Collection<EighthSignup>> GetAsync(DateTime fromDate, int count) {
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
            var blocks = results.SelectMany(r => r).Select(v => v.GetObject()).Take(count);

        }
    }
}
