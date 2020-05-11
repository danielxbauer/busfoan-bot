﻿using System;
using System.Threading.Tasks;
using Statecharts.NET.Utilities.Time;

namespace BusfoanBot.Models
{
    public sealed class BotOptions
    {
        public TimeSpan AnswerTimeout { get; set; } = 180.Seconds();
        public Func<BotContext, Task> OnDone { get; set; }
    }
}
