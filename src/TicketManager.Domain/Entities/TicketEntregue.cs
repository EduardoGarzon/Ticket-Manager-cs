using TicketManager.Domain.Enums;
using TicketManager.Domain.Exceptions;

namespace TicketManager.Domain.Entities;

public class TicketEntregue
{
    public int Id { get; private set; }
    public int FuncionarioId { get; private set; }
    public int Quantidade { get; private set; }
    public SituacaoCadastro Situacao { get; private set; }
    public DateTime DataEntrega { get; private set; }

    public TicketEntregue(int funcionarioId, int quantidade)
    {
        FuncionarioId = ValidarFuncionarioId(funcionarioId);
        Quantidade = ValidarQuantidade(quantidade);
        Situacao = SituacaoCadastro.Ativo;
        DataEntrega = DateTime.Now;
    }

    private TicketEntregue(int id, int funcionarioId, int quantidade, SituacaoCadastro situacao, DateTime dataEntrega)
    {
        Id = id;
        FuncionarioId = funcionarioId;
        Quantidade = quantidade;
        Situacao = situacao;
        DataEntrega = dataEntrega;
    }

    public static TicketEntregue Reconstituir(int id, int funcionarioId, int quantidade, SituacaoCadastro situacao, DateTime dataEntrega)
    {
        return new TicketEntregue(id, funcionarioId, quantidade, situacao, dataEntrega);
    }

    public void DefinirId(int id)
    {
        if (Id != 0)
            throw new DomainException("O Id deste registro já foi definido e não pode ser alterado.");

        Id = id;
    }

    public void AtualizarQuantidade(int quantidade)
    {
        Quantidade = ValidarQuantidade(quantidade);
    }

    public void Ativar()
    {
        Situacao = SituacaoCadastro.Ativo;
    }

    public void Inativar()
    {
        Situacao = SituacaoCadastro.Inativo;
    }

    private static int ValidarFuncionarioId(int funcionarioId)
    {
        if (funcionarioId <= 0)
            throw new DomainException("O ticket precisa estar vinculado a um funcionário válido.");

        return funcionarioId;
    }

    private static int ValidarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade de tickets entregues deve ser maior que zero.");

        return quantidade;
    }
}