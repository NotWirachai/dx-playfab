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

    public class LoginInputModel
    {
        public string? CustomId { get; set; }

        public bool? CreateAccount { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class Player : ControllerBase
    {
        [HttpGet]
        public async Task<dynamic> GetInventoryCollectionIdsAsync(HttpClient httpClient, [FromHeader(Name = "X-EntityToken")] string entityToken)
        {
            string titleId = "2506C";
            string url = $"https://{titleId}.playfabapi.com/Inventory/GetInventoryCollectionIds";

            var data = new Dictionary<string, object> { { "Count", 1 }, };
           
            string json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("X-EntityToken", entityToken);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic jsonResult = JsonSerializer.Deserialize<dynamic>(responseBody);
            Console.WriteLine("response =>> " + responseBody);
            Console.WriteLine("jsonResult =>> " + jsonResult);
            return jsonResult;
        }

        [HttpPost]
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
            Console.WriteLine("response =>> " + responseBody);
            Console.WriteLine("jsonResult =>> " + jsonResult);
            return jsonResult;
        }
    }

}
