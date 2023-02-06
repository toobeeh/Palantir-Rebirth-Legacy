using Newtonsoft.Json.Linq;
using Palantir_Rebirth.Data.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.User
{
    internal class PalantirMemberFactory
    {
        private static Dictionary<int, PalantirMember> instancesByToken;
        private static Dictionary<ulong, PalantirMember> instancesByID;

        static PalantirMemberFactory()
        {
            instancesByToken = new();
            instancesByID = new();
        }

        public static PalantirMember ByToken(int token)
        {
            PalantirMember instance;
            instancesByToken.TryGetValue(token, out instance);
            if (instance != null) return instance;

            instance = new PalantirMember(token.ToString());
            instancesByToken.Add(token, instance);
            instancesByID.Add(Convert.ToUInt64(instance.ID), instance);
            return instance;
        }
        public static PalantirMember ByToken(string token)
        {
            return ByToken(Convert.ToInt32(token));
        }

        public static PalantirMember ByDiscordID(ulong discordID)
        {
            PalantirMember instance;
            instancesByID.TryGetValue(discordID, out instance);
            if (instance != null) return instance;

            string id = discordID.ToString();
            var member = Program.PalantirDb
                .Query(db => db.Members.Where(member => member.Member.Contains(id)))
                .Select(member => JSONUtils.FromString<Member>(member.Member))
                .FirstOrDefault(member => member.UserID == id);

            if (member == null) throw new Exception("no member with such discord id");

            instance = new PalantirMember(member.UserLogin);
            instancesByToken.Add(Convert.ToInt32(instance.Token), instance);
            instancesByID.Add(discordID, instance);
            return instance;
        }

        public static PalantirMember ByDiscordID(string discordID)
        {
            return ByDiscordID(Convert.ToUInt64(discordID));
        }
    }
}
