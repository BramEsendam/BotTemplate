﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Bot.BotLists.Interfaces.Services;
using Discord.WebSocket;

namespace Bot.Discord.Timers
{
    public class DiscordBotListsUpdateTimer
    {
        private readonly DiscordShardedClient _client;
        private readonly IBotListUpdater _botListUpdater;

        public DiscordBotListsUpdateTimer(DiscordShardedClient client, IBotListUpdater botListUpdater)
        {
            _client = client;
            _botListUpdater = botListUpdater;
        }

        private Timer _timer;
        internal Task TimerAsync()
        {
            _timer = new Timer
            {
                Interval = TimeSpan.FromMinutes(Constants.BotListUpdateMinutes).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += TimerElapsed;
            return Task.CompletedTask;
        }

        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateStatsAsync().ConfigureAwait(false);
        }

        private async Task UpdateStatsAsync()
        {
            var guildCountArray = _client.Shards.Select(x => x.Guilds.Count).ToArray();
            var shardIdArray = _client.Shards.Select(x => x.ShardId).ToArray();
            await _botListUpdater.UpdateStatusAsync(_client.CurrentUser.Id, _client.Shards.Count, guildCountArray, shardIdArray).ConfigureAwait(false);
        }
    }
}
