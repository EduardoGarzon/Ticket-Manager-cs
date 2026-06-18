namespace TicketManager.Application.Relatorios;

public record RelatorioPeriodo(DateTime Inicio, DateTime Fim, IReadOnlyList<ItemRelatorioFuncionario> Itens, int TotalGeral);