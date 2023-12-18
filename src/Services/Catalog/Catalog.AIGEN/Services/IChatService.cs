//create an interface for the chat service for OpenAI service
using Azure.AI.OpenAI;

namespace EshopOnAI.ProductGenerator.Services
{
    public interface IChatService
    {
        Task<Completions> GetChatResponseAsync(string prompt);
        Task<ChatCompletions> GetChatCompletionsAsync(string prompt);
    }
}