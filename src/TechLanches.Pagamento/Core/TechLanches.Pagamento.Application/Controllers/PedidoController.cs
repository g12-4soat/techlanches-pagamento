﻿using Microsoft.Extensions.Caching.Memory;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Gateways;
using TechLanches.Pagamento.Application.Gateways.Interfaces;
using TechLanches.Pagamento.Domain.Enums.Pedido;

namespace TechLanches.Pagamento.Application.Controllers
{
    public class PedidoController : IPedidoController
    {
        private readonly IPedidoGateway _pedidoGateway;

        public PedidoController(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _pedidoGateway = new PedidoGateway(httpClientFactory, cache);
        }

        public async Task<PedidoResponseDTO> TrocarStatus(int pedidoId, StatusPedido statusPedido)
        {
            var pedido = await _pedidoGateway.TrocarStatus(pedidoId, statusPedido);
            return pedido;
        }
    }
}