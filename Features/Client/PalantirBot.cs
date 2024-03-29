﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Palantir_Rebirth.Features.Lobbies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Palantir_Rebirth.Commands.Slash;
using DSharpPlus.Entities;

namespace Palantir_Rebirth.Features.Client
{
    internal class PalantirBot
    {
        private DiscordClient client;
        private readonly bool nightly;
        private SlashCommandsExtension slash;
        public InteractivityExtension Interactivity { private set; get; }

        public PalantirBot(string tokenPath, bool nightly)
        {
            string token = File.ReadAllText(tokenPath);
            this.nightly = nightly;

            client = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
            });

            var commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "." },
                DmHelp = false,
                IgnoreExtraArguments = true,
                CaseSensitive = false
            });

            slash = client.UseSlashCommands();
            slash.RegisterCommands<SpriteSlashCommands>(779435254225698827);

            slash.SlashCommandErrored += async (s, e) =>
            {
                await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Errored with <@{e.Exception.ToString()}> "));
            };

            Interactivity = client.UseInteractivity();
        }

        public async Task Connect()
        {
            await client.ConnectAsync();
        }

        public async Task SendDebugMessage(string message)
        {
            await (await client.GetChannelAsync(1071167977946353745)).SendMessageAsync(message);
        }

        public string GetSlashCommandMention(string name, ulong? guildId)
        {
            var guild = slash.RegisteredCommands.FirstOrDefault(g => g.Key == guildId);
            if(guild.Key is null) guild = slash.RegisteredCommands.FirstOrDefault(g => g.Key == null);
            if (guild.Key is not null)
            {
                var command = guild.Value.FirstOrDefault(c => c.Name == name);
                if(command is not null)
                {
                    return $"</{command.Name}:{command.Id}>";
                }
            }
            return "/" + name;
        }

        public async Task LoadGuilds()
        {
            List<Data.SQLite.PalantirEntity> guilds = nightly ? 
                await Program.PalantirDb.QueryAsync<Data.SQLite.PalantirEntity>(db => db.PalantiriNightly) : 
                await Program.PalantirDb.QueryAsync<Data.SQLite.PalantirEntity>(db => db.Palantiri);

            foreach(var guild in guilds)
            {
                var service = new LobbiesService(client, guild);
                await service.Start();
                await Task.Delay(200); // avoid rate limits: 50/s. Guild init takes up to 12 calls for the message setup
            }
        }
    }
}
