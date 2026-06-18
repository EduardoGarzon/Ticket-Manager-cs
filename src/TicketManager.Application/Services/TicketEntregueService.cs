using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Exceptions;
using TicketManager.Domain.Repositories;

namespace TicketManager.Application.Services;

public class TicketEntregueService
{
    private readonly ITicketEntregueRepository _ticketRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;

    public TicketEntregueService(
        ITicketEntregueRepository ticketRepository,
        IFuncionarioRepository funcionarioRepository)
    {
        _ticketRepository = ticketRepository;
        _funcionarioRepository = funcionarioRepository;
    }

    public TicketEntregue RegistrarEntrega(int funcionarioId, int quantidade)
    {
        var funcionario = _funcionarioRepository.ObterPorId(funcionarioId)
            ?? throw new DomainException("Funcionário não encontrado.");

        // Decisão adicional, não pedida explicitamente no enunciado:
        if (funcionario.Situacao == SituacaoCadastro.Inativo)
            throw new DomainException("Não é possível registrar tickets para um funcionário inativo.");

        var ticket = new TicketEntregue(funcionarioId, quantidade);
        _ticketRepository.Adicionar(ticket);

        return ticket;
    }

    public void EditarQuantidade(int id, int quantidade)
    {
        var ticket = _ticketRepository.ObterPorId(id)
            ?? throw new DomainException("Ticket não encontrado.");

        ticket.AtualizarQuantidade(quantidade);
        _ticketRepository.Atualizar(ticket);
    }

    public void Ativar(int id)
    {
        var ticket = _ticketRepository.ObterPorId(id)
            ?? throw new DomainException("Ticket não encontrado.");

        ticket.Ativar();
        _ticketRepository.Atualizar(ticket);
    }

    public void Inativar(int id)
    {
        var ticket = _ticketRepository.ObterPorId(id)
            ?? throw new DomainException("Ticket não encontrado.");

        ticket.Inativar();
        _ticketRepository.Atualizar(ticket);
    }

    public IEnumerable<TicketEntregue> ListarPorFuncionario(int funcionarioId)
    {
        if (_funcionarioRepository.ObterPorId(funcionarioId) is null)
        {
            throw new DomainException("Funcionário não encontrado.");
        }
        return _ticketRepository.ObterPorFuncionario(funcionarioId);
    }
}