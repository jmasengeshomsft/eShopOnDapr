// Copyright (c) Microsoft. All rights reserved.

using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductGen.Extensions;

internal static class ServiceCollectionExtensions
{
    private static readonly DefaultAzureCredential s_azureCredential = new();

    internal static IServiceCollection AddAzureServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<BlobServiceClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureStorageAccountEndpoint = config["AzureStorageAccountEndpoint"];
            ArgumentNullException.ThrowIfNullOrEmpty(azureStorageAccountEndpoint);

            var blobServiceClient = new BlobServiceClient(
                new Uri(azureStorageAccountEndpoint), s_azureCredential);

            return blobServiceClient;
        });

        _ = services.AddSingleton<BlobContainerClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureStorageContainer = config["AzureStorageContainer"];
            return sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(azureStorageContainer);
        });

        
        services.AddSingleton<OpenAIClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureOpenAiServiceEndpoint = config["AzureOpenAiServiceEndpoint"];
            var azureOpenAiServiceKey = config["AzureOpenAiServiceKey"];

            ArgumentNullException.ThrowIfNullOrEmpty(azureOpenAiServiceEndpoint);
            ArgumentNullException.ThrowIfNullOrEmpty(azureOpenAiServiceKey);

            var openAIClient = new OpenAIClient(
                new Uri(azureOpenAiServiceEndpoint), new AzureKeyCredential(azureOpenAiServiceKey));

            return openAIClient;
        });

        // services.AddSingleton<AzureBlobStorageService>();
        // services.AddSingleton<ReadRetrieveReadChatService>();

        return services;
    }
}
