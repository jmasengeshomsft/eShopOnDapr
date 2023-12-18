//create an c# interface for the image generator service
namespace EshopOnAI.ProductGenerator.Services
{
    public interface IImageGeneratorService
    {
        Task<Uri> GenerateImageAsync(string prompt);
        //declare a method that save a image in memory from a url
        Task<string> GetImageFromUrlAsync(string uri, string localFolderPath, string fileName);
    }
}