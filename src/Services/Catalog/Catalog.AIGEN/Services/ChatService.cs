

//create an interface for the chat service for OpenAI service
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace EshopOnAI.ProductGenerator.Services
{

    public class ChatService : IChatService
    {
        
        private readonly AzureOpenAIClient  _openAIClient;
        //private readonly ChatClient _chatClient;  

        public ChatService(AzureOpenAIClient  openAIClient)
        {
            _openAIClient = openAIClient;
           // _chatClient = 
        }

        public async Task<string> GetChatCompletionsAsync(string prompt)
        {
        //     // var response = await _openAIClient.GetChatCompletionsAsync(new ChatCompletionsOptions()
        //     // {
        //     //     MaxTokens = 2000,
        //     //     DeploymentName = "chat",
        //     //     Messages = 
        //     //     {
        //     //          new ChatMessage(ChatRole.System, "You are an agent that helps people to design merchandise for their ecommerce store. Your response is passed to other system as valid JSON"),
        //     //          new ChatMessage(ChatRole.User, prompt),

        //     //     }
        //     // });

        //     var chatClient = _openAIClient.GetChatClient("my-gpt-35-turbo-deployment");

        //     var completion = chatClient.CompleteChat(
        //             new SystemChatMessage("You are an agent that helps people to design merchandise for their ecommerce store. Your response is passed to other system as valid JSON"),
        //             new UserChatMessage(prompt)
        //     );


        //     return completion.Content[0].Text;

        return null;
        }

        // public async Task<Completions> GetChatResponseAsync(string prompt)
        // {

        //     Response<Completions> response = await _openAIClient.GetCompletionsAsync(new CompletionsOptions()
        //     {
        //         DeploymentName = "gpt-35-turbo-instruct", 
        //         Prompts = { prompt },
        //         MaxTokens = 2000
        //     });

        //     return response.Value;
        // }

        // public async Task<Completions> GetChatResponseAsync(string prompt)
        // {

        //     Response<Completions> response = await _openAIClient.GetCompletionsAsync(new CompletionsOptions()
        //     {
        //         DeploymentName = "gpt-35-turbo-instruct", 
        //         Prompts = { prompt },
        //         MaxTokens = 2000
        //     });

        //     return response.Value;
        // }

        
    }
}