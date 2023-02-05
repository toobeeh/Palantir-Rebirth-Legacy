using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.JSON
{
    public class Member
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserLogin { get; set; }
        public List<ObservedGuild> Guilds { get; set; }
    }

    public class PlayerStatus
    {
        public Member PlayerMember { get; set; }
        public string Status { get; set; }
        public string? LobbyID { get; set; }
        public string? LobbyPlayerID { get; set; }
    }
    public class CustomCard
    {
        public string HeaderColor { get; set; }
        public double HeaderOpacity { get; set; }
        public double BackgroundOpacity { get; set; }
        public string BackgroundImage { get; set; }
        public string LightTextColor { get; set; }
        public string DarkTextColor { get; set; }
    }
}
