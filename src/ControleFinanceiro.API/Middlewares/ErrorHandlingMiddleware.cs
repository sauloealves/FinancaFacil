using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.Common.Exceptions;

namespace ControleFinanceiro.API.Middlewares {
    public class ErrorHandlingMiddleware {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch(BusinessException ex) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;                

                await context.Response.WriteAsJsonAsync(
                    ApiResponse.Fail(ex.Message));

            } catch(Exception ex) {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse.Fail($"Erro interno no servidor. {ex.Message}"));
            }
        }
    }
}
