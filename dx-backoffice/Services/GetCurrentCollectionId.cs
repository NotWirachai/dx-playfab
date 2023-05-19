using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace dx_backoffice.Services
{
  public class GetCurrentCollectionId
  {
    public async Task<dynamic> GetUserDataAsync(HttpClient httpClient, string playFabId)
    {
      string url = $"https://2506C.playfabapi.com/Server/GetUserData";
      httpClient.DefaultRequestHeaders.Add("X-SecretKey", "ZN4R9AYPEYWMRPUR4TGWEOJZMZTDYKAG1RP3K8R3W3TFHQE5ZH");

      var data = new Dictionary<string, object>
    {
        { "PlayFabId", playFabId },
        { "Keys", new string[] { "CurrentCollectionId" } }
    };

      string json = JsonSerializer.Serialize(data);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      HttpResponseMessage response = await httpClient.PostAsync(url, content);
      string responseBody = await response.Content.ReadAsStringAsync();
      dynamic jsonResult = JsonSerializer.Deserialize<dynamic>(responseBody);

      if (jsonResult.ValueKind == JsonValueKind.Object)
      {
        if (jsonResult.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Object)
        {
          if (dataElement.TryGetProperty("Data", out JsonElement nestedDataElement) && nestedDataElement.ValueKind == JsonValueKind.Object)
          {
            if (nestedDataElement.TryGetProperty("CurrentCollectionId", out JsonElement currentCollectionIdElement) && currentCollectionIdElement.ValueKind == JsonValueKind.Object)
            {
              if (currentCollectionIdElement.TryGetProperty("Value", out JsonElement valueElement) && valueElement.ValueKind == JsonValueKind.String)
              {
                string currentCollectionId = valueElement.GetString();
                return currentCollectionId; // Return the value as a 200 OK response
              }
            }
          }
        }
      }

      return null; // Return a 404 Not Found response if the value cannot be retrieved
    }
  }
}
