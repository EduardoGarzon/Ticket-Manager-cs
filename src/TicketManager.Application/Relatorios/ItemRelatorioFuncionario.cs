using TicketManager.Domain.Enums;

namespace TicketManager.Application.Relatorios;

public record ItemRelatorioFuncionario(int FuncionarioId, string NomeFuncionario, SituacaoCadastro SituacaoFuncionario, int TotalTickets);