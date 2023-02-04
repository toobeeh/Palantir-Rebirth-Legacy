using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Client;
using Palantir_Rebirth.Features.Lobbies;
using Palantir_Rebirth.Features.Quartz;
using System;
using System.Resources;

namespace Palantir_Rebirth
{
    internal class Program
    {
        public static PalantirBot palantir;
        public static PalantirDatabase palantirDb;

        static async Task Main(string[] args)
        {
            var config = Data.JSON.Utils.FromFile<Data.JSON.BotConfig>(@"C:\Users\User\source\repos\toobeeh\Palantir-Rebirth\config.json");

            palantir = new PalantirBot(config.TokenPath);
            palantirDb = new PalantirDatabase(config.PalantirDatabasePath);

            await palantir.Connect();

            var lobbyCollector = new LobbyCollectorJob(palantirDb);
            lobbyCollector.Execute(null as Quartz.IJobExecutionContext);

            await Task.Delay(-1);
        }
    }
}