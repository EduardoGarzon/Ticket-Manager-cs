using TicketManager.Domain.Entities;

namespace TicketManager.Domain.Repositories;

public interface ITicketEntregueRepository
{
    void Adicionar(TicketEntregue ticket);
    void Atualizar(TicketEntregue ticket);
    TicketEntregue? ObterPorId(int id);
    IEnumerable<TicketEntregue> ObterPorFuncionario(int funcionarioId);
    IEnumerable<TicketEntregue> ObterPorPeriodo(DateTime inicio, DateTime fim);
}