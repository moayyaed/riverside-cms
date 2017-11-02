﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Storage.Infrastructure
{
    public class AzureBlobService : IBlobService
    {
        private readonly IOptions<AzureBlobOptions> _options;

        public AzureBlobService(IOptions<AzureBlobOptions> options)
        {
            _options = options;
        }

        public string GetBlobContainer(Blob blob)
        {
            return string.Format("tenant-{0}", blob.TenantId);
        }

        public string GetBlobName(Blob blob)
        {
            string blobName = string.Join("/", blob.Location);
            if (!string.IsNullOrWhiteSpace(blobName))
                blobName += "/";
            blobName += string.Format("{0}{1}-{2}", blob.UploadId, blob.Name);
            return blobName;
        }

        public async Task<Stream> ReadBlobContentAsync(Blob blob)
        {
            string blobContainer = GetBlobContainer(blob);
            string blobName = GetBlobName(blob);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_options.Value.StorageConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainer);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            Stream target = new MemoryStream();

            await cloudBlockBlob.DownloadToStreamAsync(target);

            return target;
        }
    }
}