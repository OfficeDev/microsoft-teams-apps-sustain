// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Application;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class CloudBlobService: IBlobService
{
    private readonly CloudBlobClient _cloudBlobClient;
    private readonly string _container;
    
    public CloudBlobService(Uri blobStorageUri, string accountName, string key, string container)
    {
        StorageCredentials storageCredentials = new StorageCredentials(accountName, key);
        _cloudBlobClient = new CloudBlobClient(blobStorageUri, storageCredentials);
        _container = container;
    }

    public async Task RemoveFile(string absoluteUri)
    {
        var splitUri = absoluteUri.Split('/');
        var filename = splitUri[splitUri.Length-1];
        //CloudBlockBlob blob = new CloudBlockBlob(new Uri(absoluteUri));

        var container = _cloudBlobClient.GetContainerReference(_container);
        CloudBlockBlob blob = container.GetBlockBlobReference(filename);

        await blob.DeleteIfExistsAsync();
    }

    public async Task<string> UploadFile(Stream stream, string fileName)
    {
        var fileNamePieces = fileName.Split(".");
        string newFilename = $"{fileNamePieces[0]}_{Guid.NewGuid()}.{fileNamePieces[1]}";

        var container = _cloudBlobClient.GetContainerReference(_container);
        var blockBlob = container.GetBlockBlobReference(newFilename);

        using (var newStream = new MemoryStream())
        {
            stream.Position = 0;
            stream.CopyTo(newStream);

            newStream.Position = 0;

            await blockBlob.UploadFromStreamAsync(newStream);
        }

        return blockBlob.Uri.ToString();
    }

    public string GetSasLink(string link)
    {
        CloudBlockBlob cloudBlockBlob = new CloudBlockBlob(new Uri(link), _cloudBlobClient.Credentials);

        SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
        sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60);
        sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

        var sasLink = cloudBlockBlob.Uri + cloudBlockBlob.GetSharedAccessSignature(sasConstraints);

        return sasLink;
    }
}
