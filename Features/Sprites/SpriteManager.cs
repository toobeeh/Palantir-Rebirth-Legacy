using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Sprites.Exceptions;
using Palantir_Rebirth.Features.User;
using Palantir_Rebirth.Features.User.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public SpriteProperty GetSpriteFromInv(SpritesEntity sprite)
        {
            var spt = member.Sprites.FirstOrDefault(s => s.ID == sprite.ID);
            if (spt is null) throw new MemberSpriteException(member, sprite, MemberSpriteException.NOT_IN_INV);
            return spt;
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

            string inv = SpriteUtils.InventoryToString(member.Sprites);
            inv += "," + sprite.ID;
            SpriteUtils.WriteSpriteInventory(inv, member);
            member.MarkDirty();

            return sprite;
        }

        public SpriteProperty? UseSprite(int spriteId, int slotId)
        {
            var sprite = spriteId != 0 ? GetSpriteFromInv(SpriteUtils.GetSprite(spriteId)) : null;

            if (sprite is not null && member.GetSlotCount() < slotId) throw new MemberSpriteSlotException(member, sprite, slotId, MemberSpriteSlotException.SLOT_NOT_UNLOCKED);
            if (sprite is not null && sprite.Special && member.Sprites.Any(s => s.ID != sprite.ID && s.Special)) throw new MemberSpriteSlotException(member, sprite, slotId, MemberSpriteSlotException.TO_MANY_SPECIAL);

            var inv = member.Sprites;
            inv.ForEach(m =>
            {
                if (sprite is not null && m.ID == sprite.ID) m.Slot = slotId;
                else if (slotId != 0 && m.Slot == slotId) m.Slot = 0;
            });

            string invString = SpriteUtils.InventoryToString(inv);
            SpriteUtils.WriteSpriteInventory(invString, member);
            member.MarkDirty();

            if(sprite is not null) sprite.Slot = slotId;
            return sprite;
        }
    }
}
