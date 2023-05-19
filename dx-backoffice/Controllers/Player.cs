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
    using dx_backoffice.Models;
    using System.Xml;
    using dx_backoffice.Services;

    [ApiController]
    [Route("[controller]")]
    public class Player : ControllerBase
    {
        [HttpPost("collectionId")]
        public async Task<dynamic> GetInventoryCollectionIdsAsync(HttpClient httpClient, [FromHeader(Name = "X-EntityToken")] string entityToken, [FromBody] CollectionInputModel collectionInputModel)
        {
            string titleId = "2506C";
            string url = $"https://{titleId}.playfabapi.com/Inventory/GetInventoryCollectionIds";

            var data = new Dictionary<string, object>
                {
                    { "Count", 20 },
                };

            string json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-EntityToken", entityToken);

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResult = JsonSerializer.Deserialize<dynamic>(responseBody);

            string latestCollectionId = string.Empty;

            GetCurrentCollectionId getCurrentCollectionId = new GetCurrentCollectionId();

            if (jsonResult.ValueKind == JsonValueKind.Object)
            {
                if (jsonResult.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Object)
                {
                    if (dataElement.TryGetProperty("CollectionIds", out JsonElement collectionIdsElement) && collectionIdsElement.ValueKind == JsonValueKind.Array && collectionIdsElement.GetArrayLength() > 0)
                    {
                        // CollectionIds has values
                        Console.WriteLine("CollectionIds has values");

                        List<string> collectionIds = new List<string>();
                        foreach (var collectionIdElement in collectionIdsElement.EnumerateArray())
                        {
                            string collectionId = collectionIdElement.GetString();
                            collectionIds.Add(collectionId);
                        }
                        latestCollectionId = await getCurrentCollectionId.GetUserDataAsync(httpClient, collectionInputModel.PlayFabId) as string;

                        var responseData = new
                        {
                            code = 200,
                            status = "OK",
                            data = new
                            {
                                CollectionIds = collectionIds,
                                CurrentCollecitonId = latestCollectionId
                            }
                        };

                        return new JsonResult(responseData);
                    }
                }
            }
            // Add the default return statement here
            return new JsonResult(new { code = 400, status = "Bad Request", error = "Unable to retrieve collection data" });
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
                { "CreateAccount", true },
            };
            string json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-SecretKey", SecretKey);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            JsonDocument jsonResult = JsonDocument.Parse(responseBody);

            JsonElement statusProperty = default;
            JsonElement dataProperty = default;
            JsonElement newlyCreatedProperty = default;
            JsonElement entityTokenProperty = default;

            string latestCollectionId = string.Empty;

            StateModel stateModel = new StateModel();
            PlayerDataService playerDataService = new PlayerDataService();
            UniqueIdModel uniqueIdModel = new UniqueIdModel();

            uniqueIdModel.GenerateUniqueId();

            string randomState = stateModel.GetRandomState();

            if (jsonResult.RootElement.TryGetProperty("status", out statusProperty) &&
                statusProperty.GetString() == "OK")
            {
                if (jsonResult.RootElement.TryGetProperty("data", out dataProperty) &&
                    dataProperty.TryGetProperty("NewlyCreated", out newlyCreatedProperty) &&
                    newlyCreatedProperty.GetBoolean() &&
                    dataProperty.TryGetProperty("EntityToken", out entityTokenProperty))
                {
                    string playFabId = jsonResult.RootElement.GetProperty("data").GetProperty("PlayFabId").GetString();
                    string entityToken = jsonResult.RootElement.GetProperty("data").GetProperty("EntityToken").GetProperty("EntityToken").GetString();
                    string entityId = jsonResult.RootElement.GetProperty("data").GetProperty("EntityToken").GetProperty("Entity").GetProperty("Id").GetString();

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
                        {"CollectionId", randomState+"-"+uniqueIdModel.uniqueId }
                    };

                    string collectionJson = JsonSerializer.Serialize(requestBody);
                    var collectionContent = new StringContent(collectionJson, Encoding.UTF8, "application/json");

                    string urlCollectionIds = $"https://{titleId}.playfabapi.com/Inventory/AddInventoryItems";
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("X-EntityToken", entityToken);
                    HttpResponseMessage responseCollectionIds = await httpClient.PostAsync(urlCollectionIds, collectionContent);

                    string responseBodyCollectionIds = await responseCollectionIds.Content.ReadAsStringAsync();
                    dynamic jsonResultCollectionIds = JsonSerializer.Deserialize<dynamic>(responseBodyCollectionIds);

                    latestCollectionId = requestBody["CollectionId"].ToString();

                    await playerDataService.UpdateUserDataAsync(httpClient, playFabId, latestCollectionId);

                }
            }

            return jsonResult;
        }

        [HttpGet("state")]
        public async Task<dynamic> GetAllStateAsync(HttpClient httpClient)
        {
            string titleId = "2506C";
            string url = $"https://{titleId}.playfabapi.com/Server/GetTitleData";

            var data = new Dictionary<string, object>
            {
                { "Keys", new string[] { "States" } },
            };

            string json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-SecretKey", "ZN4R9AYPEYWMRPUR4TGWEOJZMZTDYKAG1RP3K8R3W3TFHQE5ZH");

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

            return jsonDocument;
        }

        [HttpPost("updateCollection")]
        public async Task<dynamic> UpdateUserDataAsync(HttpClient httpClient, [FromBody] UpdateStateModel updateStateModel)
        {
            string titleId = "2506C";
            string url = $"https://{titleId}.playfabapi.com/Server/UpdateUserData";

            var data = new Dictionary<string, object>
                {
                    { "PlayFabId", updateStateModel.playFabId },
                {
                    "Data", new Dictionary<string, string>
                    {
                        { "CurrentCollectionId", updateStateModel.currentCollectionId }
                    }
                }
            };

            string json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-SecretKey", "ZN4R9AYPEYWMRPUR4TGWEOJZMZTDYKAG1RP3K8R3W3TFHQE5ZH");

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }

}
