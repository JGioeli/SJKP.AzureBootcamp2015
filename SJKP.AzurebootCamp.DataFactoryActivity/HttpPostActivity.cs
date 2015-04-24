using Microsoft.Azure.Management.DataFactories.Models;
using Microsoft.DataFactories.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SJKP.AzurebootCamp.DataFactoryActivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.AzurebootCamp.DataFactoryActivity
{
    public class HttpPostActivity : IDotNetActivity
    {
        private IActivityLogger logger;
        public IDictionary<string, string> Execute(IEnumerable<ResolvedTable> inputTables, IEnumerable<ResolvedTable> outputTables, IDictionary<string, string> properties, IActivityLogger logger)
        {
            this.logger = logger;
            try
            {
                var requestBin = properties["requestBinUrl"];
                logger.Write(TraceEventType.Information, "RequestBinURL {0}", requestBin);

                var sliceStartTime = properties["sliceStart"];
                var startTime = ParseSliceStartTime(sliceStartTime);

                using (HttpClient client = new HttpClient())
                {
                    foreach (var table in inputTables)
                    {
                        var connectionString = table.LinkedService.GetConnectionString();
                        var folder = table.Table.GetFolderPath();

                        if (folder == null || connectionString == null)
                            continue;
                        BlobContinuationToken continuationToken = null;
                        CloudStorageAccount inputStorageAccount = CloudStorageAccount.Parse(connectionString);
                        CloudBlobClient inputClient = inputStorageAccount.CreateCloudBlobClient();
                        string output = string.Empty;
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
                                int count = 0;
                                if (inputBlob != null)
                                {
                                    using (StreamReader sr = new StreamReader(inputBlob.OpenRead()))
                                    {
                                        while (!sr.EndOfStream)
                                        {
                                            string line = sr.ReadLine();
                                            if (count == 0)
                                            {
                                                logger.Write(TraceEventType.Information, "First line: [{0}]", line);
                                            }
                                            count++;
                                        }

                                    }

                                }
                                output += string.Format(CultureInfo.InvariantCulture,
                                                "{0},{1},{2},{3},{4}\n",
                                                folder,
                                                inputBlob.Name,
                                                count,
                                                Environment.MachineName,
                                                DateTime.UtcNow);

                            }
                            continuationToken = result.ContinuationToken;

                        } while (continuationToken != null);

                        var task = client.PostAsync(requestBin, new StringContent(output));
                        Task.WaitAll(task);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Write(TraceEventType.Error, ex.ToString());
            }
            return new Dictionary<string, string>();
        }
     

        private DateTime ParseSliceStartTime(string sliceStartTime)
        {
            logger.Write(TraceEventType.Information, "Slidestart string {0}", sliceStartTime);
            string year = sliceStartTime.Substring(0, 4);
            string month = sliceStartTime.Substring(4, 2);
            string day = sliceStartTime.Substring(6, 2);
            string hour = sliceStartTime.Substring(8, 2);
            string minute = sliceStartTime.Substring(10, 2);
            DateTime dataSlotGathered = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), 0);

            logger.Write(TraceEventType.Information, "SliceStartTime: {0}.......", dataSlotGathered);
            return dataSlotGathered;
        }
    }

}
