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
        public Dictionary<string, List<Lobby>> GuildLobbies { get; private set; }

        private readonly PalantirDatabase db;

        public LobbyCollectorJob(PalantirDatabase db) 
        {
            GuildLobbies = new Dictionary<string, List<Lobby>>();
            this.db = db;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // get all current reports
            var reports = db.Query(db => db.Reports);

            // get all status
            var status = db.Query(db => db.Status).Select(status => Utils.FromString<PlayerStatus>(status.Status));

            // get distinct reports
            var lobbies = MoreLinq.Extensions.DistinctByExtension.DistinctBy(reports, report => report.LobbyID).Select(report => report.Report);

            // get all senders
            var senders = reports.Select(report =>
                Utils.FromString<Lobby>(report.Report)
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
            Dictionary<string, List<Lobby>> onlineLobbies = new();
            foreach (string observeToken in guilds)
            {
                List<Lobby> guildLobbies = new();
                int totalSender = 0;

                foreach (string report in lobbies)
                {
                    var lobby = Utils.FromString<Lobby>(report);
                    int guildSenders = 0;

                    // check if player is sender for a guild
                    foreach(Player player in lobby.Players)
                    {
                        var sender = senders.FirstOrDefault(sender => sender.observeToken == observeToken && sender.lobbyID == lobby.ID && sender.playerID == player.LobbyPlayerID);

                        if(sender != null && sender.status == null)
                        {
                            int a = 1;
                        }
                        if (sender != null && sender.status != null)
                        {
                            player.ID = sender.status.PlayerMember.UserID;
                            player.Sender = true;
                            guildSenders++;
                        }
                        else player.Sender = false;
                    }

                    totalSender += guildSenders;
                    
                    if(guildSenders > 0) guildLobbies.Add(lobby);
                }

                onlineLobbies.Add(observeToken, guildLobbies);

            }

            GuildLobbies = onlineLobbies;
        }
    }
}
