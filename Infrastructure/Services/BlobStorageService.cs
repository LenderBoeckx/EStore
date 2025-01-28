using System;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorageConnectionString") ?? configuration["AzureBlobStorage:AzureBlobStorageConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"]!;

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(_containerName))
        {
            throw new ArgumentException("De verbinding of containernaam is niet juist geconfigureerd.");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    // Methode om bestand naar Blob Storage te uploaden
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        if (await blobClient.ExistsAsync())
        {
            return "Er is al een afbeelding met dezelfde naam in de opslag.";
        }

        // Upload het bestand naar de blob storage
        await blobClient.UploadAsync(fileStream, overwrite: true);

        // Retourneer de URL van het bestand in Blob Storage
        return blobClient.Uri.ToString();
    }
}
