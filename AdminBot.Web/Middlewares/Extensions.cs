namespace AdminBot.Web.Middlewares;

public static class Extensions
{
    public static WebApplication UseExceptionHandling(
        this WebApplication app)
    {
        app.UseMiddleware<HandleExceptionsMiddleware>();
        return app;
    }
}