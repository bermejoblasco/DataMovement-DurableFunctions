In this ample, we can see how move files from one Blob Storage to another in a diferent region 
with Auzre Data Movement Library and Azure Durable Functions.

## Running this sample

To run this sample:

1. Create two [Azure Storage Account](https://docs.microsoft.com/en-us/azure/storage/storage-create-storage-account)
2. Install [Visual Studio 2017](https://www.visualstudio.com/en/downloads/)
3. Clone the repo. Opne it in VS2017.
4. In Blob origin Blob Storage, create a Queue wiht name datatransfer.
5. Create an [Azure Function App](https://docs.microsoft.com/en-US/azure/azure-functions/functions-create-first-azure-function)
6. Add/Modify AppSettings 
  6.1. "AzureWebJobsStorage" --> Add Origin Blob Storage connectionstring 
  6.2. "AzureWebJobsStorage" -->  Add Origin Blob Storage connectionstring 
  6.3. "StorageDestination" --> Add Destination Blob Storage connectionstring 
  6.4. "SourceContainername" --> Add name source container in Origin Blob. 
  6.5. "DestinationsContainername" --> Add name destination container in Destination Blob. 
7. Deploy Azure Function From Vs2017 to the Azure Function App.
8. Add a message in Queue create in point 4. Any message.
