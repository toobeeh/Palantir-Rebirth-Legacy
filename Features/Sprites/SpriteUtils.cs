using MoreLinq;
using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Sprites.Exceptions;
using Palantir_Rebirth.Features.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Sprites
{
    internal class SpriteUtils
    {
        public static SpritesEntity GetSprite(int spriteId)
        {
            var matches = Program.PalantirDb.Query(db => db.Sprites.Where(s => s.ID == spriteId));

            if (matches.Count() < 1) throw new SpriteNotFoundException(spriteId);

            return matches[0];
        }

        public static List<SpriteProperty> ParseInventory(string inv)
        {
            List<SpriteProperty> sprites = new();
            var entries = inv.Split(',');
            foreach(var entry in entries)
            {
                int slot = entry.Count(c => c == '.');
                var sprite = GetSprite(Convert.ToInt32(entry.Replace(".", "")));
                var sp = new SpriteProperty(sprite, slot);
                sprites.Add(sp);
            }

            return sprites;
        }

        public static void AddSpriteToInv(SpritesEntity spt, PalantirMember member)
        {
            string id = spt.ID.ToString();
            string login = member.Token;

            var db = Program.PalantirDb.Open();
            var mem = db.Members.FirstOrDefault(m => m.Login == login);
            mem.Sprites += "," + id;
            Program.PalantirDb.Close(db, true);
        }

    }
}
