// See https://aka.ms/new-console-template for more information


using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelStarWars.Models;
using SemanticKernelStarWars.Plugins.StarWarsPlugin;
using SemanticKernelStarWars.Plugins.UnitedStatesPlugin;
using Microsoft.SemanticKernel.Functions.OpenAPI.Extensions;
using Microsoft.SemanticKernel.Orchestration;
using System.Text.Json;
using Microsoft.SemanticKernel.Functions.OpenAPI.Model;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("10e17718-df7d-4824-8ddd-cde6533ba289")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = new KernelBuilder()
    .WithAzureChatCompletionService(deploymentName, endpoint, apiKey)
    .Build();

var mailPlugin = kernel.ImportSemanticFunctionsFromDirectory(@"D:\src\junk\SemanticKernelTest\skills", "MailSkill");

const string pluginManifestUrl = "http://localhost:7098/api/.well-known/ai-plugin.json";
var unitedStatesPlugin = await kernel.ImportPluginFunctionsAsync("UnitedStatesPlugin", new Uri(pluginManifestUrl));

ContextVariables variables = new ContextVariables
{
    { "year", "2015" }
};

var result = await kernel.RunAsync(variables, unitedStatesPlugin["GetUsPopulation"], mailPlugin["MailGeneratorFunction"]);
Console.WriteLine(result.GetValue<string>());

//var planner = new StepwisePlanner(kernel);

//var ask = "Write a mail to share the number of the United States population in 2015 for a research program.";
//var originalPlan = planner.CreatePlan(ask);
////Console.WriteLine("Updated plan:\n");
////Console.WriteLine(JsonSerializer.Serialize(originalPlan, new JsonSerializerOptions { WriteIndented = true }));
////var result = originalPlan.InvokeAsync(kernel.CreateNewContext(variables));

//var originalPlanResult = await kernel.RunAsync(variables, originalPlan);

//Console.WriteLine(originalPlanResult.GetValue<string>());
Console.ReadLine();