// See https://aka.ms/new-console-template for more information


using Microsoft.SemanticKernel;
using System.Text.Json;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Planners;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("55cd6791-c8af-40dc-aeeb-c73182914565")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

string bing_key = configuration["Bing:ApiKey"];


var kernelBuilder = new KernelBuilder();
kernelBuilder.WithAzureChatCompletionService(deploymentName, endpoint, apiKey);

var kernel = kernelBuilder.Build();

kernel.ImportSemanticFunctionsFromDirectory(@"D:\src\junk\SemanticKernelTest\skills", "MailSkill");
kernel.ImportSemanticFunctionsFromDirectory(@"D:\src\junk\SemanticKernelTest\skills", "TechSkill");
var bingConnector = new BingConnector(bing_key);
kernel.ImportFunctions(new WebSearchEnginePlugin(bingConnector), "bing");


var planner = new SequentialPlanner(kernel);
var ask = "Write a mail to propose a presentation aboout using semantic kernel from Microsoft to build AI applications at a technical conference. The mail should be clear and concise. Use the web to get recent information on the topic.";
var originalPlan = await planner.CreatePlanAsync(ask);
Console.WriteLine("Updated plan:\n");
Console.WriteLine(JsonSerializer.Serialize(originalPlan, new JsonSerializerOptions { WriteIndented = true }));

var originalPlanResult = await kernel.RunAsync(originalPlan);

Console.WriteLine(originalPlanResult.GetValue<string>());
Console.ReadLine();