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
using Palantir_Rebirth.Features.Client;
using Palantir_Rebirth.Features.Sprites;

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

            await context.SendSpriteEmbed(
                "Whee!",
                $"You unlocked **{sprite.Name}**!\nActivate it with {
                    Program.Palantir.GetSlashCommandMention("use", context.Interaction.GuildId)
                    } `{sprite.ID}`",
                sprite);
        }

        [SlashCommand("use", "Use a sprite on your avatar on skribbl")]
        public async Task UseSprite(InteractionContext context, [Option("ID", "The sprite ID - find all sprites on https://typo.rip")] long sprite, [Option("Slot", "The sprite slot")] long slot = 1)
        {
            var member = PalantirMemberFactory.ByDiscordID(context.Member.Id);
            var spriteProp = member.SpriteManager.UseSprite(Convert.ToInt32(sprite), Convert.ToInt32(slot));

            if(slot > 0 && spriteProp is not null)
            {
                await context.SendSpriteEmbed(
                   $"Your fancy sprite on slot {slot} was set to **`{spriteProp.Name}`**",
                   $"_ _",
                   spriteProp);
            }
            else if(spriteProp is null && slot > 0)
            {
                await context.SendSpriteEmbed(
                   $"OK then",
                   $"Your sprite on slot {slot} has been disabled.",
                   null);
            }
            else if(spriteProp is not null && slot == 0)
            {
                await context.SendSpriteEmbed(
                   $"OK then",
                   $"Your sprite {spriteProp.Name} has been disabled.",
                   spriteProp);
            }
        }

    }
}
