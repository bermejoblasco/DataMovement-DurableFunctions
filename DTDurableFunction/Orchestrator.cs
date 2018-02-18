namespace VSSample
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.DataMovement;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    public static class Orchestrator
    {
        [FunctionName("DataMovement")]
        public static async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContext ctx,
            TraceWriter log)
        {
            try
            {
                log.Info($"\n tart execution {ctx.InstanceId}");
                List<string> files = await ctx.CallActivityAsync<List<string>>(
                    "GetBlobNames",
                    ctx.InstanceId);
                var kk = files;

                var tasks = new Task<long>[files.Count];
                for (int i = 0; i < files.Count; i++)
                {
                    tasks[i] = ctx.CallActivityAsync<long>(
                        "CopyBlob",
                        files[i]);
                }

                await Task.WhenAll(tasks);
                log.Info($"\n *******************End execution {ctx.InstanceId}******************");
            }
            catch (Exception ex)
            {
                log.Error($"**********ERROR General execution:*********", ex.InnerException != null ? ex.InnerException : ex);
            }
        }

        [FunctionName("GetBlobNames")]
        public static async Task<List<string>> GetBlobNames([ActivityTrigger] string instanceId,
           TraceWriter log)
        {
            log.Info("\n GetBlobNames");

            CloudBlobContainer container = await GetContainer("AzureWebJobsStorage", ConfigurationManager.AppSettings["SourceContainername"]);
            return container.ListBlobs().OfType<CloudBlockBlob>().Select(b => b.Name).ToList();
        }

        private static async Task<CloudBlobContainer> GetContainer(string storageNameConnection, string containerName)
        {
            var storageConnectionString = ConfigurationManager.AppSettings[storageNameConnection];
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }

        public static async Task<CloudBlockBlob> GetBlob(string storageNameConnection, string containerName, string blobName)
        {
            CloudBlobContainer container = await GetContainer(storageNameConnection, containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            return blob;
        }

        [FunctionName("CopyBlob")]
        public static async Task<string> CopyBlob(
          [ActivityTrigger] string blobName,
          TraceWriter log)
        {
            if (!string.IsNullOrEmpty(blobName))
            {
                CloudBlockBlob sourceBlob = await GetBlob("AzureWebJobsStorage", ConfigurationManager.AppSettings["SourceContainername"], blobName);
                CloudBlockBlob destinationBlob = await GetBlob("StorageDestination", ConfigurationManager.AppSettings["DestinationsContainername"], blobName);
                log.Info($"\nBlob: {blobName} Transfer started...\n");

                try
                {
                    if (!destinationBlob.Exists())
                    {
                        await TransferManager.CopyAsync(sourceBlob, destinationBlob, true);
                    }
                    log.Info($"\nBlob: {blobName} Transfer operation completed");
                    return blobName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nBlob: {blobName}  The transfer is canceled: {0}", ex.Message);
                    throw;
                }
            }
            return string.Empty;
        }
    }
}