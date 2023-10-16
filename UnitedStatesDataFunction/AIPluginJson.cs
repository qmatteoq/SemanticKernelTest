using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using UnitedStatesDataFunction.Models;

namespace UnitedStatesDataFunction
{
    public class AIPluginJson
    {
        [FunctionName("GetAIPluginJson")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/ai-plugin.json")] HttpRequest req)
        {
            var currentDomain = $"{req.Scheme}://{req.Host.Value}";
            var result  = File.ReadAllText(@"D:\src\junk\SemanticKernelTest\UnitedStatesDataFunction\manifest\ai-plugin.json");
            var json = result.Replace("{url}", currentDomain);
            return new OkObjectResult(json);
        }
    }
}
