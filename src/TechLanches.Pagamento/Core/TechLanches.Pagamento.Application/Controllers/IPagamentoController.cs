﻿using TechLanches.Pagamento.Adapter.ACL.QrCode.DTOs;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Application.Controllers
{
    public interface IPagamentoController
    {
        Task<bool> RealizarPagamento(int pedidoId, StatusPagamentoEnum statusPagamento);
        Task<PagamentoResponseDTO> BuscarPagamentoPorPedidoId(int pedidoId);
        Task Cadastrar(int pedidoId, FormaPagamento formaPagamento, decimal valor);
        Task<string> GerarPagamentoEQrCodeMercadoPago(PedidoACLDTO pedidoMercadoPago);
        Task<PagamentoResponseACLDTO> ConsultarPagamentoMercadoPago(string pedidoComercial);
        Task<PagamentoResponseACLDTO> ConsultarPagamentoMockado(string pedidoComercial);
        Task<string> GerarPagamentoEQrCodeMockado(PedidoACLDTO pedidoMercadoPago);
    }
}
