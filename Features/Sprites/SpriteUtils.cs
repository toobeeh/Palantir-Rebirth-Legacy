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

        public static List<SpritesEntity> GetSprites() {
            return Program.PalantirDb.Query(db => db.Sprites);
        }

        public static List<SpriteProperty> ParseInventory(string inv)
        {
            List<SpriteProperty> sprites = new();
            var available = GetSprites();
            var entries = inv.Split(',');
            foreach(var entry in entries)
            {
                int slot = entry.Count(c => c == '.');
                var sprite =available.FirstOrDefault(a => a.ID == Convert.ToInt32(entry.Replace(".", "")));
                if (sprite is null) continue;
                var sp = new SpriteProperty(sprite, slot);
                sprites.Add(sp);
            }

            return sprites;
        }

        public static string InventoryToString(List<SpriteProperty> sprites)
        {
            return sprites.ConvertAll(s => ".".Repeat(s.Slot) + s.Slot.ToString()).ToDelimitedString(",");
        }

        public static void WriteSpriteInventory(string inv, PalantirMember member)
        {
            string login = member.Token;

            var db = Program.PalantirDb.Open();
            var mem = db.Members.FirstOrDefault(m => m.Login == login);
            mem.Sprites = inv;
            Program.PalantirDb.Close(db, true);
        }

    }
}
