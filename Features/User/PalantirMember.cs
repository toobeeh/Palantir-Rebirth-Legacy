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
        private readonly MemberEntity member;
        private readonly Member discordProperties;

        public string Token { get { return member.Login; } }
        public string ID { get { return discordProperties.UserID; } }
        public int Bubbles { get { return member.Bubbles; } }
        public int RegularDrops { get { return member.Drops; } }
        public string PatronEmoji { get { return (Flags.Patron || Flags.BotAdmin) && member.Emoji != null ? member.Emoji : ""; } }
        public PermissionFlag Flags { get; private set; }

        public PalantirMember(string login) {

            // get user from db
            var result = Program.PalantirDb.Query(db => db.Members.Where(member => member.Login == login));
            if (result.Count == 0) throw new Exception("no member with such login");
            member = result.ToList()[0];

            // parse dsicord props json
            discordProperties = JSONUtils.FromString<Member>(member.Member);

            // get flags
            Flags = new PermissionFlag(Convert.ToByte(member.Flag));
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(Token);
        }
    }
}
