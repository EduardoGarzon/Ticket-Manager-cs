using TicketManager.Domain.Entities;

namespace TicketManager.Application.Relatorios;

public record RelatorioFuncionarioTicketsCompleto(Funcionario Funcionario, IReadOnlyList<TicketEntregue> Tickets)
{
    public int TotalTicketsFuncionario => Tickets.Sum(t => t.Quantidade);
}
