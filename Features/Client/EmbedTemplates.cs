using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MoreLinq;
using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Client
{
    internal static class EmbedTemplates
    {
        private static DiscordEmbedBuilder WithCommandFooter(this DiscordEmbedBuilder builder, InteractionContext ctx)
        {
            string args = ctx.Interaction.Data.Options.ToList().ConvertAll(a => $"{a.Name}={a.Value}").ToDelimitedString("  ");
            string command = $"/{ctx.CommandName}   " + args;

            return builder.WithFooter(command, "https://cdn.discordapp.com/attachments/857205187445522442/1076159776532217866/32CircleFit.png");
        } 

        public static async Task SendSpriteEmbed(this InteractionContext ctx, string title, string message, SpritesEntity sprite)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithDescription(message)
                .WithColor(DiscordColor.Magenta)
                .WithImageUrl(sprite.URL)
                .WithCommandFooter(ctx);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
        }
    }
}
