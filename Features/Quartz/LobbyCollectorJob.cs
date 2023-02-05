using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Palantir_Rebirth.Data.JSON;
using Palantir_Rebirth.Data.SQLite;
using MoreLinq;

namespace Palantir_Rebirth.Features.Quartz
{
    internal class LobbyCollectorJob : IJob
    {
        class SenderTuple { public string observeToken; public string lobbyID; public string playerID; public PlayerStatus? status; }
        public static Dictionary<string, List<(Lobby, ProvidedLobby)>> GuildLobbies { get; private set; }
        public static List<PlayerStatus> PlayerStatuses { get; private set; }

        private static readonly PalantirDatabase db;

        static LobbyCollectorJob() 
        {
            GuildLobbies = new Dictionary<string, List<(Lobby, ProvidedLobby)>>();
            db = Program.PalantirDb;
            PlayerStatuses = new List<PlayerStatus>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // get all current reports
            var reports = db.Query(db => db.Reports);

            // get all status
            var status = db.Query(db => db.Status).Select(status => JSONUtils.FromString<PlayerStatus>(status.Status));

            // get distinct reports
            var lobbies = MoreLinq.Extensions.DistinctByExtension.DistinctBy(reports, report => report.LobbyID).Select(report => report.Report);

            // get properties of current lobbies
            var lobbyDetails = db.Query(db => db.Lobbies).Select(lobby => JSONUtils.FromString<ProvidedLobby>(lobby.Lobby));

            // get all senders
            var senders = reports.Select(report =>
                JSONUtils.FromString<Lobby>(report.Report)
                    .Players
                    .Where(player => player.Sender)
                    .Select(player => new SenderTuple { 
                        observeToken = report.ObserveToken, 
                        lobbyID = report.LobbyID, 
                        playerID = player.LobbyPlayerID ,
                        status = status.FirstOrDefault(status => status.LobbyPlayerID == player.LobbyPlayerID && status.LobbyID == report.LobbyID)
                    }
             )).Flatten().Cast<SenderTuple>();

            // get all distinct receiver guilds
            var guilds = reports.Select(report => report.ObserveToken).Distinct();

            // set unique lobby for each guild
            Dictionary<string, List<(Lobby, ProvidedLobby)>> onlineLobbies = new();
            foreach (string observeToken in guilds)
            {
                List<(Lobby, ProvidedLobby)> guildLobbies = new();

                foreach (string report in lobbies)
                {
                    var lobby = JSONUtils.FromString<Lobby>(report);
                    var lobbyDetail = lobbyDetails.FirstOrDefault(l => l.ID == lobby.ID);

                    if (lobbyDetail == null) continue;

                    int guildSenders = 0;

                    // check if player is sender for a guild
                    foreach(Player player in lobby.Players)
                    {
                        var sender = senders.FirstOrDefault(sender => sender.observeToken == observeToken && sender.lobbyID == lobby.ID && sender.playerID == player.LobbyPlayerID);

                        if (sender != null && sender.status != null)
                        {
                            player.ID = sender.status.PlayerMember.UserID;
                            player.Sender = true;
                            guildSenders++;
                        }
                        else player.Sender = false;
                    }
                    
                    if(guildSenders > 0) guildLobbies.Add((lobby, lobbyDetail));
                }

                onlineLobbies.Add(observeToken, guildLobbies);
            }

            GuildLobbies = onlineLobbies;
            PlayerStatuses = status.ToList();
        }
    }
}
