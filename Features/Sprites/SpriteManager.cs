using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Sprites.Exceptions;
using Palantir_Rebirth.Features.User;
using Palantir_Rebirth.Features.User.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Sprites
{
    internal class SpriteManager
    {
        private readonly PalantirMember member;
        public SpriteManager(PalantirMember member) { 
            this.member = member;
        }

        public SpritesEntity BuySprite(int spriteId)
        {
            var sprite = SpriteUtils.GetSprite(spriteId);
            if(sprite.ID == 1003 && !member.Flags.Patron) throw new MemberSpriteException(member, sprite, MemberSpriteException.PATRON_SPRITE);
            if (member.Sprites.Any(s => s.ID == spriteId)) throw new MemberSpriteException(member, sprite, MemberSpriteException.ALREADY_IN_INV);

            // check if its an event sprite
            if(sprite.EventDropID > 0)
            {

            }
            else
            {
                if (!member.Flags.BotAdmin &&  member.GetCredit() < sprite.Cost) throw new MemberSpriteException(member, sprite, MemberSpriteException.CREDIT_TOO_LOW);
            }
            
            SpriteUtils.AddSpriteToInv(sprite, member);
            member.MarkDirty();
            return sprite;
        }
    }
}
