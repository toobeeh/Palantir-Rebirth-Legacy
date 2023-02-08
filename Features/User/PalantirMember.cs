using Newtonsoft.Json;
using Palantir_Rebirth.Data.Cache;
using Palantir_Rebirth.Data.JSON;
using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.User
{
    internal class PalantirMember
    {

        private Member discordProperties;

        private readonly ItemCache<MemberEntity> memberCache;
        private readonly ItemCache<int> bubbleCache;

        public string Token { get; private set; }
        public string ID { get { return discordProperties.UserID; } }
        public int Bubbles { get { return bubbleCache.Item; } }
        public int RegularDrops { get { return memberCache.Item.Drops; } }
        public string PatronEmoji { get { return (Flags.Patron || Flags.BotAdmin) && memberCache.Item.Emoji != null ? memberCache.Item.Emoji : ""; } }
        public PermissionFlag Flags { get; private set; }

        public PalantirMember(string login) {

            // get user from db
            Token = login;
            var result = Program.PalantirDb.Query(db => db.Members.Where(member => member.Login == login));
            if (result.Count == 0) throw new Exception("no member with such login");
            var member = result.ToList()[0];

            memberCache = new(
                (db) => db.Members.First(m => m.Login == member.Login),
                (db, value) => db.Members.Update(value),
                30 * 60 * 1000
            );

            bubbleCache = new(
                (db) => db.Members.First(m => m.Login == member.Login).Bubbles,
                (db, val) =>  db.Members.First(m => m.Login == member.Login).Bubbles = val
            );

            memberCache.OnUpdate = val => bubbleCache.MarkDirty();
            memberCache.OnRead = val =>
            {
                discordProperties = JSONUtils.FromString<Member>(val.Member);
                Flags = new PermissionFlag(Convert.ToByte(val.Flag));
            };

            bubbleCache.OnUpdate = val => memberCache.MarkDirty();
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(Token);
        }
    }
}
