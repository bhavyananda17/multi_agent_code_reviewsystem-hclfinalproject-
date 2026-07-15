using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using OpenAI;
using OpenAI.Chat;

var options = new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai") };
var client = new ChatClient("llama-3.1-8b-instant", "test-key", options);

// Test OpenAIChatAgent
var agent = new OpenAIChatAgent(client, "TestAgent", "You are a test agent", 0.1f, 100);

Console.WriteLine($"Agent type: {agent.GetType().Name}");
Console.WriteLine($"Agent name: {agent.Name}");

// Check what messages it accepts - TextMessage needs Role enum
var textMsg = new TextMessage("Hello", Role.User);
Console.WriteLine($"TextMessage: {textMsg.GetType().Name}");

// Check GenerateReplyAsync
var result = await agent.GenerateReplyAsync(new[] { textMsg }, null, CancellationToken.None);
Console.WriteLine($"Result type: {result.GetType().Name}");

if (result is TextMessage tm)
{
    Console.WriteLine($"Content: {tm.Content}");
}

Console.WriteLine("API exploration complete");