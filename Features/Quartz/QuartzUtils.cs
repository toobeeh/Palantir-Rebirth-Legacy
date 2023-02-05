using Palantir_Rebirth.Data.SQLite;
using Palantir_Rebirth.Features.Lobbies;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Features.Quartz
{
    internal class QuartzUtils
    {
        public static async Task<IScheduler> GetScheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler scheduler = await schedFact.GetScheduler();
            await scheduler.Start();
            return scheduler;
        }

        public static async Task ScheduleLobbyCollector(IScheduler scheduler)
        {
            IJobDetail detail = JobBuilder.Create<LobbyCollectorJob>()
               .WithIdentity("Lobby Collector")
               .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithDailyTimeIntervalSchedule(t => t.WithIntervalInSeconds(3))
                .Build();
            await scheduler.ScheduleJob(detail, trigger);
        }
    }
}
