using System;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;
public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    //private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorageConnectionString") ?? configuration["AzureBlobStorage:AzureBlobStorageConnectionString"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("De verbinding is niet juist geconfigureerd");
        }

        //_containerName = configuration["AzureBlobStorage:ContainerName"] ?? throw new Exception("De containernaam is niet juist geconfigureerd");

        

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    // Methode om bestand naar Blob Storage te uploaden
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("products");
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

    //methode om een bestand uit de blob storage te verwijderen
    public async Task<bool> DeleteFileAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("products");
            var blobClient = containerClient.GetBlobClient(fileName);

            //delete het bestand uit de bob client
            var deleted = await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);

            return deleted;
        } catch(Azure.RequestFailedException ex)
        {
            throw new Exception($"Fout bij het verwijderen van bestand {fileName}: {ex.Message}");
        }
        
    }
}
