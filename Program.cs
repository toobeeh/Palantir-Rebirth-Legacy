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

        static async Task Main(string[] args)
        {
            var config = JSONUtils.FromFile<BotConfig>(args[0]);
            //while (config.Nightly && !Debugger.IsAttached)
            //{
            //    Thread.Sleep(1000);
            //}

            Console.WriteLine("hi");
            PalantirDb = new PalantirDatabase(config.PalantirDatabasePath);
            Console.WriteLine("hi");
            Palantir = new PalantirBot(config.TokenPath, config.Nightly);

            Console.WriteLine("hi");
            await Palantir.Connect();
            Console.WriteLine("hi");
            await Palantir.SendDebugMessage("Hello there!");
            Console.WriteLine("hi");
           // await Palantir.LoadGuilds();

            Console.WriteLine("hi");
            var scheduler = await QuartzUtils.GetScheduler();
            Console.WriteLine("hi");
            await QuartzUtils.ScheduleLobbyCollector(scheduler);

            Console.WriteLine("hi");
            await Task.Delay(-1);
        }
    }
}