using Microsoft.SemanticKernel;
using SemanticKernelStarWars.Models;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SemanticKernelStarWars.Plugins.StarWarsPlugin
{
    public class StarWarsWiki
    {
        string baseUrl = "https://swapi.dev/api";

        private HttpClient _httpClient;

        public StarWarsWiki()
        {
            _httpClient = new();
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        [SKFunction, Description("Get the birth year of a Star Wars character")]
        public async Task<string> GetCharacter(string name)
        {
            string request = $"https://swapi.dev/api/people/?search={name}";
            var results = await _httpClient.GetFromJsonAsync<StarWarsResults>(request);
            if (results.count == 0)
            {
                return "No characters found";
            }
            else 
            {
                var character = $"{results.results.FirstOrDefault().name}: Born on {results.results.FirstOrDefault().birth_year}";
                return character;
            }
            
        }
    }
}
