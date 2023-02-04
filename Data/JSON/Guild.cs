using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.JSON
{
    public class ObservedGuild
    {
        public string GuildID { get; set; }
        public string ChannelID { get; set; }
        public string MessageID { get; set; }
        public string ObserveToken { get; set; }
        public string GuildName { get; set; }
    }

    public class Webhook
    {
        public string Name { get; set; }
        public string GuildID { get; set; }
        public string WebhookURL { get; set; }
    }

    public class GuildSettings
    {
        public string Header { get; set; }
        public int Timezone { get; set; }
        public string IdleMessage { get; set; }
        public bool ShowRefreshed { get; set; }
        public bool ShowToken { get; set; }
        public bool ShowAnimatedEmojis { get; set; }
    }
}
