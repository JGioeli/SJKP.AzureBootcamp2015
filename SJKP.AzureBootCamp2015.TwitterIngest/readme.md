Get Started with Twitter Stream API
1) Create an twitter developer account
2) Create a new twitter app
3) Make sure to supply a callback url
4) Copy the Consumer Key (API Key) to the appsetting twitterkey
5) Copy the Consumer Secret (API Secret) to the appsetting: twittersecret

Resource:
https://tweetinvi.codeplex.com/documentation

Get Started with Azure Event Hubs
1) Create a new servicebus namespace (use portal or use the following azure powershell commands)
2) Create namespace: New-AzureSBNamespace -Name sjkpazurebootcamp -Location "North Europe" -NamespaceType Messaging -CreateACSNamespace $false
3) Copy the ConnectionString from the powershell to the appsetting: Microsoft.ServiceBus.ConnectionString


Source: 
http://blogs.msdn.com/b/paolos/archive/2014/12/01/how-to-create-a-service-bus-namespace-and-an-event-hub-using-a-powershell-script.aspx


Get started with Stream Analytics 
Part 1
1) Create a new steam analytics service (from the old management portal)
2) Create an input which is pointing to the Event Hub Created earlier
3) Create an output e.g. cloud table
4) Fill your Event Hub with some data and use the Download Sample button from the input to get a json file you use for testing your query. E.g. take a look at the sample Query NumberOfTweetsPer10Seconds.sql
5) Once you are satisfied with the query you can save it, and start the stream service, and it will push data to the cloud table. 

If you experience conversion errors , make sure that you can download the sample data. If not you need to invesigate the data you send to event hub and make sure that
it is support data types. 

Resource:
Stream Analytics Query Language: https://msdn.microsoft.com/en-us/library/azure/dn834998.aspx
Sentiment140 Analysis Example: http://azure.microsoft.com/da-dk/documentation/articles/stream-analytics-twitter-sentiment-analysis-trends/ 

Part 2 (Prerequisites to Data Factory)
1) Create a new output this time a blob storage, in the same storage account as before
2) Name it tweetsblob and make a container called tweet use filename prefix tweet
3) Select CSV as output format and use comma as delimiter


Get Started With Data Factory 

1) Signup the preview of Azure Data Factory 



Create Custom Action
1) Create a new class library
2) Install the following packages:
    Install-Package Microsoft.Azure.Management.DataFactories –Pre
    Install-Package Microsoft.DataFactories.Runtime –Pre
    Install-Package Azure.Storage
3) Create a class that inherits from Microsoft.DataFactories.Runtime.IDotNetActivity and implement the Execute method
4) The SJKP.AzurebootCamp.DataFactoryActivity class library contains an example the following steps explain how to deploy the HttpPostActivity
5) Build the project and make a zip all files in bin/debug or bin/release
6) Upload the zip to azure storage, e.g. mine are stored in container named activities

To debug an pipeline activity i found it easiest to call the following powershell
Switch-AzureMode AzureResourceManager
Add-AzureAccount

Get-AzureDataFactory <resourcegroupname>

To force an activity to run use:
Set-AzureDataFactoryPipelineActivePeriod -ResourceGroupName sjkpazurebootcamp2015 -PipelineName copyblob -DataFactoryName sjkpazurebootcamp2015 -StartDateTime ([System.DateTime]::UtcNow.AddMinutes(-15)) -EndDateTime ([System.DateTime]::UtcNow.AddMinutes(15)) -ForceRecalculate
Set-AzureDataFactoryPipelineActivePeriod -ResourceGroupName <resourceGroupName> -PipelineName <activityName> -DataFactoryName <dataFactoryName> -StartDateTime ([System.DateTime]::UtcNow.AddMinutes(-15)) -EndDateTime ([System.DateTime]::UtcNow.AddMinutes(15)) -ForceRecalculate


Resources:
Samples: https://github.com/Azure/Azure-DataFactory/tree/master/Samples 
Azure Documentation: http://azure.microsoft.com/da-dk/documentation/articles/?service=data-factory
Data Factory JSON Scripting Reference: https://msdn.microsoft.com/en-us/library/azure/dn835050.aspx
