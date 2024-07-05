using TechLanches.Pagamento.Core;
using TechLanches.Pagamento.Domain.Enums;

namespace TechLanches.Pagamento.Domain.Aggregates
{
    public class Pagamento : Entity, IAggregateRoot
    {
        private Pagamento() { }

        public Pagamento(int pedidoId, decimal valor, FormaPagamento formaPagamento)
        {
            PedidoId = pedidoId;
            Valor = valor;
            StatusPagamento = StatusPagamento.Aguardando;
            FormaPagamento = formaPagamento;
            Ativar();
            Validar();
        }

        public Pagamento(string id, int pedidoId, decimal valor, StatusPagamento statusPagamento, bool ativo):base(id)
        {
            PedidoId = pedidoId;
            Valor = valor;
            StatusPagamento = statusPagamento;
            Ativo = ativo;
        }

        public int PedidoId { get; private set; }
        public decimal Valor { get; private set; }
        public StatusPagamento StatusPagamento { get; private set; }
        public FormaPagamento FormaPagamento { get; private set; }
        public bool Ativo { get; private set; }

        public void Validar()
        {
            if (PedidoId <= 0)
                throw new DomainException("O pagamento deve possuir um pedido.");

            if (Valor <= 0)
                throw new DomainException("O valor deve ser maior que zero.");

            if (!Ativo)
                throw new DomainException("O pagamento precisa estar ativo na criação.");
        }

        public void Reprovar()
        {
            if (StatusPagamento == StatusPagamento.Aprovado)
                throw new DomainException("O pagamento já foi aprovado, assim não podendo reprova-lo.");

            StatusPagamento = StatusPagamento.Reprovado;
        }

        public void Aprovar()
        {
            StatusPagamento = StatusPagamento.Aprovado;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Inativar()
        {
            Ativo = false;
        }
    }
}
