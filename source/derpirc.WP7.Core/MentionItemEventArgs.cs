﻿using System;
using derpirc.Data;

namespace derpirc.Core
{
    public class MentionItemEventArgs : EventArgs
    {
        public MentionSummary Mention { get; set; }
        public MentionItem Message { get; set; }
    }
}
