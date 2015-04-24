using CsvHelper;
using Microsoft.DataFactories.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.AzurebootCamp.DataFactoryActivity
{
    public class SentimentAnalysisActivity : IDotNetActivity
    {
        public string apikey;
        public string email;
        public IActivityLogger logger;
        public string url;
        /// <summary>
        /// Calls https://datamarket.azure.com/dataset/aml_labs/lexicon_based_sentiment_analysis to calculate a sentiment for a twitter tweet.
        /// Register at the site to get an apikey.
        /// </summary>
        /// <param name="inputTables"></param>
        /// <param name="outputTables"></param>
        /// <param name="properties"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public IDictionary<string, string> Execute(IEnumerable<ResolvedTable> inputTables, IEnumerable<ResolvedTable> outputTables, IDictionary<string, string> properties, IActivityLogger logger)
        {
            this.logger = logger;
            try
            {
                url = properties["url"];
                logger.Write(TraceEventType.Information, "url {0}", url);

                apikey = properties["apikey"];
                logger.Write(TraceEventType.Information, "apikey {0}", apikey);

                email = properties["apikey"];
                logger.Write(TraceEventType.Information, "email {0}", email);
                

                foreach (var table in inputTables)
                {
                    var connectionString = table.LinkedService.GetConnectionString();
                    var folder = table.Table.GetFolderPath();

                    if (folder == null || connectionString == null)
                        continue;
                    BlobContinuationToken continuationToken = null;
                    CloudStorageAccount inputStorageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudBlobClient inputClient = inputStorageAccount.CreateCloudBlobClient();
                    do
                    {
                        BlobResultSegment result = inputClient.ListBlobsSegmented(folder,
                                                    true,
                                                    BlobListingDetails.Metadata,
                                                    null,
                                                    continuationToken,
                                                    null,
                                                    null);
                        foreach (IListBlobItem listBlobItem in result.Results)
                        {
                            CloudBlockBlob inputBlob = listBlobItem as CloudBlockBlob;
                            
                            if (inputBlob != null)
                            {
                                foreach (var outputtable in outputTables)
                                {

                                    var outputstorageaccount = CloudStorageAccount.Parse(outputtable.LinkedService.GetConnectionString());
                                    var tableName = outputtable.Table.GetTableName();
                                    var tableClient = outputstorageaccount.CreateCloudTableClient();
                                    var outputAzureTable = tableClient.GetTableReference(tableName);
                                    outputAzureTable.CreateIfNotExists();
                                    ProcessTweetBlob(inputBlob, outputAzureTable, folder);
                                }
                            }
                        

                        }
                        continuationToken = result.ContinuationToken;

                    } while (continuationToken != null);
                }
            }

            catch (Exception ex)
            {
                this.logger.Write(TraceEventType.Error, ex.ToString());
            }
            return new Dictionary<string, string>();
        }

        public double GetScore(string url, string email, string apiKey, string textToAnalyze)
        {
            using (var wb = new WebClient())
            {
                var acitionUri = new Uri(url);
                DataServiceContext ctx = new DataServiceContext(acitionUri);
                var cred = new NetworkCredential(email, apiKey);
                var cache = new CredentialCache();

                cache.Add(acitionUri, "Basic", cred);
                ctx.Credentials = cache;
                var query = ctx.Execute<ScoreResult>(acitionUri, "POST", true, new BodyOperationParameter("Text", textToAnalyze));
                ScoreResult scoreResult = query.ElementAt(0);
                double result = scoreResult.result;
                return result;
            }
        }

        public void ProcessTweetBlob(CloudBlockBlob inputBlob, CloudTable outputAzureTable, string folder)
        {
            int count = 0;
            List<TweetSentimentScore> scores = new List<TweetSentimentScore>();
            using (var reader = new CsvReader(new StreamReader(inputBlob.OpenRead())))
            {
                while (reader.Read())
                {
                    if (count == 0)
                    {
                        logger.Write(TraceEventType.Information, "First line: [{0}]", string.Join(",", reader.CurrentRecord));
                    }
                    count++;
                    var tweet = reader.GetField(0); //get the tweet
                    var entity = new TweetSentimentScore()
                    {
                        PartitionKey = "tweetsentimentscore",
                        RowKey = Guid.NewGuid().ToString(),
                        Tweet = tweet,
                        SentimentScore = GetScore(url, email, apikey, tweet)
                    };
                    scores.Add(entity);

                    outputAzureTable.Execute(TableOperation.InsertOrReplace(entity)); //Do it one row at a time for demo output

                }
            }

            var iter = scores.Count() / 100;
            for (int i = 0; i <= iter; i++)
            {
                var batchOp = new TableBatchOperation();
                scores.Skip(100 * i).Take(100).ToList().ForEach(a =>
                {
                    batchOp.Add(TableOperation.InsertOrReplace(a));
                });
                //outputAzureTable.ExecuteBatch(batchOp); //Removed for demo purposes.
            }


            logger.Write(TraceEventType.Information, string.Format(CultureInfo.InvariantCulture,
                                "{0},{1},{2},{3},{4}\n",
                                folder,
                                inputBlob.Name,
                                count,
                                Environment.MachineName,
                                DateTime.UtcNow));
        }
    }

  

    public class ScoreResult
    {
        [DataMember]
        public double result
        {
            get;
            set;
        }
    }

    public class TweetSentimentScore : TableEntity
    {

        public string Tweet { get; set; }
        public double SentimentScore { get; set; }
    }
}
