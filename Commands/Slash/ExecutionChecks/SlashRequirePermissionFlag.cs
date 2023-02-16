using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using Palantir_Rebirth.Features.User;

namespace Palantir_Rebirth.Commands.Slash.ExecutionChecks
{
    internal class SlashRequirePermissionFlag : SlashCheckBaseAttribute
    {
        private byte flag;
        public SlashRequirePermissionFlag(byte flag) {
            this.flag = flag;
        }

        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            var member = PalantirMemberFactory.ByDiscordID(ctx.Member.Id);
            if (!member.Flags.CheckForPermissionByte(flag)) return Task.FromResult(false);
            return Task.FromResult(true);
        }
    }
}
