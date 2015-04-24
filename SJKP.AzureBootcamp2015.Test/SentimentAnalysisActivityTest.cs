using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SJKP.AzurebootCamp.DataFactoryActivity;
using Microsoft.WindowsAzure.Storage;

namespace SJKP.AzureBootcamp2015.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PositiveTest()
        {
            var ananlyzer = new SentimentAnalysisActivity();

            var score = ananlyzer.GetScore("https://api.datamarket.azure.com/data.ashx/aml_labs/lexicon_based_sentiment_analysis/v1/Score", "blasimtex@hotmail.com", "ZPSUqF6HZ9eAh3St7sENsEM2YfLnsLeIKfj5wUouTNM=", "Today is a good day and the weather is nice");
            Console.WriteLine(score);
            Assert.IsTrue(score > 0.0);
        }

        [TestMethod]
        public void NegativeTest()
        {
            var ananlyzer = new SentimentAnalysisActivity();

            var score = ananlyzer.GetScore("https://api.datamarket.azure.com/data.ashx/aml_labs/lexicon_based_sentiment_analysis/v1/Score", "blasimtex@hotmail.com", "ZPSUqF6HZ9eAh3St7sENsEM2YfLnsLeIKfj5wUouTNM=", "Today is a crap day and the weather is bad");
            Console.WriteLine(score);
            Assert.IsTrue(score < 0.0);
        }

        [TestMethod]
        public void ProcessTweetTest()
        {
            var ananlyzer = new SentimentAnalysisActivity();
            ananlyzer.apikey = "";
            ananlyzer.email = "blasimtex@hotmail.com";
            ananlyzer.url = "https://api.datamarket.azure.com/data.ashx/aml_labs/lexicon_based_sentiment_analysis/v1/Score";
            ananlyzer.logger = new ConsoleLogger();

            var storage = CloudStorageAccount.Parse(System.Configuration.ConfigurationSettings.AppSettings.Get("ConnectionString"));

            var blobClient = storage.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("ouputtweets");

            var blob = container.GetBlockBlobReference("tweet/Data.662d7686-b57d-4f1e-b1a8-398e42241d57.txt");
            Assert.IsTrue(blob.Exists());
            var tableClient = storage.CreateCloudTableClient();
            var testTable = tableClient.GetTableReference("testtable");
            testTable.CreateIfNotExists();
            ananlyzer.ProcessTweetBlob(blob, testTable, "tweet");
        }
    }
}
