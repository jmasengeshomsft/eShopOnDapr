using Azure.AI.OpenAI;
using ProductGen.Extensions;
using Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EshopOnAI.ProductGenerator.Services;
using Newtonsoft.Json;
using EshopOnAI.ProductGenerator.Models;

namespace EshopOnAI.ProductGenerator
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            ServiceProvider serviceProvider = ConfigureServices();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var chatService = serviceProvider.GetRequiredService<IChatService>();
            var imageGeneratorService = serviceProvider.GetRequiredService<IImageGeneratorService>();
            var catalogService = serviceProvider.GetRequiredService<ICatalogService>();


            var settingPrompt = config["AIProducctGeneratorPrompt"];
            // var chatPrompt = """ 
                            //   Suggest 3 african animals
                            //   For each, create two merchandise items that could be sold on an ecommerce website store. 
                            //   Each merchandise should have a short cool name, description and type like mug, shirt or hoodie.
                            //   Use the description and create a prompt for each merchandise item that can be used to generate an image. The prompt should be a short sentence that describes the merchandise item. All the background for the image should be transparent.
                            //   Return a valid json object called suggestions containing each suggestion with a random integer id [id], a name [name], and a list of uniquely identifiable merchandise items [merchandises] for that suggestion. Each merchandize has the service name [brand], name [name], description [description], a prompt [prompt], a type [type], a global random unique integer id [id], a decimal price [price] and an integer stock [availableStock] to each merchandise item.
            //                 """;

            // Chat responses are returned as a list of strings.
            //Completions chatResponse = await chatService.GetChatResponseAsync(chatPrompt);

            var newResponse = await chatService.GetChatCompletionsAsync(settingPrompt);

            

            // Print the chat response to console :
           //var merchandises = chatResponse.Choices.Select(choice => JsonConvert.DeserializeObject<Root>(choice.Text)).ToList();

           //Console.WriteLine("The number of choises is " + chatResponse.Choices.Count);
           

            foreach (ChatChoice choice in newResponse.Choices)
            {  
                //Console.WriteLine("---New Utem----");
                Console.WriteLine(choice.Message.Content);
                var content = JsonConvert.DeserializeObject<Root>(choice.Message.Content);
                // //generate a timestamp to use as a unique file name

                var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), config["LocalImageFolder"]);
                if (!Directory.Exists(imageFolderPath))
                {
                    Directory.CreateDirectory(imageFolderPath);
                }

                foreach (var suggestion in content.suggestions)
                {
                    Console.WriteLine("Project: " + suggestion.name);
                    foreach (var merch in suggestion.merchandises)
                    {
                        Console.WriteLine("Name: " + merch.name);
                        Console.WriteLine("Description: " + merch.description);
                        Console.WriteLine("Type: " + merch.type);
                        Console.WriteLine("Prompt: " + merch.prompt);
                        Uri imageUri = await imageGeneratorService.GenerateImageAsync(merch.prompt);
                        var imageBytes = await imageGeneratorService.GetImageFromUrlAsync(imageUri.ToString(), Path.Combine(Directory.GetCurrentDirectory(), config["LocalImageFolder"]), merch.id + ".png");
                        Console.WriteLine(imageUri);
                    }
                }

                string fileName = $"rawcontent.json";
                string jsonFilePath = Path.Combine(imageFolderPath, fileName);
                //File.WriteAllText(jsonFilePath, choice.Text);

                var brands = await catalogService.GetProjectsAsync(content.suggestions);
                //save brands as json into a file 
                fileName = $"brands.json";
                jsonFilePath = Path.Combine(imageFolderPath, fileName);
                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(brands));

                var types = await catalogService.GetCatalogTypesAsync(content.suggestions);
                //save types as json into a file
                fileName = $"types.json";
                jsonFilePath = Path.Combine(imageFolderPath, fileName);
                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(types));

                var items = await catalogService.GetCatalogItemsAsync(content.suggestions);
                //save items as json into a file
                fileName = $"items.json";
                jsonFilePath = Path.Combine(imageFolderPath, fileName);
                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(items));
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddAzureServices()
                .AddSingleton<IChatService, ChatService>()
                .AddSingleton<IImageGeneratorService, DalleService>()
                .AddSingleton<ICatalogService, CatalogService>()
                .BuildServiceProvider();
            return serviceProvider;
        }
    }
}
