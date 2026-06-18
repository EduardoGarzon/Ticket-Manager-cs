using TicketManager.Application.Relatorios;
using TicketManager.Domain.Repositories;

namespace TicketManager.Application.Services;

public class RelatorioService
{
    private readonly ITicketEntregueRepository _ticketRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;

    public RelatorioService(
        ITicketEntregueRepository ticketRepository,
        IFuncionarioRepository funcionarioRepository)
    {
        _ticketRepository = ticketRepository;
        _funcionarioRepository = funcionarioRepository;
    }

    public RelatorioPeriodo GerarPorPeriodo(DateTime inicio, DateTime fim)
    {
        var tickets = _ticketRepository.ObterPorPeriodo(inicio, fim);
        var funcionarios = _funcionarioRepository.ObterTodos().ToDictionary(f => f.Id);

        var itens = tickets
            .GroupBy(t => t.FuncionarioId)
            .Select(grupo => new ItemRelatorioFuncionario(
                grupo.Key,
                funcionarios.TryGetValue(grupo.Key, out var funcionario) ? funcionario.Nome : "Funcionário não encontrado",
                grupo.Sum(t => t.Quantidade)))
            .OrderBy(item => item.NomeFuncionario)
            .ToList();

        var totalGeral = itens.Sum(item => item.TotalTickets);

        return new RelatorioPeriodo(inicio, fim, itens, totalGeral);
    }
}