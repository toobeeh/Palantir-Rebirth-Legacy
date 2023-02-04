using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.JSON
{
    public class Lobby
    {
        public string ID { get; set; }
        public string Key { get; set; }
        public int Round { get; set; }
        public bool Private { get; set; }
        public string Host { get; set; }
        public string Language { get; set; }
        public string Link { get; set; }
        public string GuildID { get; set; }
        public string ObserveToken { get; set; }
        public IList<Player> Players { get; set; }
        public IList<Player> Kicked { get; set; }
    }

    public class ProvidedLobby
    {
        public string ID { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public string Restriction { get; set; }
    }

    public class Player
    {
        public string Name { get; set; }
        public short Score { get; set; }
        public bool Drawing { get; set; }
        public bool Sender { get; set; }
        public string? ID { get; set; }
        public string LobbyPlayerID { get; set; }
    }
}
