//implement the interface IImageGeneratorService
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace EshopOnAI.ProductGenerator.Services
{
    public class DalleService : IImageGeneratorService
    {
        private readonly AzureOpenAIClient  _openAIClient;
        // private readonly ILogger<DalleService> _logger;

        public DalleService(AzureOpenAIClient  openAIClient)
        {
            _openAIClient = openAIClient;
            // _logger = logger;
        }

        public async Task<Uri> GenerateImageAsync(string prompt)
        {

            // var options = new ImageGenerationOptions()
            // {
            //     Prompt = prompt,
            //     Size = ImageSize.Size256x256,
            //    // DeploymentName = "dall-e-3",
            // }; 

            ImageClient chatClient = _openAIClient.GetImageClient("dalle-3");


//             Response<ImageGenerations> imageGenerations = await _openAIClient.GetImageGenerationsAsync(options);
// r
//             // Image Generations responses provide URLs you can use to retrieve requested images
//             Ui imageUri = imageGenerations.Value.Data[0].Url;


            var imageGeneration = await chatClient.GenerateImageAsync(
                prompt,
                new ImageGenerationOptions()
                {
                    Size = GeneratedImageSize.Size256x256
                }
           );

            //save this image to a azure blob
            

            return imageGeneration.Value.ImageUri;
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
        }     //create a method to upload the image to azure b
    
    }
}
