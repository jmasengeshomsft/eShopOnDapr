//create an interface for the chat service for OpenAI service
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace EshopOnAI.ProductGenerator.Services
{
    public interface IChatService
    {
       // Task<Completions> GetChatResponseAsync(string prompt);
        Task<string> GetChatCompletionsAsync(string prompt);
    }
}