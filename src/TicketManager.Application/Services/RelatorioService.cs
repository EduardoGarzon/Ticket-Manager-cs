using TicketManager.Application.Relatorios;
using TicketManager.Domain.Repositories;
using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;

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

    public RelatorioPeriodo GerarRelatorioPorPeriodo(DateTime inicio, DateTime fim)
    {
        var fimDoDia = fim.Date.AddDays(1).AddTicks(-1);

        var tickets = _ticketRepository.ObterPorPeriodo(inicio, fimDoDia);
        // .Where(t => t.Situacao == SituacaoCadastro.Ativo);

        var funcionarios = _funcionarioRepository.ObterTodos().ToDictionary(f => f.Id);

        var itens = tickets
            .GroupBy(t => t.FuncionarioId)
            .Select(grupo =>
            {
                var totalTickets = grupo.Sum(t => t.Quantidade);

                if (funcionarios.TryGetValue(grupo.Key, out var funcionario))
                    return new ItemRelatorioFuncionario(grupo.Key, funcionario.Nome, funcionario.Situacao, totalTickets);

                return new ItemRelatorioFuncionario(grupo.Key, "Funcionário não encontrado", SituacaoCadastro.Inativo, totalTickets);
            })
            .OrderBy(item => item.NomeFuncionario)
            .ToList();

        var totalGeral = itens.Sum(item => item.TotalTickets);

        return new RelatorioPeriodo(inicio, fim, itens, totalGeral);
    }

    public (IEnumerable<RelatorioFuncionarioTicketsCompleto> Itens, int TotalGeral) GerarRelatorioCompleto()
    {
        var funcionarios = _funcionarioRepository.ObterTodos();
        var tickets = _ticketRepository.ObterTodos();

        var ticketsPorFuncionario = tickets
            .GroupBy(t => t.FuncionarioId)
            .ToDictionary(grupo => grupo.Key, grupo => (IReadOnlyList<TicketEntregue>)grupo.ToList());

        var itens = funcionarios.Select(funcionario =>
        {
            var ticketsDoFuncionario = ticketsPorFuncionario.TryGetValue(funcionario.Id, out var lista)
                ? lista
                : new List<TicketEntregue>();

            return new RelatorioFuncionarioTicketsCompleto(funcionario, ticketsDoFuncionario);
        }).ToList();

        var totalGeral = itens.Sum(item => item.TotalTicketsFuncionario);

        return (itens, totalGeral);
    }
}