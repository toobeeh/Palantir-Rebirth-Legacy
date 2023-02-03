using Palantir_Rebirth.Data.SQLite;
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

            await Task.Delay(-1);
        }
    }
}