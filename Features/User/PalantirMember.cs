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
        private readonly DatabaseCache<MemberEntity> memberCache;
        private readonly DependencyCache<Member, string> discordMemberCache;
        private readonly DependencyCache<PermissionFlag, int> flagsCache;

        public string Token { get; private set; }
        public string ID { get { return discordMemberCache.Item.UserID; } }
        public int Bubbles { get { return memberCache.Item.Bubbles; } }
        public int RegularDrops { get { return memberCache.Item.Drops; } }
        public string PatronEmoji { get { return (Flags.Patron || Flags.BotAdmin) && memberCache.Item.Emoji != null ? memberCache.Item.Emoji : ""; } }
        public PermissionFlag Flags { get { return flagsCache.Item; } }

        public PalantirMember(string login) {

            // check if user exists in db
            Token = login;
            var result = Program.PalantirDb.Query(db => db.Members.Where(member => member.Login == login));
            if (result.Count == 0) throw new Exception("no member with such login");

            // init cache to db
            memberCache = new(
                (db) => db.Members.First(m => m.Login == Token),
                (db, value) => db.Members.Update(value)//,
                //30 * 60 * 1000
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
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(Token);
        }
    }
}
