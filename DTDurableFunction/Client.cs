namespace DataTransferDFSample
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using System;
    using System.Threading.Tasks;

    public static class Client
    {
        [FunctionName("Client")]
        public static async Task Run(
            [QueueTrigger("datatransfer")] string instanceId,
            [OrchestrationClient] DurableOrchestrationClient client, TraceWriter log)
        {
            log.Info($"Client start");
            await client.StartNewAsync("DataMovement", 1);
        }
    }
}
