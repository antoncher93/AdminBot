using System.Text;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace AdminBot.Web.Tests.ControllerTests;

public class Sut
{
   private readonly HttpClient _httpClient;

   public Sut(
      HttpClient httpClient)
   {
      _httpClient = httpClient;
   }

   public Task<HttpResponseMessage> GetHealthCheckAsync()
   {
      return _httpClient.GetAsync(requestUri: "/api/healthcheck");
   }

   public async Task<HttpResponseMessage> PostUpdateAsync(Update update)
   {
      var bodyContent = JsonConvert.SerializeObject(update);

      var response = await _httpClient.PostAsync(
         requestUri: "/api/update",
         content: new StringContent(
            content: bodyContent,
            encoding: Encoding.UTF8,
            mediaType: "application/json"));

      return response;
   }

   public async Task<HttpResponseMessage> PostJsonUpdateAsync(string jsonContent)
   {
      var response = await _httpClient.PostAsync(
         requestUri: "/api/update",
         content: new StringContent(jsonContent, Encoding.UTF8, "application/json"));

      return response;
   }
}