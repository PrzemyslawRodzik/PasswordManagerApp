

using System.Threading.Tasks;
using Quartz;



namespace PasswordManagerApp.Events.ScheludeTasks
{   
    [DisallowConcurrentExecution]
    public class PasswordBreachCheckJob : IJob

    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
    
}