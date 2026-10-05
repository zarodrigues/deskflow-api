using System.Text.Json;
using DeskFlow.API.Exceptions;

namespace DeskFlow.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NaoEncontradoException ex)
        {
            await ResponderAsync(context, 404, ex.Message);
        }
        catch (RegraNegocioException ex)
        {
            await ResponderAsync(context, 400, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");
            await ResponderAsync(context, 500, "Ocorreu um erro interno. Tente novamente mais tarde.");
        }
    }

    private static async Task ResponderAsync(HttpContext context, int status, string mensagem)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var corpo = JsonSerializer.Serialize(new { status, erro = mensagem });
        await context.Response.WriteAsync(corpo);
    }
}