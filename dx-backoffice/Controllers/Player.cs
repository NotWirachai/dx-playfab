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
        string titleId = "2506C";

        [HttpPost("collectionId")]
        public async Task<dynamic> GetInventoryCollectionIdsAsync(HttpClient httpClient, [FromHeader(Name = "X-EntityToken")] string entityToken, [FromBody] CollectionInputModel collectionInputModel)
        {
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

        [HttpPost("NewCollectionId")]
        public async Task<dynamic> LogInCustomIdAsync(HttpClient httpClient, [FromHeader(Name = "X-EntityToken")] string entityToken, [FromBody] CollectionInputModel collectionModel)
        {
            string url = $"https://{titleId}.playfabapi.com/Inventory/GetInventoryCollectionIds";

            var data = new Dictionary<string, object>
    {
        { "Count", 10 },
    };

            string json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-EntityToken", entityToken);

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResult = JsonSerializer.Deserialize<dynamic>(responseBody);

            StateModel stateModel = new StateModel();
            UniqueIdModel uniqueIdModel = new UniqueIdModel();
            PlayerDataService playerDataService = new PlayerDataService();
            GetCurrentCollectionId getCurrentCollectionId = new GetCurrentCollectionId();
            uniqueIdModel.GenerateUniqueId();

            string randomState = stateModel.GetRandomState();

            string latestCollectionId = string.Empty;

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
                        latestCollectionId = await getCurrentCollectionId.GetUserDataAsync(httpClient, collectionModel.PlayFabId) as string;

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
                    else
                    {
                        // CollectionIds is empty
                        Console.WriteLine("CollectionIds is empty");

                        var requestBody = new Dictionary<string, object>
                    {
                        { "Amount", 1 },
                        { "Entity", new Dictionary<string, object>
                            {
                                { "Id", collectionModel.EntityId },
                                { "Type", "title_player_account" },
                                { "TypeString", "title_player_account" }
                            }
                        },
                        { "Item", new Dictionary<string, object>
                            {
                                { "Id", "eb5e205a-06e0-456e-a7a8-3fc04a1976bf" },
                                { "Type ", "bundle" },
                            }
                        },
                        {"CollectionId", randomState+"-"+uniqueIdModel.uniqueId }
                    };

                        string collectionJson = JsonSerializer.Serialize(requestBody);
                        var collectionContent = new StringContent(collectionJson, Encoding.UTF8, "application/json");

                        string urlCollectionIds = $"https://{titleId}.playfabapi.com/Inventory/AddInventoryItems";
                        HttpResponseMessage responseCollectionIds = await httpClient.PostAsync(urlCollectionIds, collectionContent);

                        string responseBodyCollectionIds = await responseCollectionIds.Content.ReadAsStringAsync();
                        dynamic jsonResultCollectionIds = JsonSerializer.Deserialize<dynamic>(responseBodyCollectionIds);

                        latestCollectionId = requestBody["CollectionId"].ToString();

                        await playerDataService.UpdateUserDataAsync(httpClient, collectionModel.PlayFabId, latestCollectionId);
                        var newResponseData = new
                        {
                            code = 200,
                            status = "OK",
                            data = new
                            {
                                CurrentCollecitonId = latestCollectionId
                            }
                        };
                        return new JsonResult(newResponseData);
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

        [HttpGet("state")]
        public async Task<dynamic> GetAllStateAsync(HttpClient httpClient)
        {
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

        [HttpPost("addCollectionId")]
        public async Task<dynamic> AddCollectionId(HttpClient httpClient, [FromHeader(Name = "X-EntityToken")] string entityToken, [FromBody] CollectionInputModel collectionModel)
        {
            StateModel stateModel = new StateModel();
            UniqueIdModel uniqueIdModel = new UniqueIdModel();
            PlayerDataService playerDataService = new PlayerDataService();

            uniqueIdModel.GenerateUniqueId();

            string randomState = stateModel.GetRandomState();

            string latestCollectionId = string.Empty;

            var requestBody = new Dictionary<string, object>
                    {
                        { "Amount", 1 },
                        { "Entity", new Dictionary<string, object>
                            {
                                { "Id", collectionModel.EntityId },
                                { "Type", "title_player_account" },
                                { "TypeString", "title_player_account" }
                            }
                        },
                        { "Item", new Dictionary<string, object>
                            {
                                { "Id", "eb5e205a-06e0-456e-a7a8-3fc04a1976bf" },
                                { "Type ", "bundle" },
                            }
                        },
                        {"CollectionId", randomState+"-"+uniqueIdModel.uniqueId }
                    };

            string collectionJson = JsonSerializer.Serialize(requestBody);
            var collectionContent = new StringContent(collectionJson, Encoding.UTF8, "application/json");

            string urlCollectionIds = $"https://{titleId}.playfabapi.com/Inventory/AddInventoryItems";
            httpClient.DefaultRequestHeaders.Add("X-EntityToken", entityToken);
            HttpResponseMessage responseCollectionIds = await httpClient.PostAsync(urlCollectionIds, collectionContent);

            string responseBodyCollectionIds = await responseCollectionIds.Content.ReadAsStringAsync();
            dynamic jsonResultCollectionIds = JsonSerializer.Deserialize<dynamic>(responseBodyCollectionIds);

            latestCollectionId = requestBody["CollectionId"].ToString();

            await playerDataService.UpdateUserDataAsync(httpClient, collectionModel.PlayFabId, latestCollectionId);
            var newResponseData = new
            {
                code = 200,
                status = "OK",
                data = new
                {
                    CurrentCollecitonId = latestCollectionId
                }
            };
            return new JsonResult(newResponseData);
        }
    }

}
