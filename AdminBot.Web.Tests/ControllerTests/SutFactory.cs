using AdminBot.Web.Tests.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AdminBot.Web.Tests.ControllerTests;

public static class SutFactory
{
   private const string SqlConnectionString = "Server=localhost:1433;Database=dbo;User Id=sa;Password=Pa23@!Ze7&;";

   public static Sut Create()
   {
      var fakeBotClient = new FakeBotClient();
      var fakeDateTimeProvider = new FakeDateTimeProvider();
      
      var application = new WebApplicationFactory<Program>()
         .WithWebHostBuilder(
            builder =>
            {
               builder.ConfigureServices(services 
                  => ApplicationRoot.ConfigureServices(
                     services: services,
                     botName: "test_bot",
                     sqlConnectionString: SqlConnectionString,
                     botClient: fakeBotClient,
                     dateTimeProvider: fakeDateTimeProvider,
                     defaultBanTtl: TimeSpan.FromHours(3),
                     defaultWarnsLimit: 2,
                     descriptionFilePath: "./Description.md"));
            });

      var httpClient = application.CreateClient();

      return new Sut(
         httpClient: httpClient);
   }
}