using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Client
{
    internal class PalantirBot
    {
        private DiscordClient client;
        public InteractivityExtension interactivity { private set; get; }

        public PalantirBot(string tokenPath)
        {
            string token = File.ReadAllText(tokenPath);

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

            var slash = client.UseSlashCommands();

            interactivity = client.UseInteractivity();
        }

        public async Task Connect()
        {
            await client.ConnectAsync();
        }
    }
}
