namespace TicketManager.Infrastructure.Repositories;

internal class TicketEntregueRegistro
{
    public int Id { get; set; }
    public int FuncionarioId { get; set; }
    public int Quantidade { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public DateTime DataEntrega { get; set; }
}