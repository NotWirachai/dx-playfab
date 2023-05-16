namespace dx_backoffice.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dx_backoffice.Model;

    public class LoginInputModel
    {
        public string? CustomId { get; set; }

        public bool? CreateAccount { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class Player : ControllerBase
    {
        [HttpPost("collectionId")]
        public async Task<dynamic> GetInventoryCollectionIdsAsync(HttpClient httpClient, [FromHeader(Name = "X-EntityToken")] string entityToken, [FromBody] string entityId)
        {
            string titleId = "2506C";
            string url = $"https://{titleId}.playfabapi.com/Inventory/GetInventoryCollectionIds";

            var data = new Dictionary<string, object>
    {
        { "Count", 1 },
    };

            string json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-EntityToken", entityToken);

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResult = JsonSerializer.Deserialize<dynamic>(responseBody);

            StateModel stateModel = new StateModel();

            stateModel.GenerateUniqueId();

            string randomState = stateModel.GetRandomState();


            if (jsonResult.ValueKind == JsonValueKind.Object)
            {
                if (jsonResult.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Object)
                {
                    if (dataElement.TryGetProperty("CollectionIds", out JsonElement collectionIdsElement) && collectionIdsElement.ValueKind == JsonValueKind.Array && collectionIdsElement.GetArrayLength() > 0)
                    {
                        // CollectionIds has values
                        Console.WriteLine("CollectionIds has values");
                        return Results.Json(jsonResult);
                    }
                    else
                    {
                        // CollectionIds is empty
                        Console.WriteLine("CollectionIds is empty");

                        var requestBody = new Dictionary<string, object>
                    {
                        { "Amount", 1 },
                        { "Entity", new Dictionary<string, object>
                            {
                                { "Id", entityId },
                                { "Type", "title_player_account" },
                                { "TypeString", "title_player_account" }
                            }
                        },
                        { "Item", new Dictionary<string, object>
                            {
                                { "Id", "fbfe99f5-f593-4577-b9b4-865386073b30" },
                                { "Type ", "bundle" },
                            }
                        },
                        {"CollectionId", randomState+"-"+stateModel.uniqueId }
                    };

                        string collectionJson = JsonSerializer.Serialize(requestBody);
                        var collectionContent = new StringContent(collectionJson, Encoding.UTF8, "application/json");

                        string urlCollectionIds = $"https://{titleId}.playfabapi.com/Inventory/AddInventoryItems";
                        HttpResponseMessage responseCollectionIds = await httpClient.PostAsync(urlCollectionIds, collectionContent);
                        string responseBodyCollectionIds = await responseCollectionIds.Content.ReadAsStringAsync();
                        dynamic jsonResultCollectionIds = JsonSerializer.Deserialize<dynamic>(responseBody);
                        return Results.Json(jsonResultCollectionIds);
                        // return new JsonResult("CollectionIds is empty");
                    }
                }
                else
                {
                    // CollectionIds is missing
                    return new JsonResult("CollectionIds is missing");

                }
            }
            else
            {
                // Invalid response format
                return new JsonResult("Invalid response format");
            }
        }

        [HttpPost("Login")]
        public async Task<dynamic> LogInCustomIdAsync(HttpClient httpClient, [FromHeader(Name = "X-SecretKey")] string SecretKey, [FromBody] LoginInputModel loginInputModel )
        {
            string titleId = "2506C";
            string url = $"https://{titleId}.playfabapi.com/Client/LoginWithCustomID";

            var data = new Dictionary<string, object>  
            {
                { "TitleId", titleId },
                { "CustomId", loginInputModel.CustomId },
                { "CreateAccount", loginInputModel.CreateAccount },
            };
            string json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-SecretKey", SecretKey);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResult = JsonSerializer.Deserialize<dynamic>(responseBody);

            return jsonResult;
        }
    }

}
