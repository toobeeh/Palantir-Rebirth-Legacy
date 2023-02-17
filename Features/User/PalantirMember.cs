using Newtonsoft.Json;
using Palantir_Rebirth.Data.Cache;
using Palantir_Rebirth.Data.JSON;
using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Drops;
using Palantir_Rebirth.Features.Scenes;
using Palantir_Rebirth.Features.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.User
{
    internal class PalantirMember
    {
        private readonly DatabaseCache<MemberEntity> memberCache;
        private readonly DependencyCache<Member, string> discordMemberCache;
        private readonly DependencyCache<PermissionFlag, int> flagsCache;
        private readonly DependencyCache<List<SpriteProperty>, string> spritesCache;
        private readonly DependencyCache<List<SceneProperty>, string> scenesCache;

        public string Token { get; private set; }
        public string ID { get { return discordMemberCache.Item.UserID; } }
        public IReadOnlyList<ObservedGuild> Guilds {  get { return discordMemberCache.Item.Guilds; } }
        public int Bubbles { get { return memberCache.Item.Bubbles; } }
        public int RegularDrops { get { return memberCache.Item.Drops; } }
        public string PatronEmoji { get { return (Flags.Patron || Flags.BotAdmin) && memberCache.Item.Emoji != null ? memberCache.Item.Emoji : ""; } }
        public PermissionFlag Flags { get { return flagsCache.Item; } }
        public List<SpriteProperty> Sprites {  get { return spritesCache.Item; } }
        public List<SceneProperty> Scenes { get { return scenesCache.Item; } }
        public SpriteManager SpriteManager { get; }
        public SceneManager SceneManager { get; }

        public PalantirMember(string login) {

            // check if user exists in db
            Token = login;
            var result = Program.PalantirDb.Query(db => db.Members.Where(member => member.Login == login));
            if (result.Count == 0) throw new Exception("no member with such login");

            // init cache to db
            memberCache = new(
                (db) => db.Members.First(m => m.Login == Token),
                (db, value) => db.Members.Update(value),
                30 * 60 * 1000
            );

            // init dependency to cached db entity
            discordMemberCache = new(
                () => memberCache.Item.Member,
                (member) => JSONUtils.FromString<Member>(member)
            );

            // init dependency to cached db entity
            flagsCache = new(
                () => memberCache.Item.Flag,
                (flag) => new PermissionFlag(Convert.ToByte(flag))
            );

            // init dependency to cached db entity
            spritesCache = new(
                () => memberCache.Item.Sprites,
                (sprites) => SpriteUtils.ParseInventory(sprites)
            );

            // init dependency to cached db entity
            scenesCache = new(
                () => memberCache.Item.Scenes is null ? "" : memberCache.Item.Scenes,
                (scenes) => SceneUtils.ParseInventory(scenes)
            );

            SpriteManager = new SpriteManager(this);
            SceneManager = new SceneManager(this);
        }

        public (double worth, int count) GetLeagueDrops()
        {
            var drops = DropUtils.GetUserLeagueDrops(ID);
            return (DropUtils.CalcLeagueDropTotal(drops), drops.Count());
        }

        public int GetCredit()
        {
            int credit = Bubbles;
            credit += RegularDrops * 20;
            credit += Convert.ToInt32(GetLeagueDrops().worth) * 20;

            credit -= Sprites.ConvertAll(s => s.Cost).Sum();
            credit -= SceneManager.GetSceneInvWorth();

            return credit;
        }

        public int GetSlotCount()
        {
            int slots = 1;
            if (Flags.Patron) slots++;
            if (Flags.BotAdmin) slots += 1000;

            double drops = RegularDrops + GetLeagueDrops().worth;
            slots += Convert.ToInt32(Math.Floor(drops/1000));
            return slots;
        }

        public void MarkDirty()
        {
            memberCache.MarkDirty();
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(Token);
        }
    }
}
