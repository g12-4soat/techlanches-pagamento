using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechLanches.Pagamento.Application.DTOs;
using TechLanches.Pagamento.Application.Presenters;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.UnitTests.UnitTests.Application.Presenters
{
    public class PagamentoPresenterTest
    {
        [Fact]
        public void Presenter_DeveConverterEntidadeParaDTO_ComSucesso()
        {
            // Arrange
            var presenter = new PagamentoPresenter();
            var pagamento = new Pagamento.Domain.Aggregates.Pagamento("1", 1, 100.0m, StatusPagamento.Aprovado);

            // Act
            var pagamentoDTO = presenter.ParaDto(pagamento);

            // Assert
            Assert.IsType<PagamentoResponseDTO>(pagamentoDTO);
            Assert.Equal(pagamento.Id, pagamentoDTO.Id);
            Assert.Equal(pagamento.PedidoId, pagamentoDTO.PedidoId);
            Assert.Equal(pagamento.Valor, pagamentoDTO.Valor);
            Assert.Equal(pagamento.StatusPagamento.ToString(), pagamentoDTO.StatusPagamento);
        }
    }
}
