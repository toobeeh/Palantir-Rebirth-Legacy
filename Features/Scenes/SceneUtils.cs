using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Sprites;
using Palantir_Rebirth.Features.Sprites.Exceptions;
using Palantir_Rebirth.Features.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Scenes
{
    internal class SceneUtils
    {
        public static SceneEntity GetScene(int sceneId)
        {
            var matches = Program.PalantirDb.Query(db => db.Scenes.Where(s => s.ID == sceneId));

            if (matches.Count() < 1) throw new SpriteNotFoundException(sceneId);

            return matches[0];
        }

        public static List<SceneProperty> ParseInventory(string inv)
        {
            List<SceneProperty> scenes = new();
            var entries = inv.Split(',');
            foreach (var entry in entries)
            {
                bool active = entry.Contains('.');
                var scene = GetScene(Convert.ToInt32(entry.Replace(".", "")));
                var sp = new SceneProperty(scene, active);
                scenes.Add(sp);
            }

            return scenes;
        }

        public static void AddSceneToInv(SceneEntity sc, PalantirMember member)
        {
            string id = sc.ID.ToString();
            string login = member.Token;

            var db = Program.PalantirDb.Open();
            var mem = db.Members.FirstOrDefault(m => m.Login == login);
            mem.Scenes += "," + id;
            Program.PalantirDb.Close(db, true);
        }
    }
}
