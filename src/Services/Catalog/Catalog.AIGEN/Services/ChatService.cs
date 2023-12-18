

//create an interface for the chat service for OpenAI service
using Azure;
using Azure.AI.OpenAI;

namespace EshopOnAI.ProductGenerator.Services
{
    public class ChatService : IChatService
    {
        
        private readonly OpenAIClient _openAIClient;

        public ChatService(OpenAIClient openAIClient)
        {
            _openAIClient = openAIClient;
        }

        public async Task<ChatCompletions> GetChatCompletionsAsync(string prompt)
        {
            var response = await _openAIClient.GetChatCompletionsAsync(new ChatCompletionsOptions()
            {
                MaxTokens = 2000,
                DeploymentName = "chat",
                Messages = 
                {
                    
                    new ChatMessage(ChatRole.System, "You are an agent that helps people to design merchandise for their ecommerce store. Your response is passed to other system as valid JSON"),
                    //new ChatMessage(ChatRole.System, "You are an agent that helps people to design merchandise for their ecommerce store. Your response is passed to other system as valid JSON"),
                    new ChatMessage(ChatRole.User, prompt)
                }
            });

            return response.Value;
        }

        public async Task<Completions> GetChatResponseAsync(string prompt)
        {

            Response<Completions> response = await _openAIClient.GetCompletionsAsync(new CompletionsOptions()
            {
                DeploymentName = "gpt-35-turbo-instruct", // assumes a matching model deployment or model name
                Prompts = { prompt },
                MaxTokens = 2000
            });

            return response.Value;
        }
    }
}