using System.Net;
using Serilog;

namespace AdminBot.Web.Middlewares;

public class HandleExceptionsMiddleware
{
    private readonly RequestDelegate _next;

    public HandleExceptionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception e)
        {
            Log.Logger
                .Error(
                    exception: e,
                    messageTemplate: "Unhandled Exception!");
            context.Response.StatusCode = (int)HttpStatusCode.OK;
        }

    }
}