using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using TechLanches.Pagamento.Adapter.API.Constantes;
using TechLanches.Pagamento.Application.Controllers;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Domain.Aggregates;
using TechLanches.Pagamento.Domain.Enums.Pedido;

namespace TechLanches.Pagamento.Adapter.API.Endpoints
{
    public static class PagamentoEndpoints
    {
        public static void MapPagamentoEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("api/pagamentos/status/{pedidoId}", BuscarStatusPagamentoPorPedidoId).WithTags(EndpointTagConstantes.TAG_PAGAMENTO)
               .WithMetadata(new SwaggerOperationAttribute(summary: "Buscar status pagamento", description: "Retorna o status do pagamento"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, type: typeof(PagamentoResponseDTO), description: "Status do pagamento encontrado com sucesso"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, type: typeof(ErrorResponseDTO), description: "Requisição inválida"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, type: typeof(ErrorResponseDTO), description: "Pedido não encontrado"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, type: typeof(ErrorResponseDTO), description: "Erro no servidor interno"))
               .RequireAuthorization();

            app.MapPost("api/pagamentos", CadastrarPagamento).WithTags(EndpointTagConstantes.TAG_PAGAMENTO)
              .WithMetadata(new SwaggerOperationAttribute(summary: "Cria pagamento", description: "Retorna o pagamento"))
              .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.Created, description: "Pagamento criado com sucesso"))
              .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, type: typeof(ErrorResponseDTO), description: "Requisição inválida"))
              .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, type: typeof(ErrorResponseDTO), description: "Erro no servidor interno"))
             .RequireAuthorization();

            app.MapPost("api/pagamentos/qrcode", GerarQrCode).WithTags(EndpointTagConstantes.TAG_PAGAMENTO)
             .WithMetadata(new SwaggerOperationAttribute(summary: "Gera qrcode", description: "Retorna o qrcode"))
             .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, description: "Qrcode gerado com sucesso"))
             .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, type: typeof(ErrorResponseDTO), description: "Requisição inválida"))
             .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, type: typeof(ErrorResponseDTO), description: "Erro no servidor interno"))
            .RequireAuthorization();

            app.MapPost("api/pagamentos/webhook/mockado", RealizarPagamentoMocado).WithTags(EndpointTagConstantes.TAG_PAGAMENTO)
               .WithMetadata(new SwaggerOperationAttribute(summary: "Webhook pagamento mockado", description: "Retorna o pagamento"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, description: "Pagamento encontrado com sucesso"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, type: typeof(ErrorResponseDTO), description: "Requisição inválida"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, type: typeof(ErrorResponseDTO), description: "Pagamento não encontrado"))
               .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, type: typeof(ErrorResponseDTO), description: "Erro no servidor interno"))
              .RequireAuthorization();
        }

        private static async Task<IResult> BuscarStatusPagamentoPorPedidoId([FromRoute] int pedidoId, [FromServices] IPagamentoController pagamentoController)
        {
            var pagamento = await pagamentoController.BuscarPagamentoPorPedidoId(pedidoId);

            if (pagamento is null)
                return Results.NotFound(new ErrorResponseDTO { MensagemErro = $"Nenhum pedido encontrado para o id: {pedidoId}", StatusCode = HttpStatusCode.NotFound });

            return Results.Ok(pagamento);
        }

        private static async Task<IResult> CadastrarPagamento([FromBody] PagamentoRequestDTO pagamentoRequestDTO, [FromServices] IPagamentoController pagamentoController)
        {
            var pagamento = await pagamentoController.Cadastrar(pagamentoRequestDTO.PedidoId, Domain.Enums.FormaPagamento.QrCodeMercadoPago, pagamentoRequestDTO.Valor);

            return pagamento is not null
               ? Results.Created($"api/pagamentos/{pagamento.Id}", pagamento)
               : Results.BadRequest(new ErrorResponseDTO { MensagemErro = "Erro ao cadastrar pagamento.", StatusCode = HttpStatusCode.BadRequest });
        }

        private static async Task<IResult> GerarQrCode([FromServices] IPagamentoController pagamentoController)
        {
            var qrCode = await pagamentoController.GerarQrCode();
            var pagamentoResponse = new PagamentoResponseDTO { QRCodeData = qrCode };
            return Results.Ok(pagamentoResponse);
        }
        private static async Task<IResult> RealizarPagamentoMocado([FromBody] PagamentoMocadoRequestDTO request, [FromServices] IPagamentoController pagamentoController,
            [FromServices] IPedidoController pedidoController,
            [FromServices] IMemoryCache memoryCache,
            [FromHeader(Name = "Authorization")] string cognitoAcessToken)
        {
            memoryCache.Set("authtoken", cognitoAcessToken, TimeSpan.FromMinutes(5));

            var pagamentoExistente = await pagamentoController.ConsultarPagamentoMockado(request.PedidoId.ToString());

            var pagamento = await pagamentoController.RealizarPagamento(request.PedidoId, pagamentoExistente.StatusPagamento);

            return pagamento == true
                ? Results.Ok()
                : Results.BadRequest(new ErrorResponseDTO { MensagemErro = $"Pagamento recusado.", StatusCode = HttpStatusCode.BadRequest });
        }
    }
}