using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.User;
using Palantir_Rebirth.Commands.Slash.ExecutionChecks;

namespace Palantir_Rebirth.Commands.Slash
{
    internal class SpriteSlashCommands : ApplicationCommandModule
    {
        [SlashCommand("buy", "Buy a sprite")]
        [SlashRequirePermissionFlag(PermissionFlag.ADMIN)]
        public async Task BuySprite(InteractionContext context, [Option("ID", "The sprite ID - find all sprites on https://typo.rip")] long spriteId)
        {
            var member = PalantirMemberFactory.ByDiscordID(context.Member.Id);
            var sprite = member.SpriteManager.BuySprite(Convert.ToInt32(spriteId));

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.Title = "Whee!";
            embed.Description = "You unlocked **" + sprite.Name + "**!\nActivate it with `>use " + sprite.ID + "`";
            embed.Color = DiscordColor.Magenta;
            embed.ImageUrl = sprite.URL;
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
        }

    }
}
