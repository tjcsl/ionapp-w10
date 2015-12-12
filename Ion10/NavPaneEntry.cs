// Copyright (c) 2015, Joshua Cotton
// All rights reserved.

using System;
using Windows.UI.Xaml.Controls;

namespace Ion10 {
    public sealed class NavPaneEntry {
        public string Caption { get; set; }
        public string Glyph { get; set; } 
        public string FontFamily { get; set; }
        public Type Type { get; set; }
    }
}
