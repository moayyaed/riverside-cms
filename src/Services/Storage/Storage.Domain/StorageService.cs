﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Storage.Domain
{
    public class StorageService : IStorageService
    {
        private readonly IBlobService _blobService;
        private readonly IStorageRepository _storageRepository;

        public StorageService(IBlobService blobService, IStorageRepository storageRepository)
        {
            _blobService = blobService;
            _storageRepository = storageRepository;
        }

        public async Task<Blob> ReadBlobAsync(long tenantId, long uploadId)
        {
            return await _storageRepository.ReadBlobAsync(tenantId, uploadId);
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long uploadId)
        {
            Blob blob = await _storageRepository.ReadBlobAsync(tenantId, uploadId);
            BlobContent blobContent = new BlobContent
            {
                Type = blob.ContentType,
                Stream = await _blobService.ReadBlobContentAsync(blob)
            };
            return blobContent;
        }
    }
}