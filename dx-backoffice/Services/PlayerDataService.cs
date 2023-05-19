using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace dx_backoffice.Services
{
  public class PlayerDataService
  {
    public async Task UpdateUserDataAsync(HttpClient httpClient, string playFabId, string currentCollectionId)
    {
      string url = $"https://2506C.playfabapi.com/Server/UpdateUserData";

      var data = new Dictionary<string, object>
        {
            { "PlayFabId", playFabId },
        {
            "Data", new Dictionary<string, string>
            {
                { "CurrentCollectionId", currentCollectionId }
            }
        }
    };

      string json = JsonSerializer.Serialize(data);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      httpClient.DefaultRequestHeaders.Add("X-SecretKey", "ZN4R9AYPEYWMRPUR4TGWEOJZMZTDYKAG1RP3K8R3W3TFHQE5ZH");

      HttpResponseMessage response = await httpClient.PostAsync(url, content);
      string responseBody = await response.Content.ReadAsStringAsync();

      Console.WriteLine(responseBody);  

    }
  }
}
