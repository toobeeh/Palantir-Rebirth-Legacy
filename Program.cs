using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Client;
using Palantir_Rebirth.Features.Lobbies;
using Palantir_Rebirth.Features.Quartz;
using Quartz;
using System;
using System.Resources;
using System.Diagnostics;

namespace Palantir_Rebirth
{
    internal class Program
    {
        public static PalantirBot Palantir;
        public static PalantirDatabase PalantirDb;

        static async Task Main(string[] args)
        {
            var config = Data.JSON.JSONUtils.FromFile<Data.JSON.BotConfig>(args[0]);

            Console.WriteLine(Debugger.IsAttached);
            //while (config.Nightly && !Debugger.IsAttached)
            //{
            //    await Task.Delay(1000);
            //}

            PalantirDb = new PalantirDatabase(config.PalantirDatabasePath);
            Palantir = new PalantirBot(config.TokenPath, config.Nightly);

            await Palantir.Connect();
            await Palantir.SendDebugMessage("Hello there!");
            await Palantir.LoadGuilds();

            var scheduler = await QuartzUtils.GetScheduler();
            await QuartzUtils.ScheduleLobbyCollector(scheduler);

            await Task.Delay(-1);
        }
    }
}