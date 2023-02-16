using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Client;
using Palantir_Rebirth.Features.Lobbies;
using Palantir_Rebirth.Features.Quartz;
using Quartz;
using System;
using System.Resources;
using System.Diagnostics;
using Palantir_Rebirth.Data.JSON;

namespace Palantir_Rebirth
{
    internal class Program
    {
        public static PalantirBot Palantir;
        public static PalantirDatabase PalantirDb;
        public static bool Nightly { get; private set; }

        static async Task Main(string[] args)
        {
            var config = JSONUtils.FromFile<BotConfig>(args[0]);
            Nightly = config.Nightly;
            //while (config.Nightly && !Debugger.IsAttached)
            //{
            //    Thread.Sleep(1000);
            //}

            PalantirDb = new PalantirDatabase(config.PalantirDatabasePath);
            Palantir = new PalantirBot(config.TokenPath, config.Nightly);

            await Palantir.Connect();
            //await Palantir.SendDebugMessage("Hello there!");
            await Palantir.LoadGuilds();

            var scheduler = await QuartzUtils.GetScheduler();
            await QuartzUtils.ScheduleLobbyCollector(scheduler);

            await Task.Delay(-1);
        }
    }
}