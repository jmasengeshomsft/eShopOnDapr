//implement the interface IImageGeneratorService
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EshopOnAI.ProductGenerator.Services
{
    public class DalleService : IImageGeneratorService
    {
        private readonly OpenAIClient _openAIClient;
        // private readonly ILogger<DalleService> _logger;

        public DalleService(OpenAIClient openAIClient)
        {
            _openAIClient = openAIClient;
            // _logger = logger;
        }

        public async Task<Uri> GenerateImageAsync(string prompt)
        {
            Response<ImageGenerations> imageGenerations = await _openAIClient.GetImageGenerationsAsync(
                new ImageGenerationOptions()
                {
                    Prompt = prompt,
                    Size = ImageSize.Size256x256,
                });

            // Image Generations responses provide URLs you can use to retrieve requested images
            Uri imageUri = imageGenerations.Value.Data[0].Url;

            return imageUri;
        }

        public async Task<string> GetImageFromUrlAsync(string fileUrl, string localFolderPath, string fileName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Download the file as a byte array
                byte[] fileBytes = await httpClient.GetByteArrayAsync(fileUrl);

                if (!Directory.Exists(localFolderPath))
                {
                    Directory.CreateDirectory(localFolderPath);
                }

                string filePath = Path.Combine(localFolderPath, fileName);

                // Save the image to the local folder
                using (FileStream fileStream = File.Create(filePath))
                {
                    await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                return filePath;
            }
        }
    }
}
