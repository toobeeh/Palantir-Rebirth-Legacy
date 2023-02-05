using DSharpPlus;
using DSharpPlus.Entities;
using Palantir_Rebirth.Data.JSON;
using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Lobbies
{
    internal class LobbiesService
    {
        private DiscordClient client;
        private List<DiscordMessage> messages;
        private ObservedGuild config;
        private GuildSettings settings;
        private Thread thread;

        public bool Active { get; private set; }

        public LobbiesService(DiscordClient client, PalantirEntity guild) 
        {
            this.client = client;
            messages = new List<DiscordMessage>();
            Active = false;
            config = JSONUtils.FromString<ObservedGuild>(guild.Palantir);
            thread = new Thread(new ThreadStart(UpdateLoop));

            var settingsResult = Program.PalantirDb.Query(db => db.GuildSettings.Where(s => s.GuildID == config.GuildID));
            if(settingsResult.Count() > 0)
            {
                settings = JSONUtils.FromString<GuildSettings>(settingsResult[0].Settings);
            }
            else
            {
                settings = new GuildSettings
                {
                    Header = "```fix\nCurrently playing skribbl.io```",
                    IdleMessage = "Seems like no-one is playing :( \nAsk some friends to join or go solo!\n\n ",
                    Timezone = 0,
                    ShowAnimatedEmojis = true,
                    ShowRefreshed = false,
                    ShowToken = true
                };
            }
        }

        public async Task<bool> Start()
        {
            // get first message
            try
            {
                var channel = await client.GetChannelAsync(Convert.ToUInt64(config.ChannelID));
                var message = await channel.GetMessageAsync(Convert.ToUInt64(config.MessageID));
                messages.Add(message);
            }
            catch(Exception ex)
            {
                Logger.Error("Could not get first guild msg", ex);
                return false;
            }

            // get other messages
            var next = new List<DiscordMessage>(await messages[0].Channel.GetMessagesAfterAsync(messages[0].Id, 10));
            while (next.Count > 0)
            {
                var current = next[0];
                if (current.Author.Id == client.CurrentUser.Id) messages.Add(current);
                //else break;
                next.RemoveAt(0);
            }

            thread.Start();
            Active = true;
            return true;
        }

        public void Stop()
        {
            Active = false;
        }

        private async void UpdateLoop()
        {
            while (Active)
            {

            }
        }
    }
}
