using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Drops
{
    internal class DropUtils
    {
        public static List<PastDropEntity> GetUserLeagueDrops(string userID)
        {
            return Program.PalantirDb.Query(db => db.PastDrops.Where(d => d.CaughtLobbyPlayerID== userID && d.LeagueWeight > 0));
        }

        public static double CalcLeagueDropTotal(List<PastDropEntity> drops)
        {
            return drops.ConvertAll(drop => CalcLeagueDropWeight(drop.LeagueWeight)).Sum() / 100;
        }

        public static double CalcLeagueDropWeight(int catchMs)
        {
            //convert to ms ugh
            if (catchMs < 0) return 0;
            if (catchMs > 1000) return 30;
            return 
                -1.78641975945623 * Math.Pow(10, -9) * Math.Pow(catchMs, 4) 
                + 0.00000457264006980028 * Math.Pow(catchMs, 3)
                - 0.00397188791256729 * Math.Pow(catchMs, 2) 
                + 1.21566760222325 * catchMs;
        }
    }
}
