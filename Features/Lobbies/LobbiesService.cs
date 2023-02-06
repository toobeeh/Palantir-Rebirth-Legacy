using DSharp.FrameWork;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Palantir_Rebirth.Data.JSON;
using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Quartz;
using Palantir_Rebirth.Features.User;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Lobbies
{
    internal class LobbiesService
    {
        private static List<string> emojis = (new string[]{
            "<a:l9:718816560915021884>",
            "<a:l8:718816560923410452>",
            "<a:l7:718816561116217395>",
            "<a:l6:718816561871192088>",
            "<a:l5:718816561993089056>",
            "<a:l4:718816562441879602>",
            "<a:l36:721872926411980820>",
            "<a:l35:721872926189551657>",
            "<a:l34:721872925661069312>",
            "<a:l33:721872925531308032>",
            "<a:l32:721872924570550352>",
            "<a:l31:721872924465954928>",
            "<a:l30:721872923182366730>",
            "<a:l3:718816563217825845>",
            "<a:l29:721872922452688916>",
            "<a:l28:721872921995378738>",
            "<a:l27:721872921844252705>",
            "<a:l26:721872921777274908>",
            "<a:l25:721872921152192513>",
            "<a:l24:721872920866979881>",
            "<a:l23:721872920347017216>",
            "<a:l22:721872920129044510>",
            "<a:l21:721872919990501468>",
            "<a:l20:721872919973855233>",
            "<a:l2:718816563284803615>",
            "<a:l1:718816563750371358>",
            "<a:l19:721872919919067247>",
            "<a:l18:721872918921084948>",
            "<a:l17:721872918480421014>",
            "<a:l16:721872918304522280>",
            "<a:l15:721872916257439745>",
            "<a:l14:718817049987776534>",
            "<a:l13:718817051828944926>",
            "<a:l12:718816559149350973>"
        }).ToList();

        private DiscordClient client;
        private List<DiscordMessage> messages;
        private ObservedGuild config;
        private GuildSettings settings;
        private Thread thread;

        public bool Active { get; private set; }

        public LobbiesService(DiscordClient client, PalantirEntity guild) 
        {
            this.client = client;
            messages = new List<DiscordMessage>();
            Active = false;
            config = JSONUtils.FromString<ObservedGuild>(guild.Palantir);
            thread = new Thread(new ThreadStart(UpdateLoop));

            var settingsResult = Program.PalantirDb.Query(db => db.GuildSettings.Where(s => s.GuildID == config.GuildID));
            if(settingsResult.Count() > 0)
            {
                settings = JSONUtils.FromString<GuildSettings>(settingsResult[0].Settings);
            }
            else
            {
                settings = new GuildSettings
                {
                    Header = "```fix\nCurrently playing skribbl.io```",
                    IdleMessage = "Seems like no-one is playing :( \nAsk some friends to join or go solo!\n\n ",
                    Timezone = 0,
                    ShowAnimatedEmojis = true,
                    ShowRefreshed = false,
                    ShowToken = true
                };
            }
        }

        public async Task<bool> Start()
        {
            // get first message
            try
            {
                var channel = await client.GetChannelAsync(Convert.ToUInt64(config.ChannelID));
                var message = await channel.GetMessageAsync(Convert.ToUInt64(config.MessageID));
                messages.Add(message);
            }
            catch(Exception ex)
            {
                Logger.Error("Could not get first guild msg", ex);
                return false;
            }

            // get other messages
            var next = new List<DiscordMessage>(await messages[0].Channel.GetMessagesAfterAsync(messages[0].Id, 10));
            while (next.Count > 0)
            {
                var current = next[0];
                if (current.Author.Id == client.CurrentUser.Id) messages.Add(current);
                //else break;
                next.RemoveAt(0);
            }

            thread.Start();
            Active = true;
            return true;
        }

        public void Stop()
        {
            Active = false;
        }

        private async void UpdateLoop()
        {
            while (Active)
            {
                string message = await BuildLobbyContent();
                List<string> splits = new();

                // split message to parts of each max ~2000 chars
                while(message.Length > 1900)
                {
                    int firstSplit = message.IndexOf(' ');
                    if (splits.Count == 0 || splits.Last().Length > 1900) splits.Add(message[0..firstSplit]);
                    else splits[splits.Count - 1] += message[0..firstSplit];

                    message = message[(firstSplit + 1)..];
                }
                splits.Insert(0, message);

                int splitIndex = 0;
                foreach(var split in splits)
                {
                    if (messages.Count - 1 < splitIndex)
                    {
                        try
                        {
                            var msg = await messages[0].Channel.SendMessageAsync(split.Replace(" ", ""));
                            messages.Add(msg);
                            
                        }
                        catch(Exception ex)
                        {
                            Logger.Error("Could not modify lobby message", ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            if(messages[splitIndex].Content != split.Replace(" ", ""))
                            {
                                var newmsg = await messages[splitIndex].ModifyAsync(split.Replace(" ", ""));
                                messages[splitIndex] = newmsg;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Could not add new lobby message", ex);
                        }
                    }

                    splitIndex++;
                }

                // clear remaining messages
                for(int i = splitIndex + 1; i < messages.Count; i++){
                    if(messages[i].Content != "_ _")
                    {
                        var newmsg = await messages[i].ModifyAsync("_ _");
                        messages[i] = newmsg;
                    }
                }

                await Task.Delay(8000);
            }
        }

        private async Task<string> BuildLobbyContent()
        {
            var lobbies = LobbyCollectorJob.GuildLobbies.ContainsKey(config.ObserveToken) ? LobbyCollectorJob.GuildLobbies[config.ObserveToken] : new();
            lobbies = lobbies.OrderBy(lobby => lobby.Item2.ID).ToList();
            string message = "";

            // add event header
            //List<EventEntity> events = Events.GetEvents(true);
            //if (events.Count <= 0) message += "```arm\nNo event active :( Check upcoming events with '>upcoming'.\n‎As soon as an event is live, you can see details using '>event'.```\n\n";
            //else message += "```arm\n" + events[0].EventName + ": \n" + events[0].Description + "\nView details with '>event'!```\n";

            // add timestamp and connect if enabled
            message += settings.Header + "\n";
            if (settings.ShowRefreshed) message += "Refreshed: <t:" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ":R> \n";
            if (settings.ShowToken) message += "Click to connect: <https://typo.rip/i?invite=" + config.ObserveToken + ">\n";

            // space
            message += "\n";

            // build each lobby
            int lobbyIndex = 0;
            foreach(var (lobby, lobbyDetails) in lobbies)
            {
                List<short> scores = lobby.Players.Select(p => p.Score).Distinct().OrderDescending().ToList();

                // set additional properties if private
                string lobbyDescription = "";
                if (lobby.Private)
                {
                    lobbyDescription = lobbyDetails.Description.Substring(0, Math.Min(lobbyDetails.Description.Length, 100));
                    lobbyDescription = lobbyDescription.Length > 0 ?  "> `" + DSharpPlus.Formatter.Sanitize(lobbyDescription).Replace("`", "").Replace("\n", "") + "`\n" : "";

                    if (lobbyDescription.Contains("#nojoin")) lobby.Link = "Closed Private Game"; 

                    if (lobbyDetails.Restriction == "restricted") lobby.Link = "Restricted Private Game";
                    else if (lobbyDetails.Restriction != "unrestricted" && config.GuildID != lobbyDetails.Restriction) lobby.Link = "Server-Restricted Private Game";
                }

                // set lobby header
                string lobbyText = 
                    "> **#" + (++lobbyIndex) + "**    " 
                    + (settings.ShowAnimatedEmojis ? emojis[(new Random()).Next(emojis.Count - 1)] : "") 
                    + "     " 
                    + lobby.Host + "   **|**  " 
                    + lobby.Language + "   **|**   Round " 
                    + lobby.Round + "   **|**   " + (lobby.Private ? "Private \n> <" + lobby.Link + ">" : "Public")  
                    + "\n> " + lobby.Players.Count + " Players \n";

                // add lobby description
                if (lobbyDescription != "") lobbyText += lobbyDescription.Replace("\\", "");

                string players = "";
                string sender = "```fix\n";
                int senderCount = 0;
                foreach (Player player in lobby.Players)
                {
                    if (player.Sender)
                    {
                        var member = PalantirMemberFactory.ByDiscordID(player.ID);

                        senderCount++;
                        string line = "";
                        line += Formatter.Sanitize(player.Name).Replace("\\", " ").Replace("`", " ");
                        line += new string(' ', (20 - player.Name.Length) < 0 ? 0 : (20 - player.Name.Length));
                        line += player.Score + " pts";

                        if (player.Score != 0)
                        {
                            if (scores.IndexOf(player.Score) == 0) line += " 🏆 ";
                            if (scores.IndexOf(player.Score) == 1) line += " 🥈 ";
                            if (scores.IndexOf(player.Score) == 2) line += " 🥉 ";
                        }

                        line += new string(' ', (32 - line.Length) < 0 ? 0 : (32 - line.Length));

                        // add patron emoji or bubbles
                        string patronEmoji = member.PatronEmoji;
                        if (patronEmoji.Length > 0)
                        {
                            line += "  " + patronEmoji + " ";
                        }
                        else line += "  🔮 " + member.Bubbles + " Bubbles";
                        line += player.Drawing ? " 🖍 \n" : "\n";
                        sender += line;
                    }
                    else
                    {
                        if (player.Score != 0)
                        {
                            if (scores.IndexOf(player.Score) == 0) players += " `🏆` ";
                            if (scores.IndexOf(player.Score) == 1) players += " `🥈` ";
                            if (scores.IndexOf(player.Score) == 2) players += " `🥉` ";
                        }
                        players += Formatter.Sanitize(player.Name).Replace("\\", " ").Replace("`", " ");
                        players += (player.Drawing ? " 🖍, " : ", ");
                    }
                }

                // remove trailing comma
                if (players.Length > 0) players = players[0..^2];
                sender += "```";

                if (senderCount > 0) lobbyText += sender;
                if (players.Length > 0) lobbyText += players;

                lobbyText += "\n\n\n";
                message += lobbyText;
                message += " "; // lobby break indicator em space
            }

            // add searching message
            string searching = "";

            // add waiting message
            string waiting = "";

            // add footer
            if (searching.Length > 0) message += "<a:onmyway:718807079305084939>   " + searching[0..^2];
            if (waiting.Length > 0) message += "\n\n:octagonal_sign:   " + waiting[0..^2];
            if (settings.ShowAnimatedEmojis && lobbies.Count == 0 && searching.Length == 0) message += "\n <a:alone:718807079434846238>\n";
            if (lobbies.Count == 0 && searching.Length == 0) message += settings.IdleMessage;

            return message;
        }
    }
}
