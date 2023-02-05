using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Client;
using Palantir_Rebirth.Features.Lobbies;
using Palantir_Rebirth.Features.Quartz;
using Quartz;
using System;
using System.Resources;

namespace Palantir_Rebirth
{
    internal class Program
    {
        public static PalantirBot Palantir;
        public static PalantirDatabase PalantirDb;

        static async Task Main(string[] args)
        {
            var config = Data.JSON.JSONUtils.FromFile<Data.JSON.BotConfig>(args[0]);

            Palantir = new PalantirBot(config.TokenPath);
            PalantirDb = new PalantirDatabase(config.PalantirDatabasePath);

            await Palantir.Connect();

            var scheduler = await QuartzUtils.GetScheduler();
            await QuartzUtils.ScheduleLobbyCollector(scheduler);

            await Task.Delay(-1);
        }
    }
}