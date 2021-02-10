

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace PasswordManagerApp.ScheludeTasks
{


public class QuartzHostedService : IHostedService
{
    private readonly ISchedulerFactory schedulerFactory;
    private readonly IJobFactory jobFactory;
    private readonly IEnumerable<JobMetadata> jobMetadatas;
    public QuartzHostedService(ISchedulerFactory
        schedulerFactory,
        IEnumerable<JobMetadata> jobMetadatas,
        IJobFactory jobFactory)
    {
        this.schedulerFactory = schedulerFactory;
        this.jobMetadatas = jobMetadatas;
        this.jobFactory = jobFactory;
    }
    public IScheduler Scheduler { get; set; }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        Scheduler.JobFactory = jobFactory;

        foreach (var jobMetadata in jobMetadatas)
        {
            var job = CreateJob(jobMetadata);
            var trigger = CreateTrigger(jobMetadata);

            await Scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        await Scheduler.Start(cancellationToken);
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Scheduler?.Shutdown(cancellationToken);
    }
    private ITrigger CreateTrigger(JobMetadata jobMetadata)
    {
        return TriggerBuilder.Create()
        .WithIdentity(jobMetadata.JobId.ToString())
        .WithCronSchedule(jobMetadata.CronExpression)
        .WithDescription($"{jobMetadata.JobName}")
        .Build();
    }
    private IJobDetail CreateJob(JobMetadata jobMetadata)
    {
        return JobBuilder
        .Create(jobMetadata.JobType)
        .WithIdentity(jobMetadata.JobId.ToString())
        .WithDescription($"{jobMetadata.JobName}")
        .Build();
    }
}
}
