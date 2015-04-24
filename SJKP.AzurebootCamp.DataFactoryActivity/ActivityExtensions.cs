using Microsoft.Azure.Management.DataFactories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.AzurebootCamp.DataFactoryActivity
{
    public static class ActivityExtensions
    {
        /// <summary>
        /// Get the connection string from an azurestoragelinkedservice.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetConnectionString(this LinkedService asset)
        {
            AzureStorageLinkedService storageAsset;
            if (asset == null)
            {
                return null;
            }

            storageAsset = asset.Properties as AzureStorageLinkedService;
            if (storageAsset == null)
            {
                return null;
            }

            return storageAsset.ConnectionString;
        }

        /// <summary>
        /// Get the folder path of a table that access data in azure blob storage.
        /// </summary>
        /// <param name="dataArtifact"></param>
        /// <returns></returns>
        public static string GetFolderPath(this Table dataArtifact)
        {
            AzureBlobLocation blobLocation;
            if (dataArtifact == null || dataArtifact.Properties == null)
            {
                return null;
            }

            blobLocation = dataArtifact.Properties.Location as AzureBlobLocation;
            if (blobLocation == null)
            {
                return null;
            }

            return blobLocation.FolderPath;
        }

        public static string GetTableName(this Table dataArtifact)
        {
            AzureTableLocation table;
            if (dataArtifact == null || dataArtifact.Properties == null)
            {
                return null;
            }

            table = dataArtifact.Properties.Location as AzureTableLocation;
            if (table == null)
            {
                return null;
            }

            return table.TableName;
        }
    }
}
