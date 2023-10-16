// See https://aka.ms/new-console-template for more information

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Planning;

using Microsoft.SemanticKernel.Skills.Web;
using Microsoft.SemanticKernel.Skills.Web.Bing;

using System.Text.Json;
using Microsoft.SemanticKernel.Connectors.Memory.Pinecone;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.AzureSdk;
using Microsoft.SemanticKernel.SemanticFunctions;
using static Microsoft.SemanticKernel.SemanticFunctions.PromptTemplateConfig;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("554883b0-be69-4d93-bc77-c73005be38e2")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

string pineconeEnv = configuration["Pinecone:Environment"];
string pineconeApiKey = configuration["Pinecone:ApiKey"];

var kernelBuilder = new KernelBuilder();
kernelBuilder.WithAzureTextEmbeddingGenerationService("text-embedding-ada-002", endpoint, apiKey);
kernelBuilder.WithAzureChatCompletionService(deploymentName, endpoint, apiKey);
kernelBuilder.WithMemoryStorage(new PineconeMemoryStore(pineconeEnv, pineconeApiKey));


var kernel = kernelBuilder.Build();

const string MemoryCollectionName = "semantickernel";

//await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info1", text: "Semantic Kernel is a library to orchestrate AI tasks in an application.");
//await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info2", text: "Semantic Kernel is a library built by Microsoft");
//await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "info3", text: "Semantic Kernel is available in C#, Java and Python");

string ask = "Tell me more about Semantic Kernel";

var memories = kernel.Memory.SearchAsync(MemoryCollectionName, ask, limit: 5, minRelevanceScore: 0.77);

string skPrompt = @"
Give me an overview of Semantic Kernel. Use only the information below as a reference. If you don't know the answer, say you don't know.

{{$input}}

";

string input = string.Empty;

await foreach (MemoryQueryResult memory in memories)
{
    input += @$"{memory.Metadata.Text}

    ";
}


var promptConfig = new PromptTemplateConfig
{
    Schema = 1,
    Type = "completion",
    Description = "Gets the intent of the user.",
    Input =
     {
        Parameters = new List<InputParameter>
        {
            new InputParameter
            {
                Name = "input",
                Description = "The user's request.",
                DefaultValue = ""
            }
        }
    }
};

var promptTemplate = new PromptTemplate(
    skPrompt,
    promptConfig,
    kernel
);
var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

var getIntentFunction = kernel.RegisterSemanticFunction("OrchestratorPlugin", "GetIntent", functionConfig);


var result = await kernel.RunAsync(
    input,
    getIntentFunction
);

Console.WriteLine(result.GetValue<string>());