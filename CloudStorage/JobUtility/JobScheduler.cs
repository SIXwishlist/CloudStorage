using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CloudStorage.JobUtility
{
    public class JobScheduler
    {

        public static async void Run()
        {
            Debug.WriteLine("Job started to be scheduled");

            // Define a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler scheduler = await sf.GetScheduler();

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<CloudStororageJob>()
            .WithIdentity("Job1", "Group1")
            .Build();

            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("Trigger1", "Group1")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //        s.OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(9, 53))
            //      )
            //    .Build();

            //await scheduler.ScheduleJob(job, trigger);

            //get a "nice round" time a few seconds in the future...
            DateTimeOffset startTime = DateBuilder.NextGivenMinuteDate(null, 2);

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("Trigger1", "Group1")
                .StartAt(startTime)
                .Build();

            // schedule it to run!
            DateTimeOffset? ft = await scheduler.ScheduleJob(job, trigger);
            Debug.WriteLine("Current time:" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            Debug.WriteLine("Current time UTC:" + DateTime.UtcNow.ToLongDateString() + " " + DateTime.UtcNow.ToLongTimeString());
            Debug.WriteLine(job.Key +
                     " will run at: " + ft);

            Debug.WriteLine("Job has been scheduled");

        }
      
    }
}